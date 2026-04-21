using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    public float pickupRadius = 2f;
    public float slotRadius = 2f;

    public LayerMask pickupLayer;
    public LayerMask slotLayer;

    public Transform pickupPoint;
    public Transform slotPoint;

    public PlayerHoldController holdController;

    private PlayerInputActions input;

    private void Awake()
    {
        input = new PlayerInputActions();
        input.Player.Interact.performed += _ => HandleInteract();
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    void HandleInteract()
    {
        Debug.Log("🟢 Interact pressed");

        if (!holdController.HasItem())
        {
            TryPickup();
        }
        else
        {
            TryPlaceOrDrop();
        }
    }

    void TryPickup()
    {
        Debug.Log("🔍 Searching pickup...");

        Collider[] hits = Physics.OverlapSphere(pickupPoint.position, pickupRadius, pickupLayer);

        PickupItem best = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            var item = hit.GetComponentInParent<PickupItem>();
            if (item == null) continue;

            float dist = Vector3.Distance(pickupPoint.position, item.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                best = item;
            }
        }

        if (best != null)
        {
            holdController.PickItem(best);
        }
        else
        {
            Debug.Log("❌ No pickup found");
        }
    }

    void TryPlaceOrDrop()
    {
        Debug.Log("🔍 Searching slot...");

        Collider[] hits = Physics.OverlapSphere(slotPoint.position, slotRadius, slotLayer);

        PlacementSlot bestSlot = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            var slot = hit.GetComponentInParent<PlacementSlot>();
            if (slot == null) continue;

            Debug.Log($"🎯 Slot candidate: {slot.name}");

            if (!slot.CanAccept(holdController.CurrentItem))
            {
                Debug.Log($"❌ Slot rejected item");
                continue;
            }

            float dist = Vector3.Distance(slotPoint.position, slot.snapPoint.position);
            if (dist < minDist)
            {
                minDist = dist;
                bestSlot = slot;
            }
        }

        if (bestSlot != null)
        {
            Debug.Log($"📦 Placing into slot: {bestSlot.name}");
            holdController.PlaceItem(bestSlot);
        }
        else
        {
            Debug.Log("❌ No valid slot → dropping");
            holdController.DropHeldItem();
        }
    }
}