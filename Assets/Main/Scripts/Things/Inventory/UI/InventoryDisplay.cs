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
    [SerializeField] private GameThing _inventoryOwner;

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

    public GameObject slotPrefab;
    public Transform slotContainer;

    protected List<ThingDisplay> thingDisplays = new List<ThingDisplay>();

    public override void Select()
    {
        base.Select();

        transform.rotation = Quaternion.identity;

        PopulateThings();
    }

    protected virtual void PopulateThings(List<GameThing> excludedThings = null)
    {
        inventory = inventory;

        if (inventory == null)
            return;

        if (excludedThings == null)
            excludedThings = new List<GameThing>();

        if (thingDisplays.Count < inventory.thingSlots.Length)
        {
            while (thingDisplays.Count < inventory.thingSlots.Length)
            {
                if (Instantiate(slotPrefab, slotContainer).TryGetComponent(out ThingDisplay thingDisplay))
                    thingDisplays.Add(thingDisplay);
            }
        }

        for (int i = 0; i < thingDisplays.Count; i++)
        {
            if (i < inventory.thingSlots.Length)
            {
                if (inventory.thingSlots[i].thing != null && excludedThings.Contains(inventory.thingSlots[i].thing))
                    thingDisplays[i].gameObject.SetActive(false);
                else
                    thingDisplays[i].gameObject.SetActive(true);

                thingDisplays[i].thingOwner = inventoryOwner;
                thingDisplays[i].thing = inventory.thingSlots[i].thing;

                thingDisplays[i].gameObject.name = thingDisplays[i].thing == null ? "Empty Slot" : thingDisplays[i].thing.thingName;
            }
            else
                thingDisplays[i].thing = null;
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
}
