using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [Header("Physics")]
    public Rigidbody rb;
    public Collider[] colliders;

    [Header("Data")]
    //public ItemType itemType;
    public ResourceObject resourceObject; // 🔥 IMPORTANT (for Mixer)

    [Header("State")]
    public bool isHeld;
    public bool isPlaced;
    private PlacementSlot currentSlot;

    // 🟢 PICK
    public void OnPicked(Transform holdPoint)
    {
        Debug.Log($"👉 Item picked: {name}");

        if (currentSlot != null)
        {
            currentSlot.RemoveItem(this);
            currentSlot = null;
        }
        else
        {
            // Fallback for legacy scene hierarchies
            var slot = GetComponentInParent<PlacementSlot>();
            if (slot != null)
            {
                slot.RemoveItem(this);
            }
        }

        isHeld = true;
        isPlaced = false;

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        SetPhysics(false);
    }

    // 🔴 DROP
    public void OnDropped(Vector3 dropPos)
    {
        if (!isHeld && !isPlaced)
        {
            Debug.LogWarning($"⚠️ Drop ignored (not held): {name}");
            return;
        }

        Debug.Log($"🔴 DROP → {name}");

        isHeld = false;
        isPlaced = false;
        currentSlot = null;

        transform.SetParent(null);
        transform.position = dropPos;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        SetPhysics(true);
    }

    // 📦 PLACE INTO SLOT
    public bool OnPlaced(PlacementSlot slot)
    {
        if (slot == null || slot.snapPoint == null)
        {
            Debug.LogWarning($"⚠️ Invalid slot while placing: {name}");
            return false;
        }

        Debug.Log($"📦 PLACE → {name} into {slot.name}");

        isHeld = false;
        isPlaced = true;
        currentSlot = slot;

        transform.SetParent(slot.snapPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        SetPhysics(false);
        return true;
    }

    // 🔄 RESET (VERY IMPORTANT for reuse)
    public void ResetItem()
    {
        Debug.Log($"🔄 RESET → {name}");

        isHeld = false;
        isPlaced = false;
        currentSlot = null;

        transform.SetParent(null);

        SetPhysics(true);
    }

    // ⚙️ PHYSICS CONTROL
    void SetPhysics(bool state)
    {
        if (rb != null)
        {
            rb.isKinematic = !state;
            rb.useGravity = state;
            rb.collisionDetectionMode = state
                ? CollisionDetectionMode.ContinuousDynamic
                : CollisionDetectionMode.Discrete;
        }

     
    }

    // 🔍 DEBUG HELP
    private void OnValidate()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (colliders == null || colliders.Length == 0)
            colliders = GetComponentsInChildren<Collider>();

        if (resourceObject == null)
            resourceObject = GetComponent<ResourceObject>();
    }
}

// 🔥 ITEM TYPE ENUM
public enum ItemType
{
    None,
    Fire,
    Water,
    Earth,
    Air,
    Steam
      
}
