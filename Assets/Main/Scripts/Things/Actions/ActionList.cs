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
            displayInventory = _currentAction == null;
        }
    }
    private ActionThing _currentAction;

    public List<string> availableActionCategories = new List<string>();
    [SerializeField]
    private List<string> defaultActionCategories = new List<string>()
    {
        "Move",
        "Action",
        "End"
    };

    [SerializeField, Space]
    private List<string> actionBlacklist = new List<string>()
    {
        "Attack"
    },
    actionWhitelist = new List<string>();

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

        bool canAddAction = true;

        foreach (ActionThing action in actions)
        {
            if (action == null)
                continue;

            canAddAction = true;

            foreach (string includedAction in actionWhitelist)
            {
                if (action.thingType.Contains(includedAction))
                {
                    canAddAction = true;
                    break;
                }

                canAddAction = false;
            }

            if (!canAddAction)
                continue;

            foreach (string excludedAction in actionBlacklist)
            {
                if (action.thingType.Contains(excludedAction))
                {
                    canAddAction = false;
                    break;
                }
            }

            if (Contains(action.thingName))
                canAddAction = false;

            if (canAddAction)
                AddThing(action, true, action.transform.parent);
        }
    }
}
