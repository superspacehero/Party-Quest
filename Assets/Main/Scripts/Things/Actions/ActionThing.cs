using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Game Things/Action Thing")]
public class ActionThing : UnsavedThing
{
    // ActionThings are a type of GameThing that are used to perform actions.
    // They are used for everything from moving, to attacking, to interacting with other GameThings.

    // ActionThings have a start function and an end function.
    // When the start function is called, the ActionThing category will be removed from the GameThing's list of categories.

    protected override void Awake()
    {
    }

    public override string thingType
    {
        get => "Action";
    }

    public override string thingSubType
    {
        get => "Action";
    }

    public GameThing user { get; protected set; }

    // The current node the character is on
    public override Pathfinding.GraphNode currentNode { get => user.currentNode; set => user.currentNode = value; }

    // The position of the character
    public override Vector3 position { get => user.position; set => user.position = value; }

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

    protected virtual void EndAction(bool displayInventory = true)
    {
        actionRunning = false;
        onActionEnd?.Invoke();

        user.actionList.currentAction = null;
        user.actionList.usedActionCategories.Add(thingSubType);

        if (user is CharacterThing && displayInventory)
            (user as CharacterThing).DisplayActions(true);

        gameObject.SetActive(false);
    }

    protected virtual void CancelAction()
    {
        onActionEnd.RemoveAllListeners();
        StopAllCoroutines();

        actionRunning = false;
        user.actionList.currentAction = null;

        (user as CharacterThing).DisplayActions(true);
    }
}