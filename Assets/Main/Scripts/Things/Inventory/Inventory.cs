using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Game Things/Inventories/Inventory"), System.Serializable, DisallowMultipleComponent]
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

        public void AddThing(GameThing thingToAdd, bool setTransform = true)
        {
            if (thingToAdd == null || thingType != "" && (thingType != thingToAdd.thingType && thingType != thingToAdd.thingSubType))
            {
                Debug.Log("Can't add thing to slot.", thingToAdd);
                return;
            }

            thing = thingToAdd;

            if (setTransform && transform != null)
            {
                SetParentTransform(thingToAdd, transform);
            }

            thingToAdd.gameObject.SetActive(!disableWhenInInventory);
        }

        public void RemoveThing(Transform newParent = null)
        {
            if (thing == null)
                return;

            if (newParent == null)
                newParent = thing.transform.parent;  // Keep the original parent if none is provided

            RemoveParentTransform(thing, newParent);
            thing.gameObject.SetActive(true);
            thing = null;
        }

        private void SetParentTransform(GameThing thing, Transform parent)
        {
            thing.transform.parent = parent;
            thing.transform.localPosition = Vector3.zero;
            thing.transform.localRotation = Quaternion.identity;
            thing.transform.localScale = Vector3.one;
        }

        private void RemoveParentTransform(GameThing thing, Transform newParent = null)
        {
            thing.transform.parent = newParent;
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

    public void AddThing(GameThing item, bool addSlot = false, Transform slotTransform = null, bool disableWhenInInventory = true, bool setParent = true)
    {
        if (addSlot)
        {
            ThingSlot[] newThingSlots = new ThingSlot[thingSlots.Length + 1];
            for (int i = 0; i < thingSlots.Length; i++)
            {
                newThingSlots[i] = thingSlots[i];
            }

            if (item != null)
            {
                newThingSlots[thingSlots.Length] = new ThingSlot();

                if (slotTransform == null)
                    slotTransform = inventoryTransform;
                newThingSlots[thingSlots.Length].transform = slotTransform;

                newThingSlots[thingSlots.Length].disableWhenInInventory = disableWhenInInventory;
                newThingSlots[thingSlots.Length].thingType = (item.thingSubType != "" ? item.thingSubType : item.thingType);
                newThingSlots[thingSlots.Length].AddThing(item, setParent);  // Passing the setParent argument here

                thingSlots = newThingSlots;
            }
        }
        else
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
    }


    public void AddThings(GameThing[] items, bool displayInventory, GameThing owner, bool addSlot = false, Transform slotTransform = null, bool disableWhenInInventory = true, bool setParent = true)
    {
        foreach (GameThing item in items)
        {
            AddThing(item, addSlot, slotTransform, disableWhenInInventory, setParent);
        }

        if (displayInventory)
        {
            // this.displayInventory = true;
            inventoryDisplay.Select(owner);
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

    public void Clear()
    {
        foreach (ThingSlot slot in thingSlots)
        {
            RemoveThing(slot);
        }

        thingSlots = new ThingSlot[0];
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

    public bool Contains(string thingName)
    {
        foreach (ThingSlot slot in thingSlots)
        {
            if (slot.thing != null && slot.thing.thingName == thingName)
                return true;
        }

        return false;
    }

    public List<GameThing> ContainsVariable(string variableName)
    {
        List<GameThing> things = new List<GameThing>();

        foreach (ThingSlot slot in thingSlots)
        {
            if (slot.thing != null && slot.thing.variables.GetVariable(variableName) > 0)
                things.Add(slot.thing);
        }

        return things;
    }

    public List<ThingSlot> GetSlotsByThingType(string thingType)
    {
        List<ThingSlot> slots = new List<ThingSlot>();

        foreach (ThingSlot slot in thingSlots)
        {
            if (slot.thingType == thingType)
            {
                slots.Add(slot);
            }
        }

        return slots;
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

    // Displaying the inventory in-game
    public InventoryDisplay inventoryDisplay
    {
        get
        {
            if (_inventoryDisplay == null)
            {
                Transform parent = transform.parent;

                if (parent == null)
                    parent = transform;
                else
                {
                    while (parent.parent != null)
                    {
                        parent = parent.parent;
                    }
                }

                if (parent != null)
                {
                    foreach (var display in parent.GetComponentsInChildren<InventoryDisplay>(true))
                    {
                        if (display is InventoryDisplay)
                        {
                            _inventoryDisplay = display;
                            break;
                        }
                    }

                }
            }
            return _inventoryDisplay;
        }
    }
    [SerializeField] private InventoryDisplay _inventoryDisplay;
    public bool displayInventory
    {
        get => _displayInventory;
        set
        {
            _displayInventory = value;
            if (inventoryDisplay != null)
            {
                if (_displayInventory)
                    inventoryDisplay.Select(null);
                else
                    inventoryDisplay.Deselect();

                // Debug.Log("Displaying inventory: " + _displayInventory, inventoryDisplay);
            }
            else
                Debug.LogWarning("No InventoryDisplay found on " + gameObject.name, this);
        }
    }
    private bool _displayInventory;
}