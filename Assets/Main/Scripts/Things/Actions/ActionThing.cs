using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionThing : GameThing
{
    // ActionThings are a type of GameThing that are used to perform actions.
    // They are used for everything from moving, to attacking, to interacting with other GameThings.

    // ActionThings have a start function and an end function.
    // When the start function is called, the ActionThing category will be removed from the GameThing's list of categories.

    public override string thingType
    {
        get => "Action";
    }

    protected bool actionRunning = false;

    public override void Use(GameThing user)
    {
        if (!actionRunning)
        {
            actionRunning = true;
            StartCoroutine(RunAction(user));
        }
    }

    protected virtual IEnumerator RunAction(GameThing user)
    {
        // This is the base StartAction() function for ActionThings.
        // It does nothing, and is overridden by subclasses.
        yield return null;
    }
}