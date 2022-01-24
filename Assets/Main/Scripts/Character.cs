using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Character : Thing
{
    [FoldoutGroup("Inventory")]
    public List<Thing> items;

    [FoldoutGroup("Inventory")]
    public Thing leftWeapon, rightWeapon;
    [FoldoutGroup("Inventory")]
    public Thing head, body, leftHand, rightHand, leftFoot, rightFoot;
    
    
}
