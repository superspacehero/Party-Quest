using System.Collections.Generic;

public class ChooseAttackAction : ActionThing
{
    public override void Use(GameThing user)
    {
        this.user = user;
        user.actionList.SetAction(this);

        List<string> whitelistedActionCategories = new List<string>() { "Attack" };
        actionList.PopulateActionList(user.GetComponentsInChildren<ActionThing>(true), whitelistedActionCategories);

        if (user is CharacterThing && (user as CharacterThing).input.isPlayer)
            actionList.displayInventory = true;
        else
            GameManager.instance.emptyMenu.Select();

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
