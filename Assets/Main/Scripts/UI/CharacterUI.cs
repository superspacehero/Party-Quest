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
        get
        {
            return characterInfo.name;
        }
        set
        {
            characterInfo = characterInfo.LoadCharacter(value, characterCategory);
        }
    }

    public CharacterThing.CharacterInfo characterInfo
    {
        get => _characterInfo;
        set
        {
            _characterInfo = value;

            if (characterNameText != null)
                characterNameText.text = _characterInfo.name;
            if (characterPortraitImage != null)
                characterPortraitImage.sprite = General.StringToSprite(_characterInfo.portrait);

            if (characterValueText != null)
                characterValueText.text = _characterInfo.value.ToString();
        }
    }
    [SerializeField] private CharacterThing.CharacterInfo _characterInfo;

    [SerializeField] private bool loadCharacterOnStart;

    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI characterValueText;
    [SerializeField] private Image characterPortraitImage;

    private void Start()
    {
        if (loadCharacterOnStart)
            characterName = characterName;
    }
}
