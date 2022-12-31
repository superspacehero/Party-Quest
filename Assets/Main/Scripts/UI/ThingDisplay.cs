using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThingDisplay : MonoBehaviour
{
    public GameThing thing
    {
        set
        {
            UpdateDisplay(value);
        }
    }

    [SerializeField] private UnityEngine.UI.Image iconImage;
    [SerializeField, Space(10)] private TMPro.TextMeshProUGUI nameText;
    [SerializeField] private TMPro.TextMeshProUGUI descriptionText;

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
        }
        else
        {
            if (nameText != null)
                nameText.text = displayThing.thingName;
            if (descriptionText != null)
                descriptionText.text = displayThing.thingDescription;
            if (iconImage != null)
            {
                iconImage.enabled = true;
                iconImage.sprite = displayThing.thingIcon;
                displayThing.SetColors(iconImage);
            }
        }
    }
}
