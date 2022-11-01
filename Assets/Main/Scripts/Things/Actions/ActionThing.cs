using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionThing : MonoBehaviour
{
    // GameThings are a base class for all interactables and collectibles in the game.
    // They have a name, a description, an icon, and a value.
    // Their value is primarily used for determining how much they are worth when sold,
    // but can also be used for other things, such as the number a die lands on, damage/armor/healing stats, etc.

    // They also have a thingType, which is a string used to determine what type of thing they are,
    // and an attachedThing, which is a reference to another GameThing that is attached to this one.
    // This can be used to control an attached GameThing, or to pass information between two GameThings.

    // At their core, their primary function is to be used,
    // which is handled by the Use() function.

    // This function has a variety of uses, depending on the GameThing subclass, which include:
    // - Item GameThings on the floor, which lets them be picked up by the user;
    // - Consumable item GameThings, which leaves behind one, some, or no other GameThings on use, depending on the item;
    // - Equipment item GameThings, which can be equipped and unequipped, and which change stats on the character equipping them;
    // - Character GameThings, which, when used by another character, allow the user to take control of the target character
    //   (used for player characters, NPCs, and objects that can be directly controlled);
    // - Mechanism Trigger and Mechanism GameThings, the former of which can be used to toggle, activate, or deactivate the latter
    //   (used for doors, switches, and other objects that can be interacted with in a variety of ways);
    
    public string thingName;
    public string thingDescription;
    public Sprite thingIcon;
    public int thingValue;
    [SerializeField] protected Inventory.ThingSlot attachedThing;

    public virtual string thingType
    {
        get => "GameThing";
    }

    public virtual void Use(GameThing user)
    {
        // This is the base Use() function for GameThings.
        // It does nothing, and is overridden by subclasses.
    }

    public void AttachThing(GameThing thing)
    {
        attachedThing.AddThing(thing);
    }

    public void DetachThing()
    {
        attachedThing.RemoveThing();
    }

    public GameThing GetAttachedThing()
    {
        return attachedThing.thing;
    }
}