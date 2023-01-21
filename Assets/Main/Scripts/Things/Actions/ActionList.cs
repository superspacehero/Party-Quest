using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Game Things/Inventories/Action List"), DisallowMultipleComponent]
public class ActionList : Inventory
{
    private ActionThing currentAction
    {
        get => _currentAction;
        set
        {
            _currentAction = value;
            displayActionList = _currentAction == null;
        }
    }
    private ActionThing _currentAction;

    // Action list display for character
    private ActionListDisplay actionListDisplay
    {
        get
        {
            if (_actionListDisplay == null)
            {
                Transform parent = transform.parent;
                while (parent != null)
                {
                    if (parent.parent == null)
                        break;
                    else
                        parent = parent.parent;
                }

                if (parent != null)
                    _actionListDisplay = parent.GetComponentsInChildren<ActionListDisplay>(true)[0];
            }
            return _actionListDisplay;
        }
    }
    private ActionListDisplay _actionListDisplay;
    public bool displayActionList
    {
        get => _displayActionList;
        set
        {
            _displayActionList = value;
            if (actionListDisplay != null)
                actionListDisplay.gameObject.SetActive(_displayActionList);
            else
                Debug.LogWarning("No ActionListDisplay found on " + gameObject.name, this);
        }
    }
    private bool _displayActionList;

    public List<string> availableActionCategories = new List<string>();
    [SerializeField] private List<string> defaultActionCategories = new List<string>()
    {
        "Move",
        "Action",
        "End"
    };

    public void SetAction(ActionThing action)
    {
        if (currentAction != null)
            currentAction.actionRunning = false;

        currentAction = action;
    }

    public void ClearAction()
    {
        SetAction(null);
    }

    public void UseAction(GameThing user)
    {
        if (availableActionCategories.Contains(currentAction.thingSubType))
        {
            currentAction.onActionEnd.AddListener(() => availableActionCategories.Remove(currentAction.thingSubType));
            currentAction.Use(user);
        }
    }

    public void ResetActions()
    {
        availableActionCategories = new List<string>(defaultActionCategories);
    }



    public void Move(Vector2 direction)
    {
        if (currentAction != null)
            currentAction.Move(direction);
    }

    public void PrimaryAction(bool pressed)
    {
        if (currentAction != null)
            currentAction.PrimaryAction(pressed);
    }

    public void SecondaryAction(bool pressed)
    {
        if (currentAction != null)
            currentAction.SecondaryAction(pressed);
    }



    public void PopulateActionList(ActionThing[] actions)
    {
        if (actions == null)
            return;

        // string actionDebug = "Populating action list with: ";

        foreach (ActionThing action in actions)
        {
            if (action == null)
                continue;

            AddThing(action, true);
            // actionDebug += action.thingName + (action == actions[actions.Length - 1] ? "" : ", ");
        }

        // Debug.Log(actionDebug, this);
    }
}
