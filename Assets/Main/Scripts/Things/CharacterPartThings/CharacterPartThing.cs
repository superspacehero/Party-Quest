using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPartThing : GameThing
{
    // CharacterThings are a subclass of GameThings that represent characters in the game.

    // CharacterThings have all the properties of GameThings, as well as a list of stats.

    // CharacterThings can be controlled by the player, by AI, or by other characters.

    public override string thingType
    {
        get => "CharacterPart";
    }
}