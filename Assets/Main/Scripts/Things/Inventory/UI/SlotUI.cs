using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;

    public void Setup(Inventory.ThingSlot slot)
    {
        iconImage.sprite = slot.thing.thingIcon;
        nameText.text = slot.thing.thingName;
    }
}
