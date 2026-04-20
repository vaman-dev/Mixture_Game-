using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    public float interactRadius = 2f;
    public LayerMask pickupLayer;
    public LayerMask slotLayer;
    public Transform holdPoint;

    private GameObject heldObject;
    private PlayerInputActions input;

    // Gizmo debug variables
    private Color gizmoColor = Color.red;
    private bool objectFound = false;
    private bool slotFound = false;

    private void Awake()
    {
        input = new PlayerInputActions();
        input.Player.Interact.performed += _ => Interact();
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    void Update()
    {
        // Update gizmo debug information
        UpdateGizmoDebug();
    }

    void UpdateGizmoDebug()
    {
        // Reset flags
        objectFound = false;
        slotFound = false;

        // Check for pickup objects
        Collider[] pickupHits = Physics.OverlapSphere(transform.position, interactRadius, pickupLayer);
        if (pickupHits.Length > 0)
        {
            objectFound = true;
        }

        // Check for slot objects
        Collider[] slotHits = Physics.OverlapSphere(transform.position, interactRadius, slotLayer);
        if (slotHits.Length > 0)
        {
            slotFound = true;
        }

        // Update gizmo color based on detection
        if (slotFound)
        {
            gizmoColor = Color.yellow;  // Yellow: slot found
        }
        else if (objectFound)
        {
            gizmoColor = Color.green;   // Green: object found
        }
        else
        {
            gizmoColor = Color.red;     // Red: nothing found
        }
    }

    void Interact()
    {
        if (heldObject == null)
            TryPickup();
        else
            TryDrop();
    }

    void TryPickup()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRadius, pickupLayer);

        if (hits.Length == 0) return;

        var obj = hits[0];

        if (obj.TryGetComponent<ResourceObject>(out var resource))
        {
            heldObject = obj.gameObject;

            heldObject.transform.SetParent(holdPoint);
            heldObject.transform.localPosition = Vector3.zero;

            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            Debug.Log($"🟢 Picked: {resource.data.resourceName}");
        }
    }

    void TryDrop()
    {
        Debug.Log("🔽 Trying to drop");

        Collider[] hits = Physics.OverlapSphere(transform.position, interactRadius, slotLayer);

        Debug.Log("Slots found: " + hits.Length);

        foreach (var hit in hits)
        {
            Debug.Log("Found slot: " + hit.name);

            if (hit.TryGetComponent<MixSlot>(out var slot))
            {
                if (slot.TryPlace(heldObject))
                {
                    Debug.Log("✅ Placed into slot");

                    heldObject = null;
                    return;
                }
            }
        }

        Debug.Log("❌ No slot found, dropping on ground");
    }

    // Gizmo visualization for debugging
    private void OnDrawGizmos()
    {
        // Set the gizmo color based on current state
        Gizmos.color = gizmoColor;

        // Draw the interaction radius sphere
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}