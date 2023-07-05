using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Pathfinding;
using Sirenix.OdinInspector;

public class ThingInput : GameThing
{
    public bool isPlayer = true;

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
                return;

            if (value)
            {
                if (attachedThing.thing && attachedThing.thing is CharacterThing)
                    _actionCoroutine = StartCoroutine(ActionCoroutine(attachedThing.thing as CharacterThing));
            }
            else
            {
                if (_actionCoroutine != null)
                {
                    StopCoroutine(_actionCoroutine);
                    _actionCoroutine = null;
                }
            }
        }
    }
    private bool _canControl = false;

    private IEnumerator ActionCoroutine(CharacterThing thing)
    {
        _currentTargetIndex = 0;
        List<GraphNode> path = Nodes.GetPathToNode(thing.transform.position, _targets[_currentTargetIndex].transform.position);

        while (_canControl)
        {
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
                        path = Nodes.GetPathToNode(thing.transform.position, _targets[_currentTargetIndex].transform.position);
                }
            }
            else
            {
                // We've reached the end target
                movement = Vector2.zero;
                primaryAction = false;
                secondaryAction = false;
                _canControl = false;
            }

            yield return null;
        }
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

            if (attachedThing.thing)
                attachedThing.thing.Move(_movement);
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
            
            if (attachedThing.thing)
                attachedThing.thing.PrimaryAction(_primaryAction);
        }
    }
    private bool _primaryAction;

    public bool secondaryAction
    {
        get => _secondaryAction;
        set
        {
            _secondaryAction = value;
            
            if (attachedThing.thing)
                attachedThing.thing.SecondaryAction(_secondaryAction);
        }
    }
    private bool _secondaryAction;

    public bool pause
    {
        get => _pause;
        set
        {
            _pause = value;
            
            if (attachedThing.thing && attachedThing.thing is CharacterThing)
                (attachedThing.thing as CharacterThing).Pause();
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

    public void OnPause(InputValue value)
    {
        pause = value.isPressed;
    }
}
