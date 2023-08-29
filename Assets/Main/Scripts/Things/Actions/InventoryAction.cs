using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryAction : ActionThing
{
    public override string thingType => "Inventory";
    public override string thingSubType => "Inventory";

    public override void Use(GameThing user)
    {
        base.Use(user);

        if (user.inventory != null)
            user.inventory.displayInventory = true;
    }

    public override void SecondaryAction(bool pressed)
    {
        if (pressed)
            CancelAction();
    }

    protected override void CancelAction()
    {
        base.CancelAction();

        if (user.inventory != null)
            user.inventory.displayInventory = false;
    }
}
