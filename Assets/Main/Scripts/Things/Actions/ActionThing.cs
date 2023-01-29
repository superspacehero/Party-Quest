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

    public virtual void Move(Vector2 direction)
    {
        // This is the base Move() function for ActionThings.
        // It does nothing, and is overridden by subclasses.
    }

    public virtual void PrimaryAction(bool pressed)
    {
        // This is the base PrimaryAction() function for ActionThings.
        // It does nothing, and is overridden by subclasses.
    }

    public virtual void SecondaryAction(bool pressed)
    {
        // This is the base SecondaryAction() function for ActionThings.
        // It does nothing, and is overridden by subclasses.
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

    protected void EndAction()
    {
        actionRunning = false;
        onActionEnd.Invoke();

        gameObject.SetActive(false);
    }

    protected virtual void CancelAction()
    {
        onActionEnd.RemoveAllListeners();
        StopAllCoroutines();

        actionRunning = false;
    }
}