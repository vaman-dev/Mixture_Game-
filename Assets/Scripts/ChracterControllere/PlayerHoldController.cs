using UnityEngine;

public class PlayerHoldController : MonoBehaviour
{
    public Transform holdPoint;
    [SerializeField] private float dropForwardDistance = 0.6f;
    public PickupItem CurrentItem { get; private set; }

    public bool HasItem() => CurrentItem != null;

    public void PickItem(PickupItem item)
    {
        CurrentItem = item;
        item.OnPicked(holdPoint);

        Debug.Log($"🟢 Picked: {item.name}");
    }

    public void DropHeldItem()
    {
        if (CurrentItem == null) return;

        Debug.Log($"🔴 Dropping: {CurrentItem.name}");

        Transform dropTransform = holdPoint != null ? holdPoint : transform;
        Vector3 dropPos = dropTransform.position + (dropTransform.forward * dropForwardDistance);
        CurrentItem.OnDropped(dropPos);

        CurrentItem = null;
    }

    public void PlaceItem(PlacementSlot slot)
    {
        if (CurrentItem == null) return;

        Debug.Log($"📦 Sending item to slot: {slot.name}");

        bool placed = slot.PlaceItem(CurrentItem);
        if (placed)
        {
            CurrentItem = null;
        }
        else
        {
            Debug.Log($"⚠️ Place failed, keeping item in hand: {CurrentItem.name}");
        }
    }
}
