using System.Collections.Generic;

public class ChooseAttackAction : ActionThing
{
    public override void Use(GameThing user)
    {
        this.user = user;
        user.actionList.SetAction(this);

        List<string> whitelistedActionCategories = new List<string>() { "Attack" };
        actionList.PopulateActionList(user.GetComponentsInChildren<ActionThing>(true), whitelistedActionCategories);
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
