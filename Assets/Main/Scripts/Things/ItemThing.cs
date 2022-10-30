using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemThing : GameThing
{
    // Items are GameThings that can be used by a character.
    // If they are on the ground, they will be picked up when used, and added to the inventory of the character that used them.
    // Once in the inventory of a character, their normal Use() function will be called, which is defined by the subclass of Item.

    public override void Use(GameThing user)
    {
        // If the user is a character, and the item is on the ground, pick it up.
        if (user is not CharacterThing)
            return;

        CharacterThing userCharacter = (CharacterThing)user;

        if (userCharacter.inventory.Contains(this))
            UseItem(userCharacter);
        else
            userCharacter.AddToInventory(this);
    }

    public virtual void UseItem(CharacterThing user)
    {
        // This is the base UseItem() function for Items.
        // It does nothing, and is overridden by subclasses.
    }
}