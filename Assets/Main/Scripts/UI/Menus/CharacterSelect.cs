using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;

[RequireComponent(typeof(CharacterUI))]
public class CharacterSelect : GameThing
{
    #region Fields
        [SerializeField] private bool initialized, canAddNewCharacter;
        [SerializeField] private CharacterThing.CharacterInfo newCharacter;
        [SerializeField] private LocalizedString newCharacterName;
        [SerializeField] private Sprite newCharacterSprite;

        [SerializeField] private GameObject toNewCharacterArrow, characterSelectArrows, toCharacterSelectArrow;
    #endregion

    #region Properties
        private static List<CharacterThing.CharacterInfo> characters, selectedCharacters = new List<CharacterThing.CharacterInfo>();
        private static bool charactersLoaded = false;

        public CharacterThing.CharacterInfo? selectedCharacter
        {
            get => _selectedCharacter;
            set
            {
                _selectedCharacter = value;

                if (charactersLoaded)
                {
                    if (value != null)
                    {
                        characterSelectArrows.SetActive(false);
                        toCharacterSelectArrow.SetActive(false);
                    }
                    else
                        newCharacterSelected = true;
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
                _newCharacterSelected = ((canAddNewCharacter && value) || characters.Count == 1) ? true : false; 

                if (charactersLoaded)
                    characterUI.characterInfo = _newCharacterSelected ? newCharacter : characters[characterIndex];

                if (toNewCharacterArrow)
                {
                    if (canAddNewCharacter)
                        toNewCharacterArrow.SetActive(!_newCharacterSelected);
                    else
                        toNewCharacterArrow.SetActive(false);
                }
                if (characterSelectArrows)
                    characterSelectArrows.SetActive(!_newCharacterSelected);
                if (toCharacterSelectArrow)
                    toCharacterSelectArrow.SetActive(_newCharacterSelected && characters.Count > 1);
            }
        }
        private bool _newCharacterSelected = false;

        private int characterIndex
        {
            get => _characterIndex;
            set
            {
                value = Mathf.Clamp(value, 1, characters.Count - 1);

                if (!newCharacterSelected)
                    _characterIndex = value;

                if (charactersLoaded)
                    characterUI.characterInfo = characters[characterIndex];
            }
        }
        private int _characterIndex = 1;
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
                    characters = CharacterThing.CharacterInfo.LoadCharacters("Player");

                    Debug.Log($"Found {characters.Count} characters");

                    // Create the new character
                    newCharacter = new CharacterThing.CharacterInfo
                    (
                        newCharacterName,
                        newCharacterSprite,
                        0,
                        null,
                        new GameThing.GameThingVariables(),
                        null
                    );
                    
                    // Add the new character to the list of characters
                    characters.Insert(0, newCharacter);
                }
            }

            yield return null;

            initialized = true;

            // Make the character select a child of the character select menu
            if (CharacterSelectMenu.instance != null)
                transform.SetParent(CharacterSelectMenu.instance.characterSelectParent, false);

            // Set the character UI
            newCharacterSelected = characters.Count == 1;
        }

        #region Input
            private Vector2Int inputDirection = Vector2Int.zero;

            public override void Move(Vector2 direction)
            {
                base.Move(direction);

                if (!charactersLoaded || !initialized)
                    return;

                Vector2Int newInputDirection = Vector2Int.RoundToInt(direction);

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

            public override void PrimaryAction(bool value)
            {
                base.PrimaryAction(value);

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

                        }
                    }
                    else
                    {
                        // If all players have selected a character, go to the next menu
                        if (CharacterSelectMenu.instance != null && selectedCharacters.Count == CharacterSelectMenu.instance.characterSelects.Count)
                        {
                            CharacterSelectMenu.instance.NextMenu();
                            return;
                        }

                        // Set the character as our selected character
                        selectedCharacter = characters[characterIndex];

                        // Add the character to the list of selected characters
                        selectedCharacters.Add(characters[characterIndex]);

                        // Remove the character from the list of characters
                        characters.RemoveAt(characterIndex);
                    }
                }
            }
            
            public override void SecondaryAction(bool value)
            {
                base.SecondaryAction(value);

                if (!charactersLoaded || !initialized)
                    return;

                if (value)
                {
                    if (selectedCharacter != null)
                    {
                        if (!newCharacterSelected)
                        {
                            // Add the character back to the list of characters
                            characters.Insert(characterIndex, selectedCharacter.Value);

                            // Remove the character from the list of selected characters
                            selectedCharacters.Remove(selectedCharacter.Value);
                        }

                        // Set the character as not selected
                        selectedCharacter = null;
                    }
                    else
                    {
                        // Remove this character select
                        if (CharacterSelectMenu.instance != null)
                            CharacterSelectMenu.instance.characterSelects.Remove(this);

                        // Destroy this character select
                        Destroy(gameObject);
                    }
                }
            }
        #endregion
    #endregion
}
