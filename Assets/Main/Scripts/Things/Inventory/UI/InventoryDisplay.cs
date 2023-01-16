using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
    public GameThing inventoryOwner;
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
    [SerializeField] private Inventory _inventory;

    public GameObject slotPrefab;
    public Transform slotContainer;

    protected List<ThingDisplay> thingDisplays = new List<ThingDisplay>();

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        PopulateThings();
    }

    protected virtual void PopulateThings()
    {
        

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
                thingDisplays[i].gameObject.SetActive(true);

                thingDisplays[i].thingOwner = inventoryOwner;
                thingDisplays[i].thing = inventory.thingSlots[i].thing;
            }
            else
                thingDisplays[i].thing = null;
        }
    }
}
