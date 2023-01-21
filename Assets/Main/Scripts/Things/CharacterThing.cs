using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[AddComponentMenu("Game Things/Character Thing")]
public class CharacterThing : GameThing
{
    // CharacterThings are a subclass of GameThings that represent characters in the game.
    public override string thingType
    {
        get => "Character";
    }

    // Team that the character belongs to
    public int team;

    #region Controlling Characters
    // Action list for character
    public ActionList actionList
    {
        get
        {
            if (_actionList == null)
                _actionList = GetComponentInChildren<ActionList>();
            return _actionList;
        }
    }
    private ActionList _actionList;

    public void DisplayActionList(bool display)
    {
        if (actionList != null)
            actionList.displayActionList = display;
    }

    // Character controller for character
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

    // Method to attach or detach this character thing to a user
    public override void Use(GameThing user)
    {
        if (user != null)
        {
            if (user.GetAttachedThing() == this)
                user.DetachThing();
            else
                user.AttachThing(this);
        }
    }

    // Method to move character
    public void Move(Vector2 direction)
    {
        if (actionList != null)
            actionList.Move(direction);
    }

    // Method to perform primary action on character
    public void PrimaryAction(bool pressed)
    {
        if (actionList != null)
            actionList.PrimaryAction(pressed);
    }

    // Method to perform secondary action on character
    public void SecondaryAction(bool pressed)
    {
        if (actionList != null)
            actionList.SecondaryAction(pressed);
    }
    #endregion

    // List of character part prefabs used to assemble the character
    public List<GameObject> characterPartPrefabs = new List<GameObject>();
    // List of character parts that make up the character
    protected List<CharacterPartThing> parts = new List<CharacterPartThing>(), addedParts = new List<CharacterPartThing>();
    [FoldoutGroup("Variables")] public GameThingVariables baseVariables = new GameThingVariables();

    // Base inventory slot for character
    [SerializeField] protected Inventory.ThingSlot characterBase;

    // Method to attach parts to a character part
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

    // Method to attach a character part to a slot
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
                part.isBackPart = slot.transform.name.Contains("Back");
                part.CheckIsBackPart();

                AttachPartsToPart(part);

                break;
            }
        }
    }

    [Button]
    // Method to assemble the character
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

        if (actionList != null)
            actionList.PopulateActionList(GetComponentsInChildren<ActionThing>());

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
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        AssembleCharacter();
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        GameManager.AddCharacter(this);
    }
    
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        GameManager.RemoveCharacter(this);
    }
}