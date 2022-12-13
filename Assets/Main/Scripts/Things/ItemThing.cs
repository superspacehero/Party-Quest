using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Game Things/Item Thing")]
public class ItemThing : GameThing
{
    // Items are GameThings that can be used by a character.
    // If they are on the ground, they will be picked up when used, and added to the inventory of the character that used them.
    // Once in the inventory of a character, their normal Use() function will be called, which is defined by the subclass of Item.

    [HideInInspector] public override string thingType
    {
        get => "Item";
    }

    public override void Use(GameThing user)
    {
        // If the user has an inventory, and the item is on the ground, pick it up.
        if (user.TryGetComponent(out Inventory inventory))
        {
            if (inventory.Contains(this))
                UseItem(user);
            else
                inventory.AddThing(this);
        }
    }

    public virtual void UseItem(GameThing user)
    {
        // This is the base UseItem() function for Items.
        // It does nothing, and is overridden by subclasses.
    }
}