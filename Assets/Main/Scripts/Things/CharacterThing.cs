using System;
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

    // The character's portrait
    public Sprite thingPortrait;

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

        public CharacterInfo LoadCharacter(string characterName, string characterCategory = "Player")
        {
            return FromString(PlayerPrefs.GetString(characterCategory + "_" + characterName));
        }

        public void SaveCharacter(string characterName, string characterCategory = "Player")
        {
            PlayerPrefs.SetString(characterCategory + "_" + characterName, ToString());

            // Save the character name to the list of characters
            string characterList = PlayerPrefs.GetString("Characters_" + characterCategory);

            if (!characterList.Contains(characterName))
            {
                characterList += (characterList == "" ? "" : ",") + characterName;
                PlayerPrefs.SetString("Characters_" + characterCategory, characterList);
            }

            PlayerPrefs.Save();
        }

        public static List<CharacterInfo> LoadCharacters(string characterCategory = "Player")
        {
            List<CharacterInfo> characters = new List<CharacterInfo>();

            string characterList = PlayerPrefs.GetString("Characters_" + characterCategory);

            if (characterList != "")
            {
                string[] characterNames = characterList.Split(',');

                foreach (string characterName in characterNames)
                {
                    if (characterName != "")
                    {
                        CharacterInfo character = new CharacterInfo().LoadCharacter(characterName, characterCategory);
                        characters.Add(character);
                    }
                }
            }

            return characters;
        }

        public static void DeleteCharacter(string characterName, string characterCategory = "Player")
        {
            PlayerPrefs.DeleteKey(characterCategory + "_" + characterName);

            // Remove the character name from the list of characters
            string characterList = PlayerPrefs.GetString("Characters_" + characterCategory);

            if (characterList.Contains(characterName))
            {
                characterList = characterList.Replace(characterName + ",", "");
                PlayerPrefs.SetString("Characters_" + characterCategory, characterList);
            }

            PlayerPrefs.Save();
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

            if (value.inventory != null)
                inventory = value.inventory;
            else
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

    public void MyTurn()
    {

        if (actionList != null)
        {
            actionList.PopulateActionList(GetComponentsInChildren<ActionThing>());
            actionList.ResetActions();
        }
        else
            Debug.LogWarning("No action list found for character " + thingName);

        DisplayInventory(true);
    }

    public void DisplayInventory(bool display, ActionList actionListToUse = null)
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

        if (movementController != null)
            movementController.movementInput = direction;

        base.Move(direction);
    }

    // Method to perform primary action on character
    public override void PrimaryAction(bool pressed)
    {
        if (actionList != null)
            actionList.PrimaryAction(pressed);

        if (movementController != null)
            movementController.jumpInput = pressed;

        base.PrimaryAction(pressed);
    }

    // Method to perform secondary action on character
    public override void SecondaryAction(bool pressed)
    {
        if (actionList != null)
            actionList.SecondaryAction(pressed);

        base.SecondaryAction(pressed);
    }

    #endregion

    #region Character Assembly

    // List of character part prefabs used to assemble the character
    [UnityEngine.Serialization.FormerlySerializedAs("characterPartPrefabs")] public List<CharacterPartThing.CharacterPartInfo> characterParts = new List<CharacterPartThing.CharacterPartInfo>();
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

        foreach (CharacterPartThing.CharacterPartInfo characterPart in characterParts)
        {
            if (characterPart.prefab == null)
                continue;

            if (CharacterPartThing.Instantiate(out CharacterPartThing characterPartThing, characterPart, characterBase.transform))
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
    }

    // Method to convert the character to a JSON string
    public override string ToString()
    {
        return characterInfo.ToString();
    }

    // Method to save the character to player prefs
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

    // Method to load the character from player prefs
    [Button]
    public void LoadCharacter(string characterName, string characterCategory = "Player")
    {
        characterInfo.LoadCharacter(characterName, characterCategory);
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