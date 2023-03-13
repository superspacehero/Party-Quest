using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class ChooseAttackAction : ActionThing
{
    public override void Use(GameThing user)
    {
        this.user = user;
        user.actionList.SetAction(this);

        actionList.PopulateActionList(user.GetComponentsInChildren<ActionThing>(true));
        actionList.ResetActions();
        actionList.displayInventory = true;
        
        gameObject.SetActive(true);
    }

    public override void SecondaryAction(bool pressed)
    {
        if (pressed)
            CancelAction();
    }

    protected override void CancelAction()
    {
        base.CancelAction();

        actionList.displayInventory = false;
    }
}
