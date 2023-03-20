using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;
using TMPro;

public class ThingDisplay : MonoBehaviour
{
    [HideInInspector] public GameThing thingOwner;

    public GameThing thing
    {
        get => _thing;
        set
        {
            UpdateDisplay(value);

            _thing = value;
        }
    }
    private GameThing _thing;

    [SerializeField] private Image iconImage;
    public Button iconButton;

    [SerializeField, Space(10)] private List<TextMeshProUGUI> nameTexts;
    [SerializeField] private List<TextMeshProUGUI> descriptionTexts;

    [SerializeField] private List<LocalizationParamsManager> localizationParamsManagers;

    public void Select()
    {
        if (iconButton != null)
            iconButton.Select();
    }

    private void UpdateDisplay(GameThing displayThing)
    {
        if (displayThing == null)
        {
            foreach (TextMeshProUGUI text in nameTexts)
                text.text = "";
            foreach (TextMeshProUGUI text in descriptionTexts)
                text.text = "";
            if (iconImage != null)
                iconImage.enabled = false;
            if (iconButton != null)
            {
                iconButton.onClick.RemoveAllListeners();
                iconButton.interactable = false;
            }
        }
        else
        {
            foreach (TextMeshProUGUI text in nameTexts)
                text.text = displayThing.thingName;
            foreach (TextMeshProUGUI text in descriptionTexts)
                text.text = displayThing.thingDescription;

            foreach (LocalizationParamsManager localizationParamsManager in localizationParamsManagers)
                localizationParamsManager.SetParameterValue("THING", displayThing.thingName, true);

            if (iconImage != null)
            {
                iconImage.enabled = true;
                iconImage.sprite = displayThing.thingIcon;
                displayThing.SetColors(iconImage);
            }
            if (iconButton != null)
            {
                iconButton.onClick.RemoveListener(() => displayThing.Use(user:thingOwner));

                if (thingOwner != null)
                    iconButton.onClick.AddListener(() => displayThing.Use(user:thingOwner));

                iconButton.interactable = true;
            }
        }
    }
}
