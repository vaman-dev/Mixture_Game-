using UnityEngine;

public class PlacementSlot : MonoBehaviour
{
    public Transform snapPoint;

    public bool isOccupied;
    public PickupItem currentItem;
    public ResourceData storedData; // 🔥 NEW

    // ✅ SLOT ACCEPTS ANY ITEM NOW
    public bool CanAccept(PickupItem item)
    {
        if (isOccupied)
        {
            Debug.Log("❌ Slot already occupied");
            return false;
        }

        if (item == null) return false;

        // 🔥 NEW: enforce resource requirement
        var resource = item.GetComponent<ResourceObject>();

        if (resource == null || resource.data == null)
        {
            Debug.LogWarning("⚠️ Item is not a valid resource");
            return false;
        }

        return true;
    }

    // ✅ PLACE ITEM
    public bool PlaceItem(PickupItem item)
    {
        if (!CanAccept(item)) return false;
        if (!item.OnPlaced(this)) return false;

        currentItem = item;
        isOccupied = true;

        // 🔥 FETCH DATA FROM RESOURCE OBJECT
        var resource = item.GetComponent<ResourceObject>();
        if (resource != null)
        {
            storedData = resource.data;
            Debug.Log($"📦 Slot filled with: {storedData.resourceName}");
        }
        else
        {
            Debug.LogError("❌ ResourceObject missing on item!");
        }

        return true;
    }

    // ✅ REMOVE ITEM
    public void RemoveItem(PickupItem item)
    {
        if (currentItem == null)
        {
            Debug.LogWarning("⚠️ Slot already empty");
            return;
        }

        // 🔥 SAFE CHECK
        if (currentItem != item)
        {
            Debug.LogWarning("⚠️ Wrong item trying to clear slot");
            return;
        }

        Debug.Log($"🟡 Removing item from slot: {item.name}");

        currentItem = null;
        storedData = null;
        isOccupied = false;
    }

    // ✅ CLEAR SLOT
    public void ClearSlot()
    {
        Debug.Log("🟡 Slot cleared");

        currentItem = null;
        storedData = null;
        isOccupied = false;
    }

    // 🔥 HELPER (VERY USEFUL)
    public ResourceData GetData()
    {
        return storedData;
    }
}