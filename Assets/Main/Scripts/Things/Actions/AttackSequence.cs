using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSequenceThing : GameThing
{
    public override string thingType
    {
        get => "AttackSequence";
    }

    [Min(0f)] public float range = 1f;
}