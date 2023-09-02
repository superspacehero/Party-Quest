using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentThing : CharacterPartThing
{
    public override string thingType
    {
        get => "Equipment";
    }

    [Sirenix.OdinInspector.FoldoutGroup("Equipment")]
    public Inventory.ThingSlot elementSlot;
}