using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionListDisplay : InventoryDisplay
{
    public override Inventory inventory
    {
        get
        {
            if (_inventory != null)
                return _inventory;
            else if (inventoryOwner != null)
            {
                if (_inventoryOwner is CharacterThing)
                    _inventory = (_inventoryOwner as CharacterThing).actionList;
                else
                    _inventory = _inventoryOwner.inventory;

                // Debug.Log($"Using {_inventoryOwner.thingName}'s action list as inventory");
            }

            return _inventory;
        }
        set
        {
            if (value != null && value is ActionList)
                _inventory = value;
            else
                _inventory = null;
        }
    }
    // [SerializeField] private ActionList actionList;

    protected override void PopulateThings(List<GameThing> excludedThings = null)
    {
        // if (inventory is not ActionList)
        //     return;

        if (excludedThings == null)
            excludedThings = new List<GameThing>();

        foreach (Inventory.ThingSlot slot in inventory.thingSlots)
        {
            if ((slot.thing != null && slot.thing is ActionThing) && ((slot.thing as ActionThing).actionRunning || (inventory as ActionList).usedActionCategories.Contains(slot.thing.thingSubType)))
                excludedThings.Add(slot.thing);
        }

        base.PopulateThings(excludedThings);


        // foreach (ThingDisplay thingDisplay in thingDisplays)
        // {
        //     if (thingDisplay.iconButton != null)
        //     {
        //         thingDisplay.iconButton.onClick.AddListener(() => inventory.displayInventory = false);

        //         thingDisplay.iconButton.interactable = !(inventory as ActionList).usedActionCategories.Contains(thingDisplay.thing.thingSubType);
        //     }
        // }
    }
}
