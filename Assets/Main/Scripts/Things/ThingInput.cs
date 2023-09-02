using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Pathfinding;
using Sirenix.OdinInspector;

public class ThingInput : UnsavedThing
{
    public bool isPlayer = true;

    public PlayerInput playerInput
    {
        get
        {
            if (_playerInput == null)
                TryGetComponent(out _playerInput);

            return _playerInput;
        }
    }
    private PlayerInput _playerInput;

    #region Vibration

    private Gamepad gamepad
    {
        get
        {
            if (_gamepad == null && !_hasCheckedForGamepad)
            {
                _hasCheckedForGamepad = true;
                if (playerInput != null)
                    _gamepad = playerInput.devices.FirstOrDefault(device => device is Gamepad) as Gamepad;
            }

            return _gamepad;
        }
    }
    private Gamepad _gamepad;
    private bool _hasCheckedForGamepad = false;

    private List<AudioSource> audioSources = new List<AudioSource>();
    private bool isVibrationCoroutineRunning = false;

    public void AddAudioSourceForVibration(AudioSource source)
    {
        if (source == null || audioSources.Contains(source)) return;

        audioSources.Add(source);

        if (!isVibrationCoroutineRunning)
        {
            StartCoroutine(VibrateBasedOnAudio());
        }
    }

    private IEnumerator VibrateBasedOnAudio()
    {
        isVibrationCoroutineRunning = true;

        while (audioSources.Count > 0)
        {
            float cumulativeVibration = 0;

            for (int i = audioSources.Count - 1; i >= 0; i--)
            {
                if (audioSources[i] == null || !audioSources[i].isPlaying)
                {
                    audioSources.RemoveAt(i);
                }
                else
                {
                    cumulativeVibration += GetCurrentAudioVolume(audioSources[i]);
                }
            }

            cumulativeVibration = Mathf.Clamp01(cumulativeVibration);
            gamepad?.SetMotorSpeeds(cumulativeVibration, cumulativeVibration);

            yield return null; // wait for the next frame
        }

        isVibrationCoroutineRunning = false;
    }

    private float GetCurrentAudioVolume(AudioSource source)
    {
        float[] data = new float[256];
        source.GetOutputData(data, 0);
        float a = 0;
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }
        return a / 256.0f;
    }

    #endregion

    #region AI Variables

    private Coroutine _actionCoroutine;
    [SerializeField] private List<GameThing> _targets = new List<GameThing>();
    private int _currentTargetIndex = 0;

    public bool canControl
    {
        get => _canControl;
        set
        {
            _canControl = value;

            if (isPlayer)
            {
                // if (value)
                //     GameManager.instance?.touchControls?.Select();

                // Debug.Log($"Player can {(value ? "" : "not ")}control");
                return;
            }

            if (value)
            {
                foreach (Inventory.ThingSlot slot in inventory.thingSlots)
                {
                    if (slot.thing && slot.thing is CharacterThing)
                    {
                        CharacterThing character = slot.thing as CharacterThing;

                        if (character == GameManager.currentCharacter)
                        {
                            _actionCoroutine = StartCoroutine(ActionCoroutine(character));
                            character.movementController.canControl = MovementController.ControlLevel.Full;
                            character.movementController.canMove = true;
                            character.movementController.canJump = true;
                        }
                        else
                        {
                            character.movementController.canControl = MovementController.ControlLevel.None;
                            character.movementController.canMove = false;
                            character.movementController.canJump = false;
                        }
                    }
                }

            }
            else
            {
                if (_actionCoroutine != null)
                {
                    StopCoroutine(_actionCoroutine);
                    _actionCoroutine = null;

                    foreach (Inventory.ThingSlot slot in inventory.thingSlots)
                    {
                        if (slot.thing && slot.thing is CharacterThing)
                        {
                            CharacterThing character = slot.thing as CharacterThing;
                            character.movementController.canControl = MovementController.ControlLevel.None;
                            character.movementController.canMove = false;
                            character.movementController.canJump = false;
                        }
                    }
                }
            }
        }
    }
    [SerializeField] private bool _canControl = false;

    enum AIState
    {
        ChoosingAction,
        Idling,
        Moving,
        Attacking,
        Healing,
        Fleeing,
        EndingTurn
    }

    [SerializeField] private float _actionDelay = 1f;

    private IEnumerator ActionCoroutine(CharacterThing thing)
    {
        GameManager.instance?.emptyMenu?.Select();

        AIState currentState = AIState.Idling;
        AIState nextState = AIState.ChoosingAction;

        float timer = 0f;

        bool performedAction = false;
        bool attemptedHealing = false;

        _currentTargetIndex = 0;
        List<GraphNode> path = new List<GraphNode>();

        // Get the movement range
        int numberOfDiceRolls = thing.variables.GetVariable("movement");
        int movementRange = 0;

        thing.UnoccupyCurrentNode(true);

        for (int i = 0; i < numberOfDiceRolls; i++)
        {
            // Get random number between 1 and 6
            int currentDiceValue = Random.Range(1, 7);

            movementRange += currentDiceValue; // Add the current dice value to the sum of dice rolls
        }

        // Get the targets in the room and nearby
        Level.Room? myRoom = GameManager.instance?.level.GetRoom(thing);
        if (myRoom != null)
        {
            if (_targets == null)
                _targets = new List<GameThing>();
            else
                _targets.Clear();

            // Get all the things in the room
            List<GameThing> thingsInRoom = new List<GameThing>();
            foreach (GameThing thingInRoom in myRoom.Value.things)
            {
                if (thingInRoom != null && thingInRoom != thing)
                    thingsInRoom.Add(thingInRoom);
            }

            // Get the distance of each thing in the room to the character
            if (thingsInRoom.Count > 0)
            {
                // Sort the things by node distance to the character
                thingsInRoom = thingsInRoom
                    .Select(thingInRoom => (thingInRoom, distance: Nodes.GetNodeDistance(thing.transform.position, thingInRoom.transform.position)))
                    // .Where(tuple => tuple.distance <= movementRange)
                    .OrderBy(tuple => tuple.distance)
                    .ThenBy(tuple => tuple.thingInRoom is CharacterThing) // Move CharacterThings to the end of the list
                    .Select(tuple => tuple.thingInRoom)
                    .ToList();
            }

            foreach (GameThing thingInRoom in thingsInRoom)
            {
                // Check if the thing is an item
                switch (thingInRoom.GetType().Name)
                {
                    case "ItemThing":
                        // Check if the item is a healing item
                        if (thingInRoom.variables.GetVariable("health") > 0)
                        {
                            // Add the item to our list of targets
                            _targets.Add(thingInRoom);
                        }
                        break;

                    case "CharacterThing":
                        // Check if the character is an enemy
                        if ((thingInRoom as CharacterThing).characterTeam != thing.characterTeam)
                        {
                            // Add the character to our list of targets
                            _targets.Add(thingInRoom);
                        }
                        break;
                }
            }

            Debug.Log($"Found {_targets.Count} targets");
        }

        // The state of attack
        WeaponThing weapon = null;
        // The attack sequence
        AttackSequenceThing attackSequence = null;

        while (_canControl)
        {
            switch (currentState)
            {
                case AIState.Idling:
                    movement = Vector2.zero;
                    primaryAction = false;
                    secondaryAction = false;

                    timer = 0f;
                    while (timer < _actionDelay)
                    {
                        timer += Time.deltaTime;
                        yield return null;
                    }

                    currentState = nextState;
                    break;

                case AIState.ChoosingAction:
                    // Choose the next action based on the state of the thing
                    if (!performedAction && !attemptedHealing && thing.variables.GetVariable("health") / (float)thing.variables.GetVariable("maxHealth") < 0.5f)
                    {
                        // We're low on health, try to heal
                        nextState = AIState.Healing;
                    }
                    else if (_targets.Count > 0)
                    {
                        if (_currentTargetIndex < _targets.Count)
                        {
                            // We have targets, move to them
                            GetPathToCurrentTarget(thing, out path, movementRange);
                            nextState = AIState.Moving;
                        }
                        else if (!performedAction)
                        {
                            // We've reached the end target, attack
                            nextState = AIState.Attacking;
                        }
                        else
                        {
                            // We've performed an action, end our turn
                            nextState = AIState.EndingTurn;
                        }
                    }
                    else
                    {
                        // We have no targets, end our turn
                        nextState = AIState.EndingTurn;
                    }
                    currentState = nextState;
                    break;

                case AIState.Healing:
                    // Check if we have any healing items
                    List<GameThing> healingItems = thing.inventory.ContainsVariable("health");

                    // Sort the healing items by how close their healing value is to the amount of health we need to get to max health
                    int healthNeeded = thing.variables.GetVariable("maxHealth") - thing.variables.GetVariable("health");
                    healingItems.Sort((a, b) => Mathf.Abs(a.variables.GetVariable("health") - healthNeeded).CompareTo(Mathf.Abs(b.variables.GetVariable("health") - healthNeeded)));

                    if (healingItems.Count > 0)
                    {
                        // We have healing items, use one
                        healingItems[0].Use(thing);
                        performedAction = true;
                    }
                    attemptedHealing = true;

                    currentState = AIState.Idling;
                    nextState = AIState.ChoosingAction;
                    break;

                case AIState.Moving:
                    if (_currentTargetIndex < _targets.Count)
                    {
                        // Get the path to the current target
                        Vector3 start = thing.transform.position;
                        Vector3 end = _targets[_currentTargetIndex].transform.position;

                        // Move to the next node in the path
                        if (path.Count > 1)
                        {
                            Vector3 nextNode = (Vector3)path[1].position;
                            float distance = Vector3.Distance(thing.transform.position, nextNode);

                            // Debug.Log($"Distance from {thing.thingName} to next node: {distance}");

                            if (distance < 0.1f)
                            {
                                // We've reached the node, move to the next one
                                path.RemoveAt(0);
                                if (path.Count > 1)
                                    nextNode = (Vector3)path[1].position;
                                else
                                    nextNode = end;
                                distance = Vector3.Distance(thing.transform.position, nextNode);
                            }

                            movement = new Vector2(nextNode.x - thing.transform.position.x, nextNode.z - thing.transform.position.z).normalized;

                            if (nextNode.y - thing.transform.position.y > 0.5f)
                            {
                                primaryAction = true;
                            }
                        }
                        else
                        {
                            // We've reached the target
                            _currentTargetIndex++;

                            if (_currentTargetIndex < _targets.Count)
                                GetPathToCurrentTarget(thing, out path, movementRange);
                        }
                    }
                    else
                    {
                        // We've reached the end target, attack
                        currentState = AIState.Idling;
                        nextState = AIState.ChoosingAction;

                        thing.OccupyCurrentNode();
                    }
                    break;

                case AIState.Attacking:
                    if (thing.attackMenu.weapon == null)
                    {
                        // Check if we have WeaponThings
                        List<WeaponThing> weapons = thing.GetComponentsInChildren<WeaponThing>(true).ToList();

                        for (int i = weapons.Count - 1; i >= 0; i--)
                        {
                            if (weapons[i].upAttackSlot == null && weapons[i].downAttackSlot == null && weapons[i].sideAttackSlot == null)
                            {
                                // We don't have any attacks, remove the WeaponThing
                                weapons.RemoveAt(i);
                            }
                        }

                        if (weapons.Count > 0)
                        {
                            // We have WeaponThings, use a random one
                            weapon = weapons[Random.Range(0, weapons.Count)];

                            // We have an WeaponThing, use it
                            weapon.Use(thing);

                            // Pick an attack
                            List<AttackSequenceThing> attackSequences = new List<AttackSequenceThing>();
                            if (weapon.upAttackSlot)
                                attackSequences.Add(weapon.upAttackSlot as AttackSequenceThing);
                            if (weapon.downAttackSlot)
                                attackSequences.Add(weapon.downAttackSlot as AttackSequenceThing);
                            if (weapon.sideAttackSlot)
                                attackSequences.Add(weapon.sideAttackSlot as AttackSequenceThing);

                            if (attackSequences.Count > 0)
                                attackSequence = attackSequences[Random.Range(0, attackSequences.Count)];

                            if (attackSequence != null)
                            {
                                // We've picked an attack, use it
                                thing.attackMenu.PickAttack(attackSequence, thing.attackAction);
                            }
                            else
                            {
                                // We couldn't find an attack, end our turn
                                Debug.Log($"Couldn't find an attack for {thing.thingName}");

                                nextState = AIState.EndingTurn;
                                currentState = AIState.Idling;
                            }
                            break;

                        }
                        else
                        {
                            // We don't have any AttackActions, end our turn
                            Debug.Log($"Couldn't find any AttackActions for {thing.thingName}");

                            nextState = AIState.EndingTurn;
                            currentState = AIState.Idling;
                        }
                    }
                    else if (thing.attackAction.attackSequenceThing != null)
                    {
                        switch (thing.attackAction.attackState)
                        {
                            case AttackAction.AttackState.PickingTarget:
                                // Attack the current target
                                if (_targets != null && _targets[_targets.Count - 1] != null)
                                {
                                    if (Vector3.Distance(thing.transform.position, _targets[_targets.Count - 1].transform.position) < thing.attackAction.attackSequenceThing.range)
                                        thing.attackAction.PickTarget(_targets[_targets.Count - 1].position);
                                    else
                                    {
                                        // We're too far away, end our turn
                                        thing.attackAction.attackState = AttackAction.AttackState.None;

                                        nextState = AIState.EndingTurn;
                                        currentState = AIState.Idling;
                                    }
                                }
                                break;

                            case AttackAction.AttackState.Attacking:
                                if (thing.attackAction.attackSequenceThing != null)
                                {
                                    // Wait for the attack to finish
                                }
                                else
                                {
                                    // We've finished attacking, end our turn
                                    nextState = AIState.EndingTurn;
                                    currentState = AIState.Idling;

                                    performedAction = true;
                                }
                                break;
                        }

                        if (thing.attackAction.attackState != AttackAction.AttackState.None && thing.attackAction.attackState != AttackAction.AttackState.Attacking)
                        {
                            weapon.PrimaryAction(true);
                            weapon.PrimaryAction(false);
                        }
                    }
                    else
                    {
                        // We don't have an AttackAction, end our turn
                        Debug.Log($"Couldn't find an AttackAction for {thing.thingName}");

                        nextState = AIState.EndingTurn;
                        currentState = AIState.Idling;
                    }

                    break;

                case AIState.EndingTurn:
                    // We've finished our turn, reset the state
                    movement = Vector2.zero;
                    primaryAction = false;
                    secondaryAction = false;

                    _canControl = false;
                    break;
            }

            yield return null;
        }

        thing.OccupyCurrentNode();

        // Go to the next character
        GameManager.NextCharacter();
    }

    private void GetPathToCurrentTarget(CharacterThing thing, out List<GraphNode> path, int maxDistance = 0)
    {
        path = Nodes.GetPathToNode(thing.transform.position, _targets[_currentTargetIndex].transform.position, maxDistance);
        // Nodes.instance.DisplayNodes(path);
    }

    #endregion

    void OnEnable()
    {
        GameManager.AddPlayer(this);
    }

    void OnDisable()
    {
        GameManager.RemovePlayer(this);
    }

    public Vector2 movement
    {
        get => _movement;
        set
        {
            _movement = value;

            _moving = value.magnitude > 0;

            foreach (Inventory.ThingSlot slot in inventory.thingSlots)
                if (slot.thing)
                    slot.thing.Move(_movement);
        }
    }
    private Vector2 _movement;
    private bool _moving;

    public bool primaryAction
    {
        get => _primaryAction;
        set
        {
            _primaryAction = value;

            foreach (Inventory.ThingSlot slot in inventory.thingSlots)
                if (slot.thing)
                    slot.thing.PrimaryAction(_primaryAction);
        }
    }
    private bool _primaryAction;

    public bool secondaryAction
    {
        get => _secondaryAction;
        set
        {
            _secondaryAction = value;

            foreach (Inventory.ThingSlot slot in inventory.thingSlots)
                if (slot.thing)
                    slot.thing.SecondaryAction(_secondaryAction);

            if (canControl && _secondaryAction)
                Menu.currentMenuOption?.PreviousMenu();
        }
    }
    private bool _secondaryAction;

    public bool tertiaryAction
    {
        get => _tertiaryAction;
        set
        {
            _tertiaryAction = value;

            foreach (Inventory.ThingSlot slot in inventory.thingSlots)
                if (slot.thing)
                    slot.thing.TertiaryAction(_tertiaryAction);
        }
    }
    private bool _tertiaryAction;

    public bool quaternaryAction
    {
        get => _quaternaryAction;
        set
        {
            _quaternaryAction = value;

            foreach (Inventory.ThingSlot slot in inventory.thingSlots)
                if (slot.thing)
                    slot.thing.QuaternaryAction(_quaternaryAction);
        }
    }
    private bool _quaternaryAction;

    public bool leftAction
    {
        get => _leftAction;
        set
        {
            _leftAction = value;

            foreach (Inventory.ThingSlot slot in inventory.thingSlots)
                if (slot.thing)
                    slot.thing.LeftAction(_leftAction);
        }
    }
    private bool _leftAction;

    public bool rightAction
    {
        get => _rightAction;
        set
        {
            _rightAction = value;

            foreach (Inventory.ThingSlot slot in inventory.thingSlots)
                if (slot.thing)
                    slot.thing.RightAction(_rightAction);
        }
    }
    private bool _rightAction;

    public bool pause
    {
        get => _pause;
        set
        {
            _pause = value;

            foreach (Inventory.ThingSlot slot in inventory.thingSlots)
                if (slot.thing && slot.thing is CharacterThing)
                    (slot.thing as CharacterThing).Pause();
        }
    }
    private bool _pause;

    public void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();
    }

    public void OnButton1(InputValue value)
    {
        primaryAction = value.isPressed;
    }

    public void OnButton2(InputValue value)
    {
        secondaryAction = value.isPressed;
    }

    public void OnButton3(InputValue value)
    {
        tertiaryAction = value.isPressed;
    }

    public void OnButton4(InputValue value)
    {
        quaternaryAction = value.isPressed;
    }

    public void OnButtonLeft(InputValue value)
    {
        leftAction = value.isPressed;
    }

    public void OnButtonRight(InputValue value)
    {
        rightAction = value.isPressed;
    }

    public void OnPause(InputValue value)
    {
        pause = value.isPressed;
    }
}
