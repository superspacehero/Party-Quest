using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;

[RequireComponent(typeof(CharacterUI))]
public class CharacterSelect : GameThing
{
    #region Fields
        private static List<CharacterThing.CharacterInfo> characters;
        private static bool initialized = false;
        [SerializeField] private bool canAddNewCharacter;
        [SerializeField] private CharacterThing.CharacterInfo newCharacter;
        [SerializeField] private LocalizedString newCharacterName;
        [SerializeField] private Sprite newCharacterSprite;

        [SerializeField] private GameObject newCharacterArrow, existingCharacterArrow;
    #endregion

    #region Properties
        private CharacterUI _characterUI;

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

        private bool _newCharacterSelected = false;

        private bool newCharacterSelected
        {
            get => _newCharacterSelected;
            set
            {
                _newCharacterSelected = value;
                characterUI.characterInfo = value ? newCharacter : characters[characterIndex];

                if (newCharacterArrow && canAddNewCharacter)
                    newCharacterArrow.SetActive(value);
                if (existingCharacterArrow)
                    existingCharacterArrow.SetActive(!value);
            }
        }

        private int _characterIndex = 1;

        private int characterIndex
        {
            get => _characterIndex;
            set
            {
                // Don't allow the value to equal 0, because that's the new character
                if (value >= 0)
                    value++;

                // Wrap the value around if it's too high or too low
                if (value >= characters.Count)
                    value = 1;
                else if (value < 1)
                    value = characters.Count - 1;

                _characterIndex = value;
            }
        }
    #endregion

    #region Methods
        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            StartCoroutine(Initialize());
        }

        public IEnumerator Initialize()
        {
            if (!initialized)
            {
                initialized = true;

                // Find all the characters of the category "Player" in player prefs
                while (characters == null)
                {
                    characters = CharacterThing.CharacterInfo.LoadCharacters("Player");
                    characters.Insert(0,
                        new CharacterThing.CharacterInfo(
                            newCharacterName,
                            newCharacterSprite,
                            0,
                            null,
                            new GameThing.GameThingVariables(),
                            null
                        )
                    );
                }
            }

            yield return null;

            // Create the new character
            newCharacter = characters[0];

            // Set the character UI
            newCharacterSelected = characters.Count == 1;
        }

        #region Input
            private Vector2Int inputDirection = Vector2Int.zero;

            public override void Move(Vector2 direction)
            {
                base.Move(direction);

                Vector2Int newInputDirection = Vector2Int.RoundToInt(direction);

                if (newInputDirection != inputDirection)
                {
                    inputDirection = newInputDirection;

                    if (Mathf.Abs(inputDirection.x) > Mathf.Abs(inputDirection.y))
                    {
                        if (!newCharacterSelected)
                            characterIndex += inputDirection.x;
                    }
                    else if (Mathf.Abs(inputDirection.x) < Mathf.Abs(inputDirection.y))
                    {
                        if (inputDirection.y > 0)
                            newCharacterSelected = true;
                        else if (inputDirection.y < 0)
                            newCharacterSelected = false;
                    }
                }
            }
        #endregion
    #endregion
}
