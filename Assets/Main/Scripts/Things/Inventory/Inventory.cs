using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Inventories are an array of GameThings that can be used by a GameThing.
    // They are used by GameThings and CharacterThings to store items,
    // by ItemThings to store the items they create when used,
    // and by MechanismThings to store the items they create when triggered.

    // Inventories hold items in an array of ThingSlots,
    // which are a struct that holds a GameThing, a Transform for where to put it,
    // a bool for whether or not to disable it when it is in the inventory,
    // and an optional type of GameThing that is only allowed to be in the slot (used for things like equipment slots).

    // One boolean is included that allows or prevents quick adding of items to the inventory (such as from the ground).
    // and another is included that returns whether or not the inventory is full.
    // When adding an item to an inventory, the item is added to the first available slot,
    // unless a specific slot is specified, in which case the item is added to that slot.

    [System.Serializable]
    public struct ThingSlot
    {
        public GameThing thing;
        public Transform transform;
        public bool disableWhenInInventory;

        public string thingType;

        public void AddThing(GameThing thingToAdd)
        {
            if (thingToAdd == null || thingType != "" && thingType != thingToAdd.thingType)
            {
                Debug.Log("Can't add thing to slot.", thingToAdd);
                return;
            }

            thing = thingToAdd;

            thingToAdd.transform.parent = transform;
            thingToAdd.transform.localPosition = Vector3.zero;
            thingToAdd.transform.localRotation = Quaternion.identity;
            thingToAdd.transform.localScale = Vector3.one;

            thingToAdd.gameObject.SetActive(!disableWhenInInventory);
        }

        public void RemoveThing()
        {
            if (thing == null)
                return;

            thing.transform.parent = null;
            thing.gameObject.SetActive(true);
            thing = null;
        }
    }

    public ThingSlot[] thingSlots;
    [SerializeField] private Transform inventoryTransform;
    public bool canQuickAddItems = true;

    public void AddThing(GameThing item, ThingSlot slot)
    {
        if (Contains(item, out ThingSlot? thingSlot))
        {
            if (thingSlot != null)
                thingSlot.Value.RemoveThing();
        }

        slot.AddThing(item);
    }

    public void AddThing(GameThing item)
    {
        foreach (ThingSlot slot in thingSlots)
        {
            if (slot.thing == null && (slot.thingType == "" || slot.thingType == item.thingType))
            {
                AddThing(item, slot);
                return;
            }
        }
    }

    public void RemoveThing(ThingSlot slot)
    {
        slot.RemoveThing();
    }

    public void RemoveThing(GameThing item)
    {
        foreach (ThingSlot slot in thingSlots)
        {
            if (slot.thing == item)
            {
                slot.RemoveThing();
                return;
            }
        }
    }

    public bool Contains(GameThing item)
    {
        foreach (ThingSlot slot in thingSlots)
        {
            if (slot.thing == item)
                return true;
        }
        
        return false;
    }

    public bool Contains(GameThing item, out ThingSlot? outSlot)
    {
        foreach (ThingSlot slot in thingSlots)
        {
            if (slot.thing == item)
            {
                outSlot = slot;
                return true;
            }
        }
        
        outSlot = null;
        return false;
    }

    public bool inventoryFull
    {
        get
        {
            foreach (ThingSlot slot in thingSlots)
            {
                if (slot.thing == null)
                    return false;
            }

            return true;
        }
    }
}