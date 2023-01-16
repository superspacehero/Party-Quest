using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionListDisplay : InventoryDisplay
{
    public override Inventory inventory { get => actionList; set => actionList = value as ActionList; }
    [SerializeField] private ActionList actionList;

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
