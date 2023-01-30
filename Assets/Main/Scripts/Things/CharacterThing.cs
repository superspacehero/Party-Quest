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

        // Method to convert the character to a JSON string
        new public string ToString()
        {
            string characterSaveData = "{";

            // Save the character information
            characterSaveData += "\"thingName\": \"" + thingName + "\",";
            characterSaveData += "\"thingValue\": " + thingValue + ",";

            characterSaveData += "\"baseVariables\": {";
            foreach (GameThingVariables.Variable variable in baseVariables.variables)
            {
                characterSaveData += "\"" + variable.name + "\": " + variable.value + (variable.name != baseVariables.variables[baseVariables.variables.Count - 1].name ? "," : "");
            }
            characterSaveData += "},";

            // Save the character parts
            characterSaveData += ",\"parts\": [";
            for (int i = 0; i < characterPartPrefabs.Count; i++)
            {
                if (characterPartPrefabs[i] == null)
                    continue;

                characterSaveData += "{";
                // Save the character part prefab's ID
                characterSaveData += "\"thingName\": \"" + characterPartPrefabs[i].name + "\",";

                    characterPartPrefabs[i].TryGetComponent(out CharacterPartThing characterPartThing);
                    
                    if (parts != null && parts.Count > i)
                        characterPartThing = parts[i];

                    // Save the character part's red, green, and blue colors
                    characterSaveData += "\"r\": " + characterPartThing.redColor + ",";
                    characterSaveData += "\"g\": " + characterPartThing.greenColor + ",";
                    characterSaveData += "\"b\": " + characterPartThing.blueColor;

                characterSaveData += "}";

                if (i < characterPartPrefabs.Count - 1)
                    characterSaveData += ",";
            }
            characterSaveData += "]";

            characterSaveData += "}";
            
            return characterSaveData;
        }

        // Method to save the character to player prefs
        [Button]
        public void SaveCharacter(string characterCategory = "Player")
        {
            PlayerPrefs.SetString(characterCategory + "_" + thingName, ToString());

            PlayerPrefs.Save();
        }

        // Method to create the character from a JSON string
        [Button]
        public void FromString(string characterSaveData)
        {
            // Load the character information

            thingName = characterSaveData.Split(new string[] { "\"thingName\": \"" }, StringSplitOptions.None)[1].Split(new string[] { "\"," }, StringSplitOptions.None)[0];
            thingValue = int.Parse(characterSaveData.Split(new string[] { "\"thingValue\": " }, StringSplitOptions.None)[1].Split(new string[] { "," }, StringSplitOptions.None)[0]);

            // Load the character's base variables
            if (characterSaveData.Contains("\"baseVariables\": {"))
            {
                string[] baseVariableString = characterSaveData.Split(new string[] { "\"baseVariables\": {" }, StringSplitOptions.None)[1].Split(new string[] { "}}" }, StringSplitOptions.None)[0].Split(new string[] { "," }, StringSplitOptions.None);

                foreach (string variable in baseVariableString)
                {
                    if (!string.IsNullOrEmpty(variable))
                    {
                        string variableKey = variable.Split(new string[] { "\"" }, StringSplitOptions.None)[1];
                        string variableValue = variable.Split(new string[] { "\"" }, StringSplitOptions.None)[3];

                        baseVariables.variables.Add(new GameThingVariables.Variable(variableKey, int.Parse(variableValue)));
                    }
                }
            }
            else
            {
                // Handle error for index going outside bounds of array
                Debug.LogError("No base variables found for character " + thingName, this);
            }

            // Load the character parts
            string[] parts = characterSaveData.Split(new string[] { "\"parts\": [" }, StringSplitOptions.None)[1].Split(new string[] { "]}" }, StringSplitOptions.None)[0].Split(new string[] { "},{" }, StringSplitOptions.None);
            for (int i = 0; i < parts.Length; i++)
            {
                // Load the character part prefab's name
                string partName = parts[i].Split(new string[] { "\"thingName\": \"" }, StringSplitOptions.None)[1].Split(new string[] { "\"," }, StringSplitOptions.None)[0];

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

            // Assemble the character
            AssembleCharacter();

            // Load the character part colors
            for (int i = 0; i < parts.Length; i++)
            {
                // Load the character part's red, green, and blue colors
                float r = float.Parse(parts[i].Split(new string[] { "\"r\": " }, StringSplitOptions.None)[1].Split(new string[] { "," }, StringSplitOptions.None)[0]);
                float g = float.Parse(parts[i].Split(new string[] { "\"g\": " }, StringSplitOptions.None)[1].Split(new string[] { "," }, StringSplitOptions.None)[0]);
                float b = float.Parse(parts[i].Split(new string[] { "\"b\": " }, StringSplitOptions.None)[1].Split(new string[] { "}" }, StringSplitOptions.None)[0]);
            }
        }

        // Method to load the character from player prefs
        [Button]
        public void LoadCharacter(string characterName, string characterCategory = "Player")
        {
            FromString(PlayerPrefs.GetString(characterCategory + "_" + characterName));
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