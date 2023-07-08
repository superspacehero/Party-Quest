using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSelectMenu : Menu
{
    public static CharacterSelectMenu instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CharacterSelectMenu>();
            }
            return _instance;
        }
    }
    private static CharacterSelectMenu _instance;

    public Transform characterSelectParent
    {
        get
        {
            if (_characterSelectParent == null)
            {
                _characterSelectParent = transform;
            }
            return _characterSelectParent;
        }
    }
    [SerializeField] private Transform _characterSelectParent;

    public PlayerInputManager playerInputManager
    {
        get
        {
            if (_playerInputManager == null)
                TryGetComponent(out _playerInputManager);
            return _playerInputManager;
        }
    }
    private PlayerInputManager _playerInputManager;

    public List<CharacterSelect> characterSelects = new List<CharacterSelect>();

    public bool allCharacterSelectsReady
    {
        get
        {
            _allCharacterSelectsReady = true;
            foreach (CharacterSelect characterSelect in characterSelects)
            {
                if (characterSelect != null)
                {
                    characterSelect.UpdateCharacterSelect();

                    if (characterSelect.selectedCharacter == null)
                        _allCharacterSelectsReady = false;
                }
            }

            return _allCharacterSelectsReady;
        }
    }
    private bool _allCharacterSelectsReady;

    public void CheckAllCharacterSelectsReady()
    {
        if (allCharacterSelectsReady)
        {
            
        }
        else
        {

        }
    }

    public override void NextMenu()
    {
        // Save all players and their selected characters
        GameManager.players.Clear();

        foreach (CharacterSelect characterSelect in characterSelects)
        {
            // Save player and character
            GameManager.players.Add(new GameManager.PlayerAndCharacter(characterSelect.playerInput, characterSelect.selectedCharacter.Value));
        }

        base.NextMenu();
    }

    public override void Select()
    {
        base.Select();

        // playerInputManager.enabled = true;
        playerInputManager.EnableJoining();
    }

    public override void Deselect()
    {
        base.Deselect();

        // playerInputManager.enabled = false;
        GameManager.ResetInputModule();

        // Reset all character selects
        while (characterSelects.Count > 0)
        {
            CharacterSelect characterSelect = characterSelects[0];
            Destroy(characterSelect.character.gameObject);
            Destroy(characterSelect.gameObject);
        }

        characterSelects.Clear();
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        _instance = null;
    }
}
