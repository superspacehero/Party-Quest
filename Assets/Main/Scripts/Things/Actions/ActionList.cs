using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Game Things/Action List")]
public class ActionList : GameThing
{
    public override string thingType
    {
        get => "ActionList";
    }

    [SerializeField] private Inventory actionInventory;

    public void PopulateInventorySlots(List<string> actionTypes)
    {
        actionInventory.PopulateInventorySlots(actionTypes);
    }

    public void UseAction(int actionIndex)
    {
        // Use the action in the specified inventory slot.
        if (actionIndex >= 0 && actionIndex < actionInventory.thingSlots.Length)
        {
            Inventory.ThingSlot actionSlot = actionInventory.thingSlots[actionIndex];
            actionSlot.thing.Use(this);
        }
    }
}
