using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using I2.Loc;

public class CharacterUI : MonoBehaviour
{
    public TextMeshProUGUI characterName;
    public Image characterImage;
    [SerializeField] private Localize localizedString;
    [SerializeField] private LocalizationParamsManager localizationParamsManager;

    public bool useCurrentCharacter = true;
    public Character character
    {
        set
        {
            if (localizationParamsManager != null)
            {
                localizationParamsManager.SetParameterValue("CHARACTER", value.name, true);
            }
        }
    }

    TextList.TextSection mainQuests, optionalQuests;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        if (useCurrentCharacter && GameManager.currentCharacter != null)
            character = GameManager.currentCharacter;
    }
}
