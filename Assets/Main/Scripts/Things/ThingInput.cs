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
                        _actionCoroutine = StartCoroutine(ActionCoroutine(character));

                        if (character == GameManager.currentCharacter)
                        {
                            character.movementController.canControl = 2;
                            character.movementController.canMove = true;
                            character.movementController.canJump = true;
                        }
                        else
                        {
                            character.movementController.canControl = 0;
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
                            character.movementController.canControl = 0;
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

        bool attemptedHealing = false;

        _currentTargetIndex = 0;
        List<GraphNode> path = new List<GraphNode>();

        // Get the movement range
        int numberOfDiceRolls = thing.variables.GetVariable("movement");
        int movementRange = 0;

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
                    .Where(tuple => tuple.distance <= movementRange)
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
                    if (!attemptedHealing && (float)thing.variables.GetVariable("health") / (float)thing.variables.GetVariable("maxHealth") < 0.5f)
                    {
                        // We're low on health, try to heal
                        nextState = AIState.Healing;
                    }
                    else if (_targets.Count > 0)
                    {
                        // We have targets, move to them
                        GetPathToCurrentTarget(thing, out path);

                        nextState = AIState.Moving;
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
                                GetPathToCurrentTarget(thing, out path);
                        }
                    }
                    else
                    {
                        // We've reached the end target
                        currentState = AIState.Idling;
                        nextState = AIState.ChoosingAction;

                        _targets.Clear();
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

        // Go to the next character
        GameManager.NextCharacter();
    }

    private void GetPathToCurrentTarget(CharacterThing thing, out List<GraphNode> path)
    {
        path = Nodes.GetPathToNode(thing.transform.position, _targets[_currentTargetIndex].transform.position);
    }

    public void SetTargets(List<GameThing> targets, CharacterThing endTarget)
    {
        _targets = targets;

        if (endTarget != null)
            _targets.Add(endTarget);
    }

    [Button("Set Target - Character"), HideIf("isPlayer")]
    public void SetTargets(CharacterThing endTarget)
    {
        _targets = new List<GameThing>();
        _targets.Add(endTarget);

        canControl = false;
        canControl = true;
    }

    [Button("Set Targets - Things"), HideIf("isPlayer")]
    public void SetTargets(List<GameThing> targets)
    {
        _targets = targets;

        canControl = false;
        canControl = true;
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
