using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

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

    [SerializeField, Space(10)] private TMPro.TextMeshProUGUI nameText;
    [SerializeField] private TMPro.TextMeshProUGUI descriptionText;

    [SerializeField] private LocalizationParamsManager localizationParamsManager;

    public void Select()
    {
        if (iconButton != null)
            iconButton.Select();
    }

    private void UpdateDisplay(GameThing displayThing)
    {
        if (displayThing == null)
        {
            if (nameText != null)
                nameText.text = "";
            if (descriptionText != null)
                descriptionText.text = "";
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
            if (localizationParamsManager != null)
                localizationParamsManager.SetParameterValue("THING", displayThing.thingName, true);
            else if (nameText != null)
                nameText.text = displayThing.thingName;
            if (descriptionText != null)
                descriptionText.text = displayThing.thingDescription;
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
