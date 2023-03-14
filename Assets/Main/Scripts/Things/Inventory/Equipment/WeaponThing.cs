using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponThing : EquipmentThing
{
    public override string thingType
    {
        get => "Weapon";
    }

    [Min(0f)] public float range = 1f;
}