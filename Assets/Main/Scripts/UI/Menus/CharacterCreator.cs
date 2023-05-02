using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;

[RequireComponent(typeof(CharacterUI))]
public class CharacterCreator : GameThing
{
    #region Fields

    // The list of character parts
    public CharacterPartList characterPartList;

    // The parent transform for character parts
    public Transform characterPartParent;

    // Reference to the CharacterThing instance for visualization
    public CharacterThing characterThing;

    #endregion

    #region Properties

    // A reference to the character currently being created
    private CharacterThing.CharacterInfo currentCharacter;

    // A dictionary to store the selected parts for the current character
    private Dictionary<string, int> selectedParts;

    // The current category index
    private int currentCategoryIndex;

    // Dictionary to map thingType to the list of parts of that type
    private Dictionary<string, List<CharacterPartThing.CharacterPartInfo>> categorizedParts;

    #endregion

    #region Methods

    private void Start()
    {
        // Initialize the current character with default parts and values
        InitializeCharacter();
    }

    private void InitializeCharacter()
    {
        // Initialize the selectedParts dictionary
        selectedParts = new Dictionary<string, int>();

        // Initialize the categorizedParts dictionary
        categorizedParts = new Dictionary<string, List<CharacterPartThing.CharacterPartInfo>>();

        // Initialize the current character with default values
        // (set the default name, sprite, and other values as needed)
        currentCharacter = new CharacterThing.CharacterInfo(/*...*/);

        // Set the default parts for each category in the characterPartList
        foreach (GameObject characterPart in characterPartList.characterParts)
        {
            CharacterPartThing partThing = characterPart.GetComponent<CharacterPartThing>();
            if (partThing != null)
            {
                string category = partThing.thingType;
                if (!selectedParts.ContainsKey(category))
                {
                    selectedParts[category] = 0;
                }

                // Add the part to the categorizedParts dictionary
                if (!categorizedParts.ContainsKey(category))
                {
                    categorizedParts[category] = new List<CharacterPartThing.CharacterPartInfo>();
                }
                categorizedParts[category].Add(partThing.characterPartInfo);
            }
        }

        // Update the character with the initial parts
        UpdateCharacter();
    }

    private void UpdateCharacter()
    {
        // Build a list of chosen CharacterPartThing.CharacterPartInfo instances
        List<CharacterPartThing.CharacterPartInfo> chosenParts = new List<CharacterPartThing.CharacterPartInfo>();

        foreach (KeyValuePair<string, int> selectedPart in selectedParts)
        {
            string category = selectedPart.Key;
            int index = selectedPart.Value;

            // Add the selected part to the list of chosen parts
            chosenParts.Add(categorizedParts[category][index]);
        }

        // Set the currentCharacter's characterParts list to the list of chosen parts
        currentCharacter.characterParts = chosenParts;

        // Set the CharacterThing's characterInfo to the currentCharacter
        characterThing.characterInfo = currentCharacter;

        // Call AssembleCharacter() on the CharacterThing instance
        characterThing.AssembleCharacter();

        // Update the character UI with the modified character
        GetComponent<CharacterUI>().characterInfo = currentCharacter;
    }

    #region Input
    private Vector2Int inputDirection = Vector2Int.zero;

    public override void Move(Vector2 direction)
    {
        base.Move(direction);

        inputDirection = Vector2Int.RoundToInt(direction);

        List<string> categories = new List<string>(selectedParts.Keys);

        if (Mathf.Abs(inputDirection.x) > Mathf.Abs(inputDirection.y))
        {
            // Change the selected part index based on the input direction
            string currentCategory = categories[currentCategoryIndex];
            selectedParts[currentCategory] += inputDirection.x;
        }
        else if (Mathf.Abs(inputDirection.x) < Mathf.Abs(inputDirection.y))
        {
            // Change the selected part category based on the input direction
            currentCategoryIndex += inputDirection.y;
            currentCategoryIndex = Mathf.Clamp(currentCategoryIndex, 0, categories.Count - 1);
        }

        // Update the character with the modified parts
        UpdateCharacter();
    }

    public override void PrimaryAction(bool value)
    {
        base.PrimaryAction(value);

        if (value)
        {
            // Confirm the current character and return to the character selection screen
            // (add the created character to the list of available characters, etc.)

            // Save the character
            currentCharacter.SaveCharacter(currentCharacter.name, "Player");

            // Load the characters
            CharacterSelect.LoadCharacters();

            // Return to the character selection screen
            gameObject.SetActive(false);
        }
    }

    public override void SecondaryAction(bool value)
    {
        base.SecondaryAction(value);

        if (value)
        {
            // Cancel character creation and return to the character selection screen
            // (don't discard the created character, though, just in case the player changes their mind about creating it)

            // Return to the character selection screen
            gameObject.SetActive(false);
        }
    }
    #endregion
    #endregion
}
