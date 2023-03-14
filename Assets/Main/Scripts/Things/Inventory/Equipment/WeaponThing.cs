using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponThing : EquipmentThing
{
    public override string thingType
    {
        get => "Weapon";
    }

    public AttackSequenceThing sideAttack, upAttack, downAttack;
}