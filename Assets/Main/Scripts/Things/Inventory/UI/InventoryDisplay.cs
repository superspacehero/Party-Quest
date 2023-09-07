using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : Menu
{
    public GameThing inventoryOwner
    {
        get
        {
            if (_inventoryOwner != null)
                return _inventoryOwner;

            _inventoryOwner = GetComponentInParent<CharacterThing>();

            return _inventoryOwner;
        }
        set => _inventoryOwner = value;
    }
    [SerializeField] protected GameThing _inventoryOwner;

    public virtual Inventory inventory
    {
        get
        {
            if (_inventory != null)
                return _inventory;

            if (inventoryOwner != null)
                inventoryOwner.TryGetComponent(out _inventory);
            else
                _inventory = GetComponentInParent<Inventory>();

            return _inventory;
        }
        set => _inventory = value;
    }
    [SerializeField] protected Inventory _inventory;

    public Transform slotContainer;

    protected List<ThingDisplay> thingDisplays = new List<ThingDisplay>();

    public void Select(GameThing owner)
    {
        base.Select();

        transform.rotation = Quaternion.identity;

        PopulateThings(null, owner);
    }

    protected virtual void PopulateThings(List<GameThing> excludedThings = null, GameThing overrideOwner = null)
    {
        inventory = inventory;

        if (inventory == null)
            return;

        if (excludedThings == null)
            excludedThings = new List<GameThing>();

        if (overrideOwner == null)
            overrideOwner = inventoryOwner;

        // if (thingDisplays.Count < inventory.thingSlots.Length)
        // {
        //     while (thingDisplays.Count < inventory.thingSlots.Length)
        //     {
        //         ThingDisplay thingDisplay = GameManager.GetThingDisplay(slotContainer);
        //         thingDisplays.Add(thingDisplay);
        //     }
        // }

        foreach (ThingDisplay thingDisplay in thingDisplays)
        {
            thingDisplay.gameObject.SetActive(false);
        }

        thingDisplays.Clear();

        for (int i = 0; i < inventory.thingSlots.Length; i++)
        {
            // if (i < inventory.thingSlots.Length)
            // {
                ThingDisplay thingDisplay = GameManager.GetThingDisplay(slotContainer);

                if (inventory.thingSlots[i].thing != null && excludedThings.Contains(inventory.thingSlots[i].thing))
                {
                    thingDisplay.gameObject.SetActive(false);
                    continue;
                }
                // else
                //     thingDisplays[i].gameObject.SetActive(true);

                thingDisplay.SetThing(inventory.thingSlots[i].thing, overrideOwner);

                thingDisplay.gameObject.name = thingDisplay.thing == null ? "Empty Slot" : thingDisplay.thing.thingName;

                thingDisplays.Add(thingDisplay);
            // }
            // else
            //     thingDisplays[i].thing = null;
        }

        foreach (ThingDisplay thingDisplay in thingDisplays)
        {
            if (thingDisplay.iconButton != null && thingDisplay.iconButton.interactable == true)
            {
                thingDisplay.iconButton.Select();
                break;
            }
        }
    }

    public void SetThingOwner(GameThing thingOwner)
    {
        foreach (ThingDisplay thingDisplay in thingDisplays)
        {
            thingDisplay.SetThing(null, thingOwner);
        }
    }
}
