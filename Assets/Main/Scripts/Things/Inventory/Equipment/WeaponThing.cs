using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponThing : EquipmentThing
{
    public override string thingType
    {
        get => "Weapon";
    }

    [Sirenix.OdinInspector.FoldoutGroup("Attacks")]
    public AttackSequenceThing sideAttack, upAttack, downAttack;
}