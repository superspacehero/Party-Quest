using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

[RequireComponent(typeof(CharacterUI))]
public class CharacterSelect : GameThing
{
    #region Fields
    
    [SerializeField] private bool initialized, canAddNewCharacter, canAddExtraPlayers = true;
    [SerializeField] private LocalizedString newCharacterName;
    [SerializeField] private Sprite newCharacterSprite;

    [SerializeField] private GameObject extraCharacterSelectPrefab;

    [SerializeField] private GameObject selectedOverlay, selectedCheckmark, addPlayerButton;

    [SerializeField] private GameObject toNewCharacterArrow, characterSelectArrows, toCharacterSelectArrow;

    public GameObject character;
    #endregion

    #region Properties

    private ThingInput thingInput
    {
        get
        {
            if (_thingInput == null)
            {
                _thingInput = GetComponentInChildren<ThingInput>();
            }
            return _thingInput;
        }
    }
    private ThingInput _thingInput;

    public UnityEngine.InputSystem.PlayerInput playerInput
    {
        get
        {
            if (_playerInput == null)
            {
                _playerInput = GetComponentInChildren<UnityEngine.InputSystem.PlayerInput>();
            }
            return _playerInput;
        }

        set
        {
            _playerInput = value;
        }
    }
    private UnityEngine.InputSystem.PlayerInput _playerInput;
    private static CharacterThing.CharacterInfo newCharacter;
    private static List<CharacterThing.CharacterInfo> characters, selectedCharacters = new List<CharacterThing.CharacterInfo>();
    private bool charactersLoaded = false;

    public CharacterThing.CharacterInfo? selectedCharacter
    {
        get => _selectedCharacter;
        set
        {
            _selectedCharacter = value;

            if (charactersLoaded)
            {
                UpdateArrowStates(value == null);
            }
        }
    }
    private CharacterThing.CharacterInfo? _selectedCharacter = null;

    private CharacterUI characterUI
    {
        get
        {
            if (_characterUI == null)
            {
                TryGetComponent(out _characterUI);
            }
            return _characterUI;
        }
    }
    private CharacterUI _characterUI;

    private bool newCharacterSelected
    {
        get => _newCharacterSelected;
        set
        {
            _newCharacterSelected = (canAddNewCharacter && value);

            if (!charactersLoaded)
            {
                UpdateArrowStates(false);
                return;
            }

            UpdateArrowStates();

            UpdateCharacterSelect();
        }
    }
    private bool _newCharacterSelected = false;

    private void UpdateArrowStates(bool showArrows = true)
    {
        if (toNewCharacterArrow)
            toNewCharacterArrow.SetActive((showArrows) ? !_newCharacterSelected : false);

        if (characterSelectArrows)
            characterSelectArrows.SetActive((showArrows) ? !_newCharacterSelected : false);

        if (toCharacterSelectArrow)
            toCharacterSelectArrow.SetActive((showArrows) ? _newCharacterSelected : false);
    }

    private int characterIndex
    {
        get => _characterIndex;
        set
        {
            // Make sure the value wraps from to the number of characters
            if (value < 0)
                value = characters.Count - 1;
            else if (value >= characters.Count)
                value = 0;

            if (!newCharacterSelected)
                _characterIndex = value;

            if (charactersLoaded)
            {
                if (characters.Count <= 0)
                    newCharacterSelected = true;

                currentCharacter = _newCharacterSelected ? newCharacter : characters[characterIndex];
                characterUI.characterInfo = currentCharacter;

                selectedOverlay.SetActive(!newCharacterSelected && selectedCharacters.Contains(characters[characterIndex]));
                selectedCheckmark.SetActive(!newCharacterSelected && (selectedCharacter != null) ? characters[characterIndex].Equals(selectedCharacter) : false);
            }
        }
    }
    private int _characterIndex = 0;

    private CharacterThing.CharacterInfo currentCharacter
    {
        get => _currentCharacter;
        set => _currentCharacter = value;
    }
    private CharacterThing.CharacterInfo _currentCharacter;

    #endregion

    #region Methods
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        if (CharacterSelectMenu.instance != null)
            CharacterSelectMenu.instance.characterSelects.Add(this);

        selectedCharacter = null;
        addPlayerButton.SetActive(canAddExtraPlayers);
    }
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        if (CharacterSelectMenu.instance != null)
            CharacterSelectMenu.instance.characterSelects.Remove(this);
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    public IEnumerator Start()
    {
        if (!charactersLoaded)
        {
            charactersLoaded = true;

            // Find all the characters of the category "Player"
            while (characters == null)
            {
                // Load the characters
                LoadCharacters();

                Debug.Log($"Found {characters.Count} characters");

                // Create the new character
                newCharacter = new CharacterThing.CharacterInfo
                (
                    newCharacterName.ToString(),
                    newCharacterSprite,
                    0,
                    null,
                    new GameThing.GameThingVariables(),
                    null
                );
            }
        }

        yield return null;

        // Set the character select as initialized
        initialized = true;
        if (thingInput != null)
            thingInput.canControl = false;

        // Make the character select a child of the character select menu
        if (CharacterSelectMenu.instance != null)
            transform.SetParent(CharacterSelectMenu.instance.characterSelectParent, false);

        // Set the character UI
        newCharacterSelected = characters.Count == 1;
    }

    public static void LoadCharacters()
    {
        characters = CharacterThing.CharacterInfo.LoadCharacters("Player");

        if (CharacterSelectMenu.instance != null)
            foreach (CharacterSelect characterSelect in CharacterSelectMenu.instance.characterSelects)
                characterSelect.UpdateCharacterSelect();
    }

    public void UpdateCharacterSelect()
    {
        if (!charactersLoaded)
            return;

        // Update the character select
        if (characters.Count > 0)
            characterIndex = characters.IndexOf(currentCharacter);
    }

    #region Input
    [SerializeField, Range(0f, 1f)] private float inputDeadzone = 0.75f;
    private Vector2Int inputDirection = Vector2Int.zero;

    public override void Move(Vector2 direction)
    {
        if (direction.normalized.magnitude < inputDeadzone)
            direction = Vector2.zero;

        Vector2Int newInputDirection = Vector2Int.RoundToInt(direction);

        if (GetAttachedThing() != null && GetAttachedThing().gameObject.activeSelf)
        {
            if (newInputDirection != inputDirection)
            {
                base.Move(newInputDirection);
            }
        }
        else if (extraSelect != null)
        {
            extraSelect.Move(newInputDirection);
        }
        else
        {
            if (!charactersLoaded || !initialized)
                return;

            if (newInputDirection != inputDirection)
            {
                inputDirection = newInputDirection;

                if (selectedCharacter != null)
                    return;

                if (Mathf.Abs(inputDirection.x) > Mathf.Abs(inputDirection.y))
                {
                    if (!newCharacterSelected)
                        characterIndex += inputDirection.x;
                }
                else if (Mathf.Abs(inputDirection.x) < Mathf.Abs(inputDirection.y))
                {
                    if (canAddNewCharacter)
                        if (inputDirection.y > 0)
                            newCharacterSelected = true;
                        else if (inputDirection.y < 0)
                            newCharacterSelected = false;
                }
            }
        }
    }

    public void MoveHorizontal(int direction)
    {
        Move(new Vector2(direction, 0f));
        Move(Vector2.zero);
    }

    public void MoveVertical(int direction)
    {
        Move(new Vector2(0f, direction));
        Move(Vector2.zero);
    }

    public override void PrimaryAction(bool value)
    {
        if (GetAttachedThing() != null && GetAttachedThing().gameObject.activeSelf)
        {
            base.PrimaryAction(value);
        }
        else if (extraSelect != null)
        {
            extraSelect.PrimaryAction(value);
        }
        else
        {
            if (!charactersLoaded || !initialized)
                return;

            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                return;
            }

            if (value)
            {
                if (newCharacterSelected)
                {
                    if (canAddNewCharacter)
                    {
                        if (GetAttachedThing() != null)
                            GetAttachedThing().gameObject.SetActive(true);
                    }
                }
                else
                {
                    // If all players have selected a character, go to the next menu
                    if (CharacterSelectMenu.instance != null && CharacterSelectMenu.instance.allCharacterSelectsReady)
                    {
                        CharacterSelectMenu.instance.NextMenu();
                        return;
                    }

                    // Set the character as our selected character
                    if (selectedCharacters.Contains(characters[characterIndex]))
                        return;

                    selectedCharacter = characters[characterIndex];

                    // Add the character to the list of selected characters
                    selectedCharacters.Add(characters[characterIndex]);

                    // Update the character select
                    UpdateCharacterSelect();
                }
            }
        }
    }

    public void PrimaryAction()
    {
        PrimaryAction(true);
        PrimaryAction(false);
    }

    public override void SecondaryAction(bool value)
    {
        if (GetAttachedThing() != null && GetAttachedThing().gameObject.activeSelf)
        {
            base.SecondaryAction(value);
        }
        else if (extraSelect != null)
        {
            extraSelect.SecondaryAction(value);
        }
        else
        {
            if (!charactersLoaded || !initialized)
                return;

            if (value)
            {
                if (selectedCharacter != null)
                {
                    if (!newCharacterSelected)
                    {
                        // Remove the character from the list of selected characters
                        selectedCharacters.Remove(selectedCharacter.Value);
                    }

                    // Set the character as not selected
                    selectedCharacter = null;

                    CharacterSelectMenu.instance.CheckAllCharacterSelectsReady();
                }
                else
                {
                    // Loop through all the connected character selects
                    CharacterSelect loopedCharacterSelect = this;
                    while (loopedCharacterSelect.extraSelect != null)
                        loopedCharacterSelect = loopedCharacterSelect.extraSelect;

                    // Remove this character select
                    if (CharacterSelectMenu.instance != null)
                        CharacterSelectMenu.instance.characterSelects.Remove(loopedCharacterSelect);

                    // Destroy this character select
                    Destroy(loopedCharacterSelect.character);
                    Destroy(loopedCharacterSelect.gameObject);
                }
            }
        }
    }

    public void SecondaryAction()
    {
        SecondaryAction(true);
        SecondaryAction(false);
    }

    public CharacterSelect extraSelect;

    public override void TertiaryAction(bool value)
    {
        if (GetAttachedThing() != null && GetAttachedThing().gameObject.activeSelf)
        {
            base.TertiaryAction(value);
        }
        else
        {
            // Add a new player with the same input device as this one
            if (value && selectedCharacter != null && canAddExtraPlayers)
            {
                if (CharacterSelectMenu.instance != null)
                {
                    // Loop through all the connected character selects
                    CharacterSelect loopedCharacterSelect = this;
                    while (loopedCharacterSelect.extraSelect != null)
                        loopedCharacterSelect = loopedCharacterSelect.extraSelect;

                    if (Instantiate(extraCharacterSelectPrefab, CharacterSelectMenu.instance.characterSelectParent).TryGetComponent(out CharacterSelect characterSelect))
                    {
                        characterSelect.playerInput = playerInput;
                        loopedCharacterSelect.extraSelect = characterSelect;
                    }
                }
            }
        }
    }

    public void TertiaryAction()
    {
        TertiaryAction(true);
        TertiaryAction(false);
    }
    #endregion
    #endregion
}
