using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Game Things/Inventories/Action List"), DisallowMultipleComponent]
public class ActionList : Inventory
{
    private ActionThing currentAction;

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
}
