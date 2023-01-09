using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[AddComponentMenu("Game Things/Character Thing")]
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

    #region Controlling Characters

    private ActionList actionList
    {
        get
        {
            if (_actionList == null)
                TryGetComponent(out _actionList);
            return _actionList;
        }
    }
    private ActionList _actionList;

    public CharacterController characterController
    {
        get
        {
            if (_characterController == null)
                _characterController = GetComponentInChildren<CharacterController>();
            return _characterController;
        }
    }
    private CharacterController _characterController;

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

    public void Move(Vector2 direction)
    {
        if (actionList != null)
            actionList.Move(direction);
    }

    public void PrimaryAction(bool pressed)
    {
        if (actionList != null)
            actionList.PrimaryAction(pressed);
    }

    public void SecondaryAction(bool pressed)
    {
        if (actionList != null)
            actionList.SecondaryAction(pressed);
    }

    #endregion

    // CharacterThings' bodies are made up of CharacterPartThings.
    // We need a way to be able to assemble them, and access the parts.
    // The question is, what's the best way to go about dealing with instantiating the prefabs, and then assembling them?
    // I ask this because, unless we destroy every single part when we replace a part, we'll have to have a way of knowing what instantiated parts to destroy.
    // For now, we'll just do the former, but I'd like to think of a way to do the latter.



    // Well, as it turns out, the character assembler works perfectly.
    // There are some things I need to figure out, though.
    // In particular, how do I handle equipment slots, and connect them with the character's parts?

    // The best thing to do would probably be to build out the equipment inventory after having assembled the character.
    // Because the assembler goes through every character part and every inventory slot they have, maybe we can just take every one of those slots,
    // and if their type matches with a list of compatible equipment types, add them to the equipment inventory.

    // Doing it this way would mean that adding equipment to a character wouldn't require a rebuild of the character in any way,
    // but if for some reason, the character is rebuilt, the equipment would be lost.
    // I guess there's an easy fix - just detach the equipment from the character, and then reattach it after the character is rebuilt.

    public List<GameObject> characterPartPrefabs = new List<GameObject>();
    protected List<CharacterPartThing> parts = new List<CharacterPartThing>(), addedParts = new List<CharacterPartThing>();
    [FoldoutGroup("Variables")] public GameThingVariables baseVariables = new GameThingVariables();

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
            if ((part.thingType == slot.thingType || part.thingSubType == slot.thingType) && !addedParts.Contains(part))
            {
                slot.AddThing(part);
                addedParts.Add(part);
                variables += part.variables;

                part.SetColors();
                part.gameObject.SetActive(true);

                AttachPartsToPart(part);

                break;
            }
        }
    }

    [Button]
    public void AssembleCharacter()
    {
        // Destroy all children of the characterBase first.
        for (int i = characterBase.transform.childCount - 1; i >= 0; i--)
        {
            #if UNITY_EDITOR
                DestroyImmediate(characterBase.transform.GetChild(i).gameObject);
            #else
                Destroy(characterBase.transform.GetChild(i).gameObject);
            #endif
        }

        parts.Clear();

        if (variables.variables != null)
            variables.variables.Clear();
        variables += baseVariables;

        foreach (GameObject characterPartPrefab in characterPartPrefabs)
        {
            if (characterPartPrefab == null)
                continue;

            if (Instantiate(characterPartPrefab, characterBase.transform).TryGetComponent(out CharacterPartThing characterPartThing))
            {
                parts.Add(characterPartThing);
                characterPartThing.gameObject.SetActive(false);
            }
        }

        AttachPartToSlot(characterBase);

        // foreach (GameThingVariables.Variable variable in variables.variables)
        // {
        //     if (variable.value < 0)
        //         variable.value = 0;
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