using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Actions;

public class Character : GridThing
{
    [FoldoutGroup("Info/Character")]
    public int team;

    [FoldoutGroup("Info/Character")]
    public List<ThingAction.ActionCategory> availableActionCategories;

    [FoldoutGroup("Info/Character"), SerializeField, ReadOnly]
    protected List<ThingAction.ActionCategory> defaultActionCategories = new List<ThingAction.ActionCategory>()
    {
        ThingAction.ActionCategory.Movement,
        ThingAction.ActionCategory.Action
    };

    protected List<ThingAction.ActionCategory> nextTurnActionCategories = new List<ThingAction.ActionCategory>();

    [FoldoutGroup("Info/Character/Inventory")]
    public List<Thing> items;

    [FoldoutGroup("Info/Character/Inventory/Equipment")]
    public Thing leftWeapon, rightWeapon;
    [FoldoutGroup("Info/Character/Inventory/Equipment")]
    public Thing head, body, leftHand, rightHand, leftFoot, rightFoot;

    public override void MyTurn()
    {
        availableActionCategories.Clear();

        defaultActionCategories.ForEach(x => availableActionCategories.Add(x));
        nextTurnActionCategories.ForEach(x => availableActionCategories.Add(x));

        nextTurnActionCategories.Clear();
    }

    public void AddActionCategoryForNextTurn(ThingAction.ActionCategory actionCategory)
    {
        nextTurnActionCategories.Add(actionCategory);
    }
}
