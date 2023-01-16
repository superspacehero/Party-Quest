using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class EndAction : ActionThing
{
    public override string thingSubType
    {
        get => "End";
    }

    protected override IEnumerator RunAction(GameThing user)
    {
        GameManager.NextCharacter();

        // The action is no longer running
        EndAction();

        yield return null;
    }
}
