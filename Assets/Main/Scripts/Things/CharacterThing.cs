using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterThing : GameThing
{
    // CharacterThings are a subclass of GameThings that represent characters in the game.

    // CharacterThings have all the properties of GameThings, as well as a list of stats.

    // CharacterThings are assembled from CharacterPartThings, which are attached to them.

    // CharacterThings can be controlled by the player, by AI, or by other characters.

    public override string thingType
    {
        get => "Character";
    }

    public override void Use(GameThing user)
    {
        // Attach this CharacterThing to the user.
        // This is how characters are controlled.
        
        if (user != null)
        {
            if (user.GetAttachedThing() == this)
                user.DetachThing();
            else
                user.AttachThing(this);
        }
    }

    protected List<CharacterPartThing> parts = new List<CharacterPartThing>(), addedParts = new List<CharacterPartThing>(), removedParts = new List<CharacterPartThing>();

    protected Inventory.ThingSlot characterBase;

    public void AssembleCharacter()
    {
        // Assemble this CharacterThing from its CharacterPartThings.
        // The solution I'm thinking of for this is to fetch a list of all the CharacterParts a character has,
        // and assemble them together based on their inventory slots.
        // To do this, I guess I need to have a base inventory slot to start the process with,
        // and then from there, just go through all the parts and assemble them into the slots that they fit into.
        
        foreach (CharacterPartThing part in parts)
        {
            if (part.thingType == characterBase.thingType)
            {
                characterBase.AddThing(part);
                addedParts.Add(part);
                break;
            }
        }
        
        // From here, I guess the thing to do is to iterate through all the character parts
        // and if they have an inventory, iterate through all the slots in that inventory.
        // With each slot, I guess we then can do another iteration through all the parts
        // and if the part has a thing type that matches the slot's thing type,
        // then we can add the part to the slot.

        foreach (CharacterPartThing part in parts)
        {
            if (part.TryGetComponent(out Inventory inventory))
            {
                foreach (Inventory.ThingSlot slot in inventory.thingSlots)
                {
                    foreach (CharacterPartThing thingSlotPart in parts)
                    {
                        if (thingSlotPart.thingType == slot.thingType && !addedParts.Contains(thingSlotPart))
                        {
                            slot.AddThing(thingSlotPart);
                            addedParts.Add(thingSlotPart);
                        }
                    }
                }
            }
        }
    }
}