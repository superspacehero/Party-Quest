using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterThing : GameThing
{
    // CharacterThings are a subclass of GameThings that represent characters in the game.
    // CharacterThings have all the properties of GameThings, as well as a list of stats,
    // a list of equipment slots, and an inventory.

    // CharacterThings can be controlled by the player, by AI, or by other characters.

    public List<GameThing> inventory;
    [SerializeField] private Transform inventoryTransform;
    
    public void AddToInventory(GameThing item)
    {
        inventory.Add(item);
        item.gameObject.SetActive(false);
        item.transform.parent = inventoryTransform;
    }

    public void RemoveFromInventory(GameThing item)
    {
        inventory.Remove(item);
        item.gameObject.SetActive(true);
        item.transform.parent = null;
    }
}