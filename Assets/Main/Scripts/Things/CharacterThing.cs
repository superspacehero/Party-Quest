using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

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

    // CharacterThings' bodies are made up of CharacterPartThings.
    // We need a way to be able to assemble them, and access the parts.
    // The question is, what's the best way to go about dealing with instantiating the prefabs, and then assembling them?
    // I ask this because, unless we destroy every single part when we replace a part, we'll have to have a way of knowing what instantiated parts to destroy.
    // For now, we'll just do the former, but I'd like to think of a way to do the latter.

    public List<GameObject> characterPartPrefabs = new List<GameObject>();
    protected List<CharacterPartThing> parts = new List<CharacterPartThing>(), addedParts = new List<CharacterPartThing>();

    [SerializeField] protected Inventory.ThingSlot characterBase;

    void AttachPartsToPart(CharacterPartThing part)
    {
        if (part.TryGetComponent(out Inventory inventory))
        {
            foreach (Inventory.ThingSlot slot in inventory.thingSlots)
            {
                AttachPartToSlot(slot);
            }
        }
    }

    void AttachPartToSlot(Inventory.ThingSlot slot)
    {
        foreach (CharacterPartThing part in parts)
        {
            if (part.thingType == slot.thingType && !addedParts.Contains(part))
            {
                slot.AddThing(part);
                addedParts.Add(part);

                AttachPartsToPart(part);

                break;
            }
        }
    }

    [Button]
    public void AssembleCharacter()
    {
        foreach (CharacterPartThing part in parts)
        {
            if (part != null && part.thingType == characterBase.thingType)
            {
                DestroyImmediate(part.gameObject);
                break;
            }
        }
        parts.Clear();

        foreach (GameObject characterPartPrefab in characterPartPrefabs)
        {
            if (Instantiate(characterPartPrefab, characterBase.transform).TryGetComponent(out CharacterPartThing characterPartThing))
                parts.Add(characterPartThing);
        }
        
        AttachPartToSlot(characterBase);

        // foreach (CharacterPartThing part in parts)
        // {
        //     if (!addedParts.Contains(part))
        //     {
        //         AttachPartsToPart(part);
        //     }
        // }
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        AssembleCharacter();
    }
}