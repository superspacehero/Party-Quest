using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

[RequireComponent(typeof(CharacterUI))]
public class CharacterCreator : GameThing
{
    // CharacterCreator is a menu that allows the player to create a character.
    // It uses a CharacterThing instance to visualize the character as it is being created.
    // It also uses a CharacterUI instance to display the character's stats.

    // The general flow of the CharacterCreator is as follows:
    // 1. Initialize by categorizing the character parts in the characterPartList by thingType.
    // 2. Get a list of all available character part slots in the CharacterThing instance.
    //  - This is done by calling CharacterThing.GetCharacterPartSlots().
    // 3. Make a dictionary with each slot as the key and the index for the selected part as the value.
    //  - This will need to be updated as the player selects different parts, but it will still need to maintain the selected parts.
    // 4. Allow the player to scroll through the different slots and select a part for each slot.
    //  - This can be done by using the inputDirection to change the currentSlotIndex.
    //  - The currentSlotIndex can be used to get the current slot from the list of slots.
    //  - The currentSlot can be used to get the current slot's thingType.
    //  - The currentSlot's thingType can be used to get the list of parts of that type from the dictionary.
    // 5. Update the CharacterThing instance with the selected parts.
    // 6. Repeat steps 2-5 until the player is satisfied with the character.

    #region Fields

    // The list of character parts
    public CharacterPartList characterPartList;

    // Reference to the CharacterThing instance for visualization
    private CharacterThing characterThing
    {
        get
        {
            if (_characterThing == null)
            {
                _characterThing = GetComponentInChildren<CharacterThing>();
            }

            return _characterThing;
        }
    }
    [SerializeField] private CharacterThing _characterThing;

    [SerializeField] private RawImage characterPreviewImage;
    [SerializeField] private TMPro.TextMeshProUGUI characterText;

    #endregion

    #region Properties

    private Dictionary<string, List<GameObject>> categorizedParts;
    private List<int> selectedParts;
    private List<Inventory.ThingSlot> availableSlots;
    private int currentSlotIndex;

    private RenderTexture characterPreviewTexture
    {
        get
        {
            if (_characterPreviewTexture == null)
            {
                _characterPreviewTexture = new RenderTexture(256, 256, 24);
                _characterPreviewTexture.Create();
            }

            return _characterPreviewTexture;
        }
    }
    private RenderTexture _characterPreviewTexture;
    private Camera characterCamera
    {
        get
        {
            if (_characterCamera == null)
            {
                _characterCamera = characterThing.GetComponentInChildren<Camera>(true);
                if (_characterCamera != null)
                    _characterCamera.gameObject.SetActive(true);
            }

            return _characterCamera;
        }
    }
    private Camera _characterCamera;

    #endregion

    #region Methods

    private void Start()
    {
        // Step 1: Categorize the character parts
        CategorizeCharacterParts();

        // Step 2: Get a list of all available character part slots
        if (characterThing == null)
        {
            Debug.LogError("CharacterThing is null");
            return;
        }

        availableSlots = characterThing.GetCharacterPartSlots();

        // Step 3: Make a dictionary with each slot as the key and the index for the selected part as the value
        InitializeSelectedParts();

        // Step 4: Allow the player to scroll through the different slots and select a part for each slot
        // This will be done in the Move() method
        characterPreviewImage.texture = characterPreviewTexture;
        SetText();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (characterThing != null)
            Destroy(characterThing.gameObject);
    }

    private void SetText()
    {
        if (characterText != null)
            characterText.text = $"{availableSlots[currentSlotIndex].thingType} - {currentSlotIndex + 1}/{availableSlots.Count}";
    }

    private void CategorizeCharacterParts()
    {
        categorizedParts = new Dictionary<string, List<GameObject>>();

        foreach (GameObject part in characterPartList.characterParts)
        {
            CharacterPartThing partThing = part.GetComponent<CharacterPartThing>();
            if (partThing != null)
            {
                string category = partThing.thingType;

                if (!categorizedParts.ContainsKey(category))
                {
                    categorizedParts[category] = new List<GameObject>();
                }
                categorizedParts[category].Add(part);
            }
        }
    }

    private void InitializeSelectedParts()
    {
        selectedParts = new List<int>(new int[availableSlots.Count]);
    }

    private void UpdateSelectedPartsList()
    {
        int newSize = availableSlots.Count;
        if (selectedParts.Count < newSize)
        {
            // Add default elements (0) to the list to match the new size
            for (int i = selectedParts.Count; i < newSize; i++)
            {
                selectedParts.Add(0);
            }
        }
        else if (selectedParts.Count > newSize)
        {
            // Remove elements from the list to match the new size
            selectedParts.RemoveRange(newSize, selectedParts.Count - newSize);
        }
    }

    private void UpdateCharacter()
    {
        // Step 5: Update the CharacterThing instance with the selected parts
        characterThing.characterParts = new List<CharacterPartThing.CharacterPartInfo>();
        for (int i = 0; i < availableSlots.Count; i++)
        {
            Inventory.ThingSlot slot = availableSlots[i];

            // Get the current slot's thingType
            string thingType = slot.thingType;

            // Get the list of parts of that type from the dictionary
            List<GameObject> parts = categorizedParts[thingType];

            // Get the index for the selected part
            int index = selectedParts[i];

            // Get the selected part
            if (parts[index] != null && index > 0 && parts[index].TryGetComponent(out CharacterPartThing partThing))
            {
                // Add the selected part to the character
                characterThing.characterParts.Add(partThing.characterPartInfo);
            }
        }

        characterThing.AssembleCharacter();

        // Update the available slots and selected parts after assembling the character
        availableSlots = characterThing.GetCharacterPartSlots();
        UpdateSelectedPartsList();

        // Update the character UI with the modified character
        GetComponent<CharacterUI>().characterInfo = characterThing.characterInfo;

        if (characterCamera != null)
        {
            characterPreviewImage.enabled = true;
            characterCamera.targetTexture = characterPreviewTexture;
        }
        else
        {
            characterPreviewImage.enabled = false;
        }

        SetText();
    }

    #region Input
    private Vector2Int inputDirection = Vector2Int.zero;

    public override void Move(Vector2 direction)
    {
        base.Move(direction);

        inputDirection = Vector2Int.RoundToInt(direction);

        if (Mathf.Abs(inputDirection.x) > Mathf.Abs(inputDirection.y))
        {
            string thingType = availableSlots[currentSlotIndex].thingType;
            selectedParts[currentSlotIndex] += inputDirection.x;

            int partCount = categorizedParts[thingType].Count;
            selectedParts[currentSlotIndex] = ((selectedParts[currentSlotIndex] % partCount) + partCount) % partCount;
        }
        else if (Mathf.Abs(inputDirection.x) < Mathf.Abs(inputDirection.y))
        {
            currentSlotIndex -= inputDirection.y;
            currentSlotIndex = Mathf.Clamp(currentSlotIndex, 0, availableSlots.Count - 1);
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

            if (characterCamera != null)
            {
                CharacterThing.CharacterInfo characterInfo = characterThing.characterInfo;
                characterInfo.portrait = General.TextureToString(characterCamera.targetTexture);
                characterThing.characterInfo = characterInfo;
            }

            // Save the character
            characterThing.SaveCharacter("Player");

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
