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
        inventory.AddThings(user.GetComponentsInChildren<WeaponThing>(true), true, null, false, false);
        if (user is CharacterThing && (user as CharacterThing).input.isPlayer)
        {
            inventory.displayInventory = true;
            Debug.Log("Player is choosing attack");
        }
        else
            Debug.Log("AI is choosing attack");
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
