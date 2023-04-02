using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public List<CharacterSelect> characterSelects = new List<CharacterSelect>();

    private bool allCharacterSelectsReady
    {
        get
        {
            foreach (CharacterSelect characterSelect in characterSelects)
            {
                if (characterSelect.selectedCharacter == null)
                    return false;
            }
            return true;
        }
    }
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
            UnityEngine.InputSystem.PlayerInput playerInput = characterSelect.GetComponentInChildren<UnityEngine.InputSystem.PlayerInput>();
            if (playerInput != null)
            {
                // Save player and character
                GameManager.players.Add(new GameManager.PlayerAndCharacter(playerInput, characterSelect.selectedCharacter.Value));
            }
            else
            {
                Debug.LogError("CharacterSelect does not have a PlayerInput component");
            }
        }

        base.NextMenu();
    }
}
