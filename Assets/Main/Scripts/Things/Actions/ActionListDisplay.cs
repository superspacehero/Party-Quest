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
                actionList = (inventoryOwner as CharacterThing).actionList;

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
            {
                if (inventoryOwner != null)
                    thingDisplay.iconButton.onClick.AddListener(() => (inventoryOwner as CharacterThing).DisplayActionList(false));

                thingDisplay.iconButton.interactable = actionList.availableActionCategories.Contains(thingDisplay.thing.thingSubType);
            }
        }
    }
}
