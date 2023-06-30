using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Game Things/Interactions"), DisallowMultipleComponent]
public class Interactions : MonoBehaviour
{
    private GameThing thisThing
    {
        get
        {
            if (_gameThing == null)
            {
                _gameThing = GetComponentInParent<GameThing>();
            }
            return _gameThing;
        }
    }
    private GameThing _gameThing;

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out GameThing otherThing) && collisionInteraction?.canInteract == true)
        {
            collisionInteraction?.Interact(otherThing, thisThing);
        }
    }

    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out GameThing otherThing) && collisionInteraction?.canInteract == true)
        {
            collisionInteraction?.StopInteract(otherThing, thisThing);
        }
    }

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun
    /// touching another rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    void OnCollisionEnter(Collision other)
    {
        if (other.collider.TryGetComponent(out GameThing otherThing) && collisionInteraction?.canInteract == true)
        {
            collisionInteraction?.Interact(otherThing, thisThing);
        }
    }

    [SerializeReference]
    public Interaction collisionInteraction;
}

public abstract class Interaction
{
    public virtual bool canInteract { get => true; }
    public abstract void Interact(GameThing interactor, GameThing interactee);
    public abstract void StopInteract(GameThing interactor, GameThing interactee);
}

public class PickUpInteraction : Interaction
{
    public override void Interact(GameThing interactor, GameThing interactee)
    {
        if (interactor.inventory)
        {
            interactor.inventory.AddThing(interactee);
        }
    }

    public override void StopInteract(GameThing interactor, GameThing interactee)
    {
    }
}

public class InteractionListInteraction : Interaction
{
    public GameObject interactionIndicator;
    public GameObject[] actionObjects;
    public ActionThing[] actions
    {
        get
        {
            if (_actions == null)
            {
                _actions = new ActionThing[actionObjects.Length];

                for (int i = 0; i < actionObjects.Length; i++)
                {
                    _actions[i] = actionObjects[i].GetComponent<ActionThing>();
                }
            }

            return _actions;
        }
    }
    private ActionThing[] _actions = null;

    public override void Interact(GameThing interactor, GameThing interactee)
    {
        interactionIndicator?.SetActive(true);
        interactor.actionList?.PopulateActionList(actions);
    }

    public override void StopInteract(GameThing interactor, GameThing interactee)
    {
        interactionIndicator?.SetActive(false);
        interactor.actionList?.ResetActions();
    }
}