using System.Collections.Generic;
using UnityEngine;

public class ChooseAttackAction : ActionThing
{
    public override void Use(GameThing user)
    {
        this.user = user;
        user.actionList.SetAction(this);

        inventory.Clear();
        inventory.AddThings(
            user.GetComponentsInChildren<WeaponThing>(true),
            user is CharacterThing && (user as CharacterThing).input.isPlayer,
            user, true, null, false, false);

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
