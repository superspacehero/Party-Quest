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

    // Team that the character belongs to
    public int team;

    #region Controlling Characters

    public void MyTurn()
    {
        if (actionList != null)
            actionList.ResetActions();
        DisplayActionList(true);
    }

    public void DisplayActionList(bool display)
    {
        if (actionList != null)
            actionList.displayActionList = display;
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
    public void Move(Vector2 direction)
    {
        if (actionList != null)
            actionList.Move(direction);

        if (movementController != null)
            movementController.movementInput = direction;
    }

    // Method to perform primary action on character
    public void PrimaryAction(bool pressed)
    {
        if (actionList != null)
            actionList.PrimaryAction(pressed);

        if (movementController != null)
            movementController.jumpInput = pressed;
    }

    // Method to perform secondary action on character
    public void SecondaryAction(bool pressed)
    {
        if (actionList != null)
            actionList.SecondaryAction(pressed);
    }
    #endregion

    #region Character Assembly

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

            foreach (GameObject characterPartPrefab in characterPartPrefabs)
            {
                if (characterPartPrefab == null)
                    continue;

                if (Instantiate(characterPartPrefab, characterBase.transform).TryGetComponent(out CharacterPartThing characterPartThing))
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

            if (actionList != null)
                actionList.PopulateActionList(GetComponentsInChildren<ActionThing>());
            else
                Debug.LogWarning("No action list found for character " + thingName);
        }

        // Method to save the character to a JSON string
        public string Save()
        {
            string characterSaveData = "{";

            // Save the character information
            characterSaveData += "\"thingName\": \"" + thingName + "\",";
            characterSaveData += "\"thingValue\": " + thingValue + ",";
            characterSaveData += JsonUtility.ToJson(baseVariables);

            // Save the character parts
            characterSaveData += ",\"parts\": [";
            for (int i = 0; i < characterPartPrefabs.Count; i++)
            {
                if (characterPartPrefabs[i] == null)
                    continue;

                characterSaveData += "{";
                // Save the character part prefab's ID
                characterSaveData += "\"thingName\": \"" + characterPartPrefabs[i].name + "\",";

                    // Save the character part's red, green, and blue colors
                    characterSaveData += "\"r\": " + parts[i].redColor + ",";
                    characterSaveData += "\"g\": " + parts[i].greenColor + ",";
                    characterSaveData += "\"b\": " + parts[i].blueColor;

                characterSaveData += "}";

                if (i < characterPartPrefabs.Count - 1)
                    characterSaveData += ",";
            }
            characterSaveData += "]";

            characterSaveData += "}";
            
            return characterSaveData;
        }

        // Method to load the character from a JSON string
        public void Load(string characterSaveData)
        {
            // Load the character information
            JsonUtility.FromJsonOverwrite(characterSaveData, this);

            // Load the character parts
            string[] parts = characterSaveData.Split(new string[] { "\"parts\": [" }, StringSplitOptions.None)[1].Split(new string[] { "]}" }, StringSplitOptions.None)[0].Split(new string[] { "},{" }, StringSplitOptions.None);
            for (int i = 0; i < parts.Length; i++)
            {
                // Load the character part prefab's name
                string partName = parts[i].Split(new string[] { "\"thingName\": \"" }, StringSplitOptions.None)[1].Split(new string[] { "\"," }, StringSplitOptions.None)[0];

                // Load the character part's red, green, and blue colors
                float r = float.Parse(parts[i].Split(new string[] { "\"r\": " }, StringSplitOptions.None)[1].Split(new string[] { "," }, StringSplitOptions.None)[0]);
                float g = float.Parse(parts[i].Split(new string[] { "\"g\": " }, StringSplitOptions.None)[1].Split(new string[] { "," }, StringSplitOptions.None)[0]);
                float b = float.Parse(parts[i].Split(new string[] { "\"b\": " }, StringSplitOptions.None)[1].Split(new string[] { "}" }, StringSplitOptions.None)[0]);

                // Find the prefab with this name
                foreach (GameObject characterPart in GameManager.instance.characterPartList.characterParts)
                {
                    if (characterPart.name == partName)
                    {
                        // Add the part to the prefab list
                        characterPartPrefabs.Add(characterPart);
                        break;
                    }
                }
            }

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
    }
    
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        GameManager.RemoveCharacter(this);
    }
}