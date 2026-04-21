using UnityEngine;

public class PlacementSlot : MonoBehaviour
{
    public Transform snapPoint;
    public ItemType acceptedType;

    public bool isOccupied;
    public PickupItem currentItem;

    // ✅ CHECK IF SLOT CAN ACCEPT ITEM
    public bool CanAccept(PickupItem item)
    {
        if (isOccupied)
        {
            Debug.Log("❌ Slot already occupied");
            return false;
        }

        if (item == null) return false;

        return item.itemType == acceptedType || acceptedType == ItemType.None;
    }

    // ✅ PLACE ITEM INTO SLOT
    public bool PlaceItem(PickupItem item)
    {
        if (!CanAccept(item)) return false;
        if (!item.OnPlaced(this)) return false;

        currentItem = item;
        isOccupied = true;

        Debug.Log($"📦 Slot filled: {item.name}");
        return true;
    }

    // ✅ REMOVE ITEM (🔥 CRITICAL FIX)
    public void RemoveItem(PickupItem item)
    {
        // Safety check → only remove if it's the SAME object
        if (currentItem != item)
        {
            Debug.LogWarning("⚠️ Tried removing wrong item from slot");
            return;
        }

        Debug.Log($"🟡 Removing item from slot: {item.name}");

        currentItem = null;
        isOccupied = false;
    }

    // OPTIONAL (force clear)
    public void ClearSlot()
    {
        Debug.Log("🟡 Slot cleared");

        currentItem = null;
        isOccupied = false;
    }
}
