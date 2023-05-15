using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : ActionThing
{
    public override string thingType => "Attack";
    public override string thingSubType => "Attack";

    [SerializeField, Range(0f, 1f)]
    private float selectionMagnitude = 0.99f;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        if (user != null)
            transform.position = user.transform.position;
    }

    public WeaponThing weapon
    {
        get
        {
            if (_weapon == null)
                _weapon = GetComponentInParent<WeaponThing>(true);

            return _weapon;
        }
    }
    private WeaponThing _weapon;

    public override string thingName
    {
        get
        {
            if (string.IsNullOrEmpty(_thingName) || _thingName == "Attack")
            {
                if (weapon != null)
                    _thingName = weapon.thingName;
            }

            return _thingName;
        }
    }

    public override Sprite thingIcon
    {
        get
        {
            if (_thingIcon == null)
            {
                if (weapon != null)
                {
                    redColor = weapon.redColor;
                    greenColor = weapon.greenColor;
                    blueColor = weapon.blueColor;

                    _thingIcon = weapon.thingIcon;
                }
            }

            return _thingIcon;
        }
    }

    private AttackMenu attackMenu
    {
        get
        {
            if (_attackMenu == null)
            {
                if (user is CharacterThing)
                    _attackMenu = (user as CharacterThing).attackMenu;
            }

            return _attackMenu;
        }
    }
    private AttackMenu _attackMenu;

    private enum AttackState
    {
        None,
        PickingAttack,
        PickingTarget,
        Attacking
    }
    private List<Pathfinding.GraphNode> _reachableNodes;

    private AttackState attackState
    {
        get => _attackState;
        set
        {
            _attackState = value;

            attackMenu.SetActive(_attackState == AttackState.PickingAttack, weapon);

            if (_attackState == AttackState.PickingTarget)
                Nodes.UnoccupyNode(Nodes.gridGraph.GetNearest(user.transform.position).node);
            else
                Nodes.OccupyNode(Nodes.gridGraph.GetNearest(user.transform.position).node);

            switch (_attackState)
            {
                case AttackState.None:
                    CancelAction();
                    break;
                case AttackState.PickingAttack:
                    targetPosition = user.transform.position;
                    Nodes.instance.HideNodes();
                    break;
                case AttackState.PickingTarget:
                    if (Nodes.instance != null)
                    {
                        _reachableNodes = Nodes.GetNodesInRadius(user.transform.position, attack.range, Vector2.one * attack.range);

                        Nodes.instance.DisplayNodes(_reachableNodes);
                        Nodes.instance.ColorNodeObjects(_reachableNodes);
                    }
                    break;
                case AttackState.Attacking:
                    if (attack != null)
                    {
                        if (GameManager.GetCharacterAtPosition(targetPosition))
                            attack.StartAttack(user, GameManager.GetCharacterAtPosition(targetPosition));
                        else if (attack.canUseEmptyTarget)
                            attack.StartAttack(user, targetPosition);
                        else
                        {
                            _attackState = AttackState.PickingTarget;
                            return;
                        }

                        if (_attackState == AttackState.Attacking && Nodes.instance != null)
                            Nodes.instance.HideNodes();

                        attack.AttackSequenceFinished.AddListener(OnAttackSequenceFinished);
                    }
                    break;
            }

            // GameplayCamera.SetCameraObject((_attackState == AttackState.PickingAttack) ? this : user);

            // Debug.Log("AttackState: " + _attackState);
        }
    }
    private AttackState _attackState = AttackState.PickingAttack;

    public override void Use(GameThing user)
    {
        base.Use(user);

        ResetAttack();
        attackState = AttackState.PickingAttack;
    }

    private void ResetAttack()
    {
        // Reset all the variables that need to be reset here
        _attackState = AttackState.None;
        _reachableNodes = null;
        targetPosition = Vector3.zero;
        targetDirection = Vector2Int.zero;
        if (attack != null)
        {
            attack.AttackSequenceFinished.RemoveListener(OnAttackSequenceFinished);
            attack = null;
        }
    }


    protected override IEnumerator RunAction()
    {
        yield return null;
    }

    public override void Move(Vector2 direction)
    {
        switch (attackState)
        {
            case AttackState.PickingAttack:
                PickAttack(direction);
                break;
            case AttackState.PickingTarget:
                PickTarget(direction);
                break;
            case AttackState.Attacking:
                if (attack != null)
                    attack.Move(direction);
                break;
        }
    }

    private AttackSequenceThing attack;

    private void PickAttack(Vector2 direction)
    {
        if (direction.magnitude > selectionMagnitude)
        {
            attackMenu.PickAttack(direction, out attack);
        }
    }

    private Vector3 targetPosition;
    private Vector2Int targetDirection;

    private void PickTarget(Vector2 direction)
    {
        if (direction.magnitude > selectionMagnitude)
        {
            Vector2Int newTargetDirection = Vector2Int.RoundToInt(direction.normalized);

            if (newTargetDirection != targetDirection)
            {
                targetDirection = newTargetDirection;
                Vector3 potentialTargetPosition =
                    (attack.range <= 1)
                        ? user.transform.position + (new Vector3(targetDirection.x, 0, targetDirection.y) * attack.range)
                        : targetPosition + (new Vector3(targetDirection.x, 0, targetDirection.y) * attack.range);

                // Check if the potential target position is within the attack range
                Pathfinding.GraphNode potentialTargetNode = Nodes.gridGraph.GetNearest(potentialTargetPosition).node;
                if (_reachableNodes.Contains(potentialTargetNode))
                {
                    // Color the previous target node
                    if (Nodes.instance != null && targetPosition != Vector3.zero)
                    {
                        Nodes.instance.ColorNodeObject(Nodes.gridGraph.GetNearest(targetPosition).node, Nodes.instance.walkableColor);
                    }

                    targetPosition = potentialTargetPosition;

                    // Color the new target node
                    if (Nodes.instance != null)
                    {
                        Nodes.instance.ColorNodeObject(potentialTargetNode, Nodes.instance.currentColor);
                    }
                }
            }
        }
    }

    private void OnAttackSequenceFinished(bool allStepsSuccessful)
    {
        // Unsubscribe from the AttackSequenceFinished event to avoid memory leaks
        if (attack != null)
        {
            attack.AttackSequenceFinished.RemoveListener(OnAttackSequenceFinished);
            attack = null;
        }

        Debug.Log("AttackSequenceFinished: " + allStepsSuccessful);

        // End the action when the attack sequence finishes
        EndAction();
    }

    public override void PrimaryAction(bool pressed)
    {
        switch (attackState)
        {
            default:
                if (pressed)
                    attackState++;
                break;
            case AttackState.PickingAttack:
                if (pressed && attack != null)
                    attackState++;
                break;
            case AttackState.PickingTarget:
                if (pressed && targetPosition != Vector3.zero)
                    attackState++;
                break;
            case AttackState.Attacking:
                if (attack != null)
                    attack.PrimaryAction(pressed);
                break;
        }
    }

    public override void SecondaryAction(bool pressed)
    {
        switch (attackState)
        {
            default:
                if (pressed)
                    attackState--;
                break;
            case AttackState.Attacking:
                if (attack != null)
                    attack.SecondaryAction(pressed);
                break;
        }
    }
}
