using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Game Things/Inventories/Action List"), DisallowMultipleComponent]
public class ActionList : Inventory
{
    public void Move(Vector2 direction)
    {
        foreach (ThingSlot slot in thingSlots)
        {
            if (slot.thing != null && slot.thing.TryGetComponent(out ActionThing actionThing))
                actionThing.Move(direction);
        }
    }

    public void PrimaryAction(bool pressed)
    {
        foreach (ThingSlot slot in thingSlots)
        {
            if (slot.thing != null && slot.thing.TryGetComponent(out ActionThing actionThing))
                actionThing.PrimaryAction(pressed);
        }
    }

    public void SecondaryAction(bool pressed)
    {
        foreach (ThingSlot slot in thingSlots)
        {
            if (slot.thing != null && slot.thing.TryGetComponent(out ActionThing actionThing))
                actionThing.SecondaryAction(pressed);
        }
    }
}
