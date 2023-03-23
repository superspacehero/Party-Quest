using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterUI : MonoBehaviour
{
    public string characterCategory = "Player";

    public string characterName
    {
        get => _characterName;
        set
        {
            characterInfo = characterInfo.LoadCharacter(value, characterCategory);
        }
    }
    [SerializeField] private string _characterName;

    private CharacterThing.CharacterInfo characterInfo
    {
        get => _characterInfo;
        set
        {
            _characterInfo = value;
            characterNameText.text = _characterInfo.name;
        }
    }
    [SerializeField] private CharacterThing.CharacterInfo _characterInfo;

    [SerializeField] private TextMeshProUGUI characterNameText;

    private void Start()
    {
        characterName = characterName;
    }
}
