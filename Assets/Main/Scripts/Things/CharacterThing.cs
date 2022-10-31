using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterThing : GameThing
{
    // CharacterThings are a subclass of GameThings that represent characters in the game.

    // CharacterThings have all the properties of GameThings, as well as a list of stats.

    // CharacterThings can be controlled by the player, by AI, or by other characters.

    public override string thingType
    {
        get => "CharacterThing";
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
}