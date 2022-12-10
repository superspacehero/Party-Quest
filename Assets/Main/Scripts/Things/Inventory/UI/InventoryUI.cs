using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public GameObject slotPrefab;
    public Transform slotContainer;

    private void Start()
    {
        PopulateSlots();
    }

    private void PopulateSlots()
    {
        foreach (Inventory.ThingSlot slot in inventory.thingSlots)
        {
            if (Instantiate(slotPrefab, slotContainer).TryGetComponent(out SlotUI slotUI))
                slotUI.Setup(slot);
        }
    }
}
