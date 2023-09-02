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

    public AttackSequenceThing attackSequenceThing
    {
        get => _attackSequenceThing;
        set
        {
            if (_attackSequenceThing != null)
            {
                _attackSequenceThing.AttackSequenceFinished.RemoveListener(OnAttackSequenceFinished);
            }

            _attackSequenceThing = value;

            if (_attackSequenceThing != null)
            {
                _attackSequenceThing.AttackSequenceFinished.AddListener(OnAttackSequenceFinished);
            }
        }
    }
    private AttackSequenceThing _attackSequenceThing;

    public override string thingName
    {
        get
        {
            if (string.IsNullOrEmpty(_thingName) || _thingName == "Attack")
            {
                if (attackSequenceThing != null)
                    _thingName = attackSequenceThing.thingName;
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
                if (attackSequenceThing != null)
                {
                    redColor = attackSequenceThing.redColor;
                    greenColor = attackSequenceThing.greenColor;
                    blueColor = attackSequenceThing.blueColor;

                    _thingIcon = attackSequenceThing.thingIcon;
                }
            }

            return _thingIcon;
        }
    }

    public enum AttackState
    {
        None,
        PickingTarget,
        Attacking
    }
    private List<Pathfinding.GraphNode> _reachableNodes;

    public AttackState attackState
    {
        get => _attackState;
        set
        {
            _attackState = value;

            if (_attackState != AttackState.PickingTarget)
            // Nodes.UnoccupyNode(Nodes.instance.gridGraph.GetNearest(user.transform.position).node);
            // else
            {
                // Nodes.OccupyNode(Nodes.instance.gridGraph.GetNearest(user.transform.position).node, user);
                Nodes.instance?.HideNodes();
            }

            switch (_attackState)
            {
                case AttackState.None:
                    CancelAction();
                    break;
                case AttackState.PickingTarget:
                    if (attackSequenceThing == null)
                    {
                        CancelAction();
                        break;
                    }

                    if (Nodes.instance != null)
                    {
                        _reachableNodes = Nodes.GetNodesInRadius(user.transform.position, attackSequenceThing.range, Vector2.one * attackSequenceThing.range);

                        if (user is CharacterThing && (user as CharacterThing).input.isPlayer)
                        {
                            Nodes.instance.DisplayNodes(_reachableNodes);
                            Nodes.instance.ColorNodeObjects(_reachableNodes);
                        }
                    }
                    break;
                case AttackState.Attacking:
                    if (attackSequenceThing != null)
                    {
                        CharacterThing character = GameManager.GetCharacterAtNode(Nodes.instance.gridGraph.GetNearest(targetPosition).node);
                        if (character != null && character != user)
                            attackSequenceThing.StartAttack(user, character);
                        else if (attackSequenceThing.canUseEmptyTarget)
                            attackSequenceThing.StartAttack(user, targetPosition);
                        else
                        {
                            _attackState = AttackState.PickingTarget;
                            return;
                        }

                        attackSequenceThing.AttackSequenceFinished.AddListener(OnAttackSequenceFinished);
                    }
                    break;
            }

            // GameplayCamera.SetCameraObject((_attackState == AttackState.PickingAttack) ? this : user);

            // Debug.Log("AttackState: " + _attackState);
        }
    }
    private AttackState _attackState = AttackState.None;

    public override void Use(GameThing user)
    {
        base.Use(user);

        ResetAttack();
        attackState = AttackState.PickingTarget;
    }

    private void ResetAttack()
    {
        // Reset all the variables that need to be reset here
        _attackState = AttackState.None;
        _reachableNodes = null;
        targetPosition = Vector3.zero;
        targetDirection = Vector2Int.zero;
        if (attackSequenceThing != null)
        {
            attackSequenceThing.AttackSequenceFinished.RemoveListener(OnAttackSequenceFinished);
            attackSequenceThing = null;
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
            case AttackState.PickingTarget:
                PickTarget(direction: direction);
                break;
            case AttackState.Attacking:
                if (attackSequenceThing != null)
                    attackSequenceThing.Move(direction);
                break;
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
                    (attackSequenceThing.range <= 1)
                        ? user.transform.position + (new Vector3(targetDirection.x, 0, targetDirection.y) * attackSequenceThing.range)
                        : targetPosition + (new Vector3(targetDirection.x, 0, targetDirection.y) * attackSequenceThing.range);

                // Check if the potential target position is within the attack range
                Pathfinding.GraphNode potentialTargetNode = Nodes.instance.gridGraph.GetNearest(potentialTargetPosition).node;
                if (_reachableNodes.Contains(potentialTargetNode))
                {
                    // Color the previous target node
                    if (Nodes.instance != null && targetPosition != Vector3.zero)
                    {
                        Nodes.instance.ColorNodeObject(Nodes.instance.gridGraph.GetNearest(targetPosition).node);
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

    public void PickTarget(Vector3 position)
    {
        targetPosition = position;
    }

    private void OnAttackSequenceFinished(bool allStepsSuccessful)
    {
        // Unsubscribe from the AttackSequenceFinished event to avoid memory leaks
        if (attackSequenceThing != null)
        {
            attackSequenceThing.AttackSequenceFinished.RemoveListener(OnAttackSequenceFinished);
            attackSequenceThing = null;
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
            case AttackState.PickingTarget:
                if (pressed && targetPosition != Vector3.zero)
                    attackState++;
                break;
            case AttackState.Attacking:
                if (attackSequenceThing != null)
                    attackSequenceThing.PrimaryAction(pressed);
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
                if (attackSequenceThing != null)
                    attackSequenceThing.SecondaryAction(pressed);
                break;
        }
    }
}
