using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Character : GridThing
{
    [FoldoutGroup("Info")]
    public int team;

    [FoldoutGroup("Info/Inventory")]
    public List<Thing> items;

    [FoldoutGroup("Info/Inventory/Equipment")]
    public Thing leftWeapon, rightWeapon;
    [FoldoutGroup("Info/Inventory/Equipment")]
    public Thing head, body, leftHand, rightHand, leftFoot, rightFoot;

    protected override void OnEnable()
    {
        base.OnEnable();
        
        GameManager.AddCharacter(this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        GameManager.RemoveCharacter(this);
    }
}
