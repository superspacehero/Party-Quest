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

            if (inventoryOwner != null)
                _inventory = (inventoryOwner as CharacterThing).actionList;

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

    protected override void PopulateThings()
    {
        base.PopulateThings();

        if (inventory is not ActionList)
            return;

        foreach (ThingDisplay thingDisplay in thingDisplays)
        {
            if (thingDisplay.iconButton != null)
            {
                thingDisplay.iconButton.onClick.AddListener(() => inventory.displayInventory = false);

                thingDisplay.iconButton.interactable = (inventory as ActionList).availableActionCategories.Contains(thingDisplay.thing.thingSubType);
            }
        }
    }
}
