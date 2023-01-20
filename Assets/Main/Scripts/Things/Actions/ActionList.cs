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
                _actionListDisplay = GetComponentInChildren<ActionListDisplay>();
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




    public void PopulateInventorySlots()
    {
        PopulateInventorySlots(defaultActionCategories);
    }

    public void PopulateActionList(ActionThing[] actions)
    {
        if (actions == null)
            return;

        foreach (ActionThing action in actions)
        {
            if (action != null)
                AddThing(action, true);
        }
    }
}
