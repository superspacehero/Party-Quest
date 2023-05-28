using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Game Things/Action Thing")]
public class StartingActionThing : ActionThing
{
    // StartingActionThings are a type of ActionThing that are used at the start of a turn.

    public override string thingType
    {
        get => "StartingAction";
    }
}