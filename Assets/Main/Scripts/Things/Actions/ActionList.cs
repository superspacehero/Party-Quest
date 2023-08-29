using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Game Things/Inventories/Action List"), DisallowMultipleComponent]
public class ActionList : Inventory
{
    public ActionThing currentAction { get; private set; }

    public List<string> usedActionCategories = new List<string>();

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
        if (!usedActionCategories.Contains(currentAction.thingSubType))
            currentAction.Use(user);
    }

    public void ResetActions()
    {
        usedActionCategories = new List<string>();
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

    public void TertiaryAction(bool pressed)
    {
        if (currentAction != null)
            currentAction.TertiaryAction(pressed);
    }

    public void QuaternaryAction(bool pressed)
    {
        if (currentAction != null)
            currentAction.QuaternaryAction(pressed);
    }

    public void LeftAction(bool pressed)
    {
        if (currentAction != null)
            currentAction.LeftAction(pressed);
    }

    public void RightAction(bool pressed)
    {
        if (currentAction != null)
            currentAction.RightAction(pressed);
    }

    public void PopulateActionList(ActionThing[] actions, List<string> actionWhitelist = null, List<string> actionBlacklist = null)
    {
        if (actions == null)
            return;

        bool canAddAction = true;

        foreach (ActionThing action in actions)
        {
            if (action == null)
                continue;

            canAddAction = true;

            foreach (string usedAction in usedActionCategories)
            {
                if (action.thingType.Contains(usedAction))
                {
                    canAddAction = false;
                    break;
                }
            }

            if (actionWhitelist != null && !actionWhitelist.Contains(action.thingSubType))
                continue;

            if (actionBlacklist != null && actionBlacklist.Contains(action.thingSubType))
                continue;

            if (Contains(action.thingName))
                canAddAction = false;

            if (canAddAction)
                AddThing(action, true, action.transform.parent);
        }
    }
}
