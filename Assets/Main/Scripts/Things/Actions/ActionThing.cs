using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Game Things/Action Thing")]
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

    public override string thingSubType
    {
        get => "Action";
    }

    protected GameThing user;

    public bool actionRunning = false;

    public UnityEngine.Events.UnityEvent onActionEnd;

    public override void Use(GameThing user)
    {
        if (!actionRunning)
        {
            this.user = user;
            actionRunning = true;

            user.actionList.SetAction(this);

            gameObject.SetActive(true);
            StartCoroutine(RunAction());
        }
    }

    protected virtual IEnumerator RunAction()
    {
        // This is the base RunAction() function for ActionThings.
        // It does nothing, and is overridden by subclasses.
        EndAction();

        yield return null;
    }

    [Sirenix.OdinInspector.Button]
    protected void StopActionRunning()
    {
        actionRunning = false;
    }

    protected void EndAction(bool displayInventory = true)
    {
        actionRunning = false;
        onActionEnd.Invoke();

        user.actionList.currentAction = null;

        if (user is CharacterThing && displayInventory)
            (user as CharacterThing).DisplayInventory(true);

        gameObject.SetActive(false);
    }

    protected virtual void CancelAction()
    {
        onActionEnd.RemoveAllListeners();
        StopAllCoroutines();

        actionRunning = false;
        user.actionList.currentAction = null;

        (user as CharacterThing).DisplayInventory(true);
    }
}