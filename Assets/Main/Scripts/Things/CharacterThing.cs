using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[AddComponentMenu("Game Things/Character Thing")]
public class CharacterThing : GameThing
{
    // CharacterThings are a subclass of GameThings that represent characters in the game.
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        // If the GameManager's level is not null, and it doesn't have this CharacterThing in its list of characters, add this CharacterThing to the list.
        if (GameManager.instance != null && !GameManager.instance.level.characters.Contains(this))
        {
            GameManager.instance.level.characters.Add(this);
        }
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();

        // If the GameManager's level is not null, and it has this CharacterThing in its list of characters, remove this CharacterThing from the list.
        if (GameManager.instance != null && GameManager.instance.level.characters.Contains(this))
        {
            GameManager.instance.level.characters.Remove(this);
        }
    }

    public override string thingType
    {
        get => "Character";
    }

    // The character's input
    public ThingInput input;

    // The character's portrait
    public Sprite thingPortrait;

    // The list of character parts
    public CharacterPartList characterPartList;

    public struct CharacterInfo
    {
        public string name, portrait;
        public int value;
        public Inventory inventory;
        public GameThingVariables baseVariables;
        public List<CharacterPartThing.CharacterPartInfo> characterParts;

        public override string ToString()
        {
            string json = JsonUtility.ToJson(this);
            Debug.Log(json);
            return json;
        }

        public static CharacterInfo FromString(string json)
        {
            return JsonUtility.FromJson<CharacterInfo>(json);
        }

        private static void CheckCharacterDirectory()
        {
            if (!System.IO.Directory.Exists(Application.persistentDataPath + "/Characters/"))
                System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/Characters/");
        }

        public CharacterInfo LoadCharacter(string characterName, string characterCategory = "Player")
        {
            CheckCharacterDirectory();

            if (System.IO.File.Exists(Application.persistentDataPath + $"/Characters/{characterCategory}_{characterName}.json"))
            {
                string json = System.IO.File.ReadAllText(Application.persistentDataPath + $"/Characters/{characterCategory}_{characterName}.json");
                return FromString(json);
            }
            else
            {
                Debug.LogError($"Character {characterName} does not exist");
                return new CharacterInfo();
            }
        }

        public void SaveCharacter(string characterName, string characterCategory = "Player")
        {
            CheckCharacterDirectory();

            string json = ToString();

            System.IO.File.WriteAllText(Application.persistentDataPath + $"/Characters/{characterCategory}_{LoadCharacters(characterCategory).Count + 1}_{characterName}.json", json);
        }

        public static List<CharacterInfo> LoadCharacters(string characterCategory = "Player")
        {
            CheckCharacterDirectory();

            List<CharacterInfo> characters = new List<CharacterInfo>();

            foreach (string characterFile in System.IO.Directory.GetFiles(Application.persistentDataPath + $"/Characters/", $"{characterCategory}_*.json"))
            {
                string json = System.IO.File.ReadAllText(characterFile);
                characters.Add(FromString(json));
            }

            return characters;
        }

        public static void DeleteCharacter(string characterName, string characterCategory = "Player")
        {
            CheckCharacterDirectory();

            System.IO.File.Delete(Application.persistentDataPath + $"/Characters/{characterCategory}_{characterName}.json");
        }

        public CharacterInfo(string name, string portrait, int value, Inventory inventory, GameThingVariables baseVariables, List<CharacterPartThing.CharacterPartInfo> characterParts)
        {
            this.name = name;
            this.portrait = portrait;
            this.value = value;
            this.inventory = inventory;
            this.baseVariables = baseVariables;
            this.characterParts = characterParts;
        }

        public CharacterInfo(string name, Sprite portrait, int value, Inventory inventory, GameThingVariables baseVariables, List<CharacterPartThing.CharacterPartInfo> characterParts)
        {
            this.name = name;
            this.portrait = General.SpriteToString(portrait);
            this.value = value;
            this.inventory = inventory;
            this.baseVariables = baseVariables;
            this.characterParts = characterParts;
        }

        // Operator to check if the character equals another character
        public static bool Equals(CharacterInfo characterInfo1, CharacterInfo characterInfo2)
        {
            return characterInfo1.name == characterInfo2.name &&
                characterInfo1.portrait == characterInfo2.portrait &&
                characterInfo1.value == characterInfo2.value &&
                characterInfo1.inventory == characterInfo2.inventory &&
                characterInfo1.baseVariables.Equals(characterInfo2.baseVariables) &&
                characterInfo1.characterParts == characterInfo2.characterParts;
        }
    }
    public CharacterInfo characterInfo
    {
        get
        {
            return new CharacterInfo()
            {
                name = thingName,
                portrait = General.SpriteToString(thingPortrait),
                value = thingValue,
                inventory = inventory,
                baseVariables = this.baseVariables,
                characterParts = this.characterParts
            };
        }

        set
        {
            thingName = value.name;
            thingPortrait = General.StringToSprite(value.portrait);

            thingValue = value.value;

            AddInventory();

            baseVariables = value.baseVariables;

            characterParts = value.characterParts;
        }
    }

    // Team that the character belongs to
    public int team;

    // The character's Attack Menu
    public AttackMenu attackMenu
    {
        get
        {
            if (_attackMenu == null)
                _attackMenu = GetComponentInChildren<AttackMenu>(true);
            return _attackMenu;
        }
    }
    private AttackMenu _attackMenu;

    #region Controlling Characters

    // The action to run at the start of the character's turn
    public StartingActionThing overrideStartingAction;

    public void MyTurn()
    {
        if (overrideStartingAction != null)
        {
            overrideStartingAction.Use(this);
            return;
        }

        if (actionList != null)
        {
            actionList.PopulateActionList(GetComponentsInChildren<ActionThing>());
            actionList.ResetActions();
        }
        else
            Debug.LogWarning("No action list found for character " + thingName);

        DisplayActions(true);
    }

    public void DisplayActions(bool display, ActionList actionListToUse = null)
    {
        if (actionListToUse == null)
            actionListToUse = actionList;

        if (actionListToUse != null)
            actionListToUse.displayInventory = display;
    }

    // Movement controller for moving the character
    public MovementController movementController
    {
        get
        {
            if (_characterController == null)
                _characterController = GetComponentInChildren<MovementController>();
            return _characterController;
        }
    }
    private MovementController _characterController;

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
    public override void Move(Vector2 direction)
    {
        if (actionList != null)
            actionList.Move(direction);

        if (movementController != null && movementController.canControl > 1)
            movementController.movementInput = direction;

        base.Move(direction);
    }

    // Method to perform primary action on character
    public override void PrimaryAction(bool pressed)
    {
        if (!(movementController.canControl > 0 && interaction != null && interaction.PrimaryAction != null && interaction.canInteract))
        {
            if (actionList != null)
                actionList.PrimaryAction(pressed);

            if (movementController != null && movementController.canControl > 1)
                movementController.jumpInput = pressed;
        }

        base.PrimaryAction(pressed);
    }

    // Method to perform secondary action on character
    public override void SecondaryAction(bool pressed)
    {
        if (actionList != null)
            actionList.SecondaryAction(pressed);

        base.SecondaryAction(pressed);
    }

    // Method to pause the game
    public void Pause()
    {

    }

    #endregion

    #region Character Assembly

    // List of character part prefabs used to assemble the character
    [UnityEngine.Serialization.FormerlySerializedAs("characterPartPrefabs")]
    public List<CharacterPartThing.CharacterPartInfo> characterParts = new List<CharacterPartThing.CharacterPartInfo>();
    // List of character parts that make up the character
    public List<CharacterPartThing> parts = new List<CharacterPartThing>();
    protected List<CharacterPartThing> addedParts = new List<CharacterPartThing>();
    [FoldoutGroup("Variables")] public GameThingVariables baseVariables = new GameThingVariables();

    // Base inventory slot for character
    [SerializeField] protected Inventory.ThingSlot characterBase;

    // Method to get all the character part slots, including the base slot
    public List<Inventory.ThingSlot> GetCharacterPartSlots()
    {
        List<Inventory.ThingSlot> slots = new List<Inventory.ThingSlot>();

        slots.Add(characterBase);

        foreach (CharacterPartThing part in parts)
        {
            if (part.TryGetComponent(out Inventory inventory))
            {
                foreach (Inventory.ThingSlot slot in inventory.thingSlots)
                {
                    slots.Add(slot);
                }
            }
        }

        return slots;
    }

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
                if (movementController != null)
                    part.SetAnimationClips(movementController.anim);

                AttachPartsToPart(part);

                break;
            }
        }
    }

    // Method to assemble the character
    [Button]
    public void AssembleCharacter()
    {
        gameObject.name = thingName;

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

        movementController.ResetAnimator();

        foreach (CharacterPartThing.CharacterPartInfo characterPart in characterParts)
        {
            if (string.IsNullOrEmpty(characterPart.prefabName))
                continue;

            // Debug.Log("Instantiating " + characterPart.prefabName);

            if (CharacterPartThing.Instantiate(out CharacterPartThing characterPartThing, characterPartList, characterPart, characterBase.transform))
            {
                parts.Add(characterPartThing);

                if (characterPartThing.TryGetComponent(out CapsuleCollider capsuleCollider) && TryGetComponent(out CMF.Mover mover))
                {
                    capsuleCollider.enabled = false;
                    mover.SetColliderThickness(capsuleCollider.radius * 2);
                    mover.SetColliderHeight(capsuleCollider.height);
                }

                characterPartThing.gameObject.SetActive(false);
            }
        }

        AttachPartToSlot(characterBase);

        // Reset the animator
        if (TryGetComponent(out Animator animator))
        {
            RuntimeAnimatorController animatorController = animator.runtimeAnimatorController;
            animator.runtimeAnimatorController = null;
            animator.runtimeAnimatorController = animatorController;
        }
    }

    // Method to convert the character to a JSON string
    public override string ToString()
    {
        return characterInfo.ToString();
    }

    // Method to save the character to a file
    [Button]
    public void SaveCharacter(string characterCategory = "Player")
    {
        characterInfo.SaveCharacter(thingName, characterCategory);
    }

    // Method to create the character from a JSON string
    public void FromString(string characterSaveData)
    {
        characterInfo = JsonUtility.FromJson<CharacterInfo>(characterSaveData);

        AssembleCharacter();
    }

    // Method to load the character from a file
    [Button]
    public void LoadCharacter(string characterName, string characterCategory = "Player")
    {
        characterInfo = characterInfo.LoadCharacter(characterName, characterCategory);

        AssembleCharacter();
    }

    #endregion

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

        General.DelayedFunctionFrames(this, () => Nodes.OccupyNode(transform.position));
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        GameManager.RemoveCharacter(this);
        Nodes.UnoccupyNode(transform.position);
    }
}