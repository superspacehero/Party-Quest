using System.Collections.Generic;
using UnityEngine;

public class ChooseAttackAction : ActionThing
{
    public override void Use(GameThing user)
    {
        this.user = user;
        user.actionList.SetAction(this);

        GameThing originalOwner = inventory.inventoryDisplay.inventoryOwner;

        inventory.inventoryDisplay.inventoryOwner = user;
        inventory.Clear();
        inventory.AddThings(
            user.GetComponentsInChildren<WeaponThing>(true),
            user is CharacterThing && (user as CharacterThing).input.isPlayer,
            true, null, false, false);
        // inventory.inventoryDisplay.inventoryOwner = originalOwner;


        gameObject.SetActive(true);
    }

    public override void SecondaryAction(bool pressed)
    {
        if (pressed)
            CancelAction();
    }

    // protected override void CancelAction()
    // {
    //     actionList.displayInventory = false;

    //     base.CancelAction();
    // }
}
