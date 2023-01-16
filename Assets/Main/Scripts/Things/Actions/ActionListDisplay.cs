using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionListDisplay : InventoryDisplay
{
    public override Inventory inventory
    {
        get
        {
            if (actionList != null)
                return actionList;

            if (inventoryOwner != null)
                inventoryOwner.TryGetComponent(out actionList);
            else
                actionList = GetComponentInParent<ActionList>();

            return actionList;
        }
        set
        {
            if (value != null && value is ActionList)
                actionList = value as ActionList;
            else
                actionList = null;
        }
    }
    private ActionList actionList;

    protected override void PopulateThings()
    {
        base.PopulateThings();

        if (actionList == null)
            return;

        foreach (ThingDisplay thingDisplay in thingDisplays)
        {
            if (thingDisplay.iconButton != null)
                thingDisplay.iconButton.interactable = actionList.availableActionCategories.Contains(thingDisplay.thing.thingSubType);
        }
    }
}
