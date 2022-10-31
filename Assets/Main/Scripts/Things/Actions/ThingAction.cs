using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;

namespace Actions
{
    [System.Serializable]
    public class ThingAction
    {
        // This is the base class for all actions.
        // It's used as the basis for all actions, which includes:
        // Walking, item consumption, attacking, building, searching, etc.

        // We need to have a means of starting an action, and a means of ending an action.
        // Ending an action is necessary for things like walking, as a player may not walk the entirety of their movement spaces
        // before they decide to perform a different action, or end their turn.

        // We also need to have a means of categorizing actions, so that we can determine whether an action category is available to perform.
        // We also need to have a means of determining whether an action is available to perform. This would still add it to action lists,
        // but would not allow it to be performed.

        public LocalizedString actionName, actionDescription;

        public bool isAvailable = true;

        public enum ActionCategory
        {
            Movement,
            Action
        }

        public virtual void StartAction()
        {
            // This is the function that is called when an action is started.
            // This is where we would do things like set up the action's UI, or set up the action's variables.
            // This is also where we would set up the action's animation.
        }

        public virtual void EndAction()
        {
            // This is the function that is called when an action is ended.
            // This is where we would do things like clean up the action's UI, or clean up the action's variables.
            // This is also where we would clean up the action's animation.
        }
    }
}
