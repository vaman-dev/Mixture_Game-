using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] public float pickupRadius = 2f;
    [SerializeField] public float slotRadius = 2f;

    [Header("Layer Masks")]
    [SerializeField] public LayerMask pickupLayer;
    [SerializeField] public LayerMask slotLayer;

    [Header("References")]
    [SerializeField] public Transform pickupPoint;
    [SerializeField] public Transform slotPoint;
    [SerializeField] public PlayerHoldController holdController;

    [Header("Gizmo Debug")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color pickupDetectedColor = Color.green;
    [SerializeField] private Color slotDetectedColor = Color.yellow;
    [SerializeField] private Color notDetectedColor = Color.red;

    private PlayerInputActions input;

    // Gizmo debug variables
    private Color pickupGizmoColor = Color.red;
    private Color slotGizmoColor = Color.red;
    private bool objectDetected = false;
    private bool slotDetected = false;

    private void Awake()
    {
        input = new PlayerInputActions();
        input.Player.Interact.performed += _ => HandleInteract();
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    private void Update()
    {
        UpdateGizmoDetection();
        HandleGhostPreview();   // 🔥 ADD THIS
    }

    void UpdateGizmoDetection()
    {
        // Check for pickup objects - origin from player position
        Collider[] pickupHits = Physics.OverlapSphere(transform.position, pickupRadius, pickupLayer);
        objectDetected = pickupHits.Length > 0;
        pickupGizmoColor = objectDetected ? pickupDetectedColor : notDetectedColor;

        // Check for slots - origin from player position
        Collider[] slotHits = Physics.OverlapSphere(transform.position, slotRadius, slotLayer);
        slotDetected = slotHits.Length > 0;
        slotGizmoColor = slotDetected ? slotDetectedColor : notDetectedColor;
    }

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

        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRadius, pickupLayer);

        PickupItem best = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            var item = hit.GetComponentInParent<PickupItem>();
            if (item == null) continue;

            float dist = Vector3.Distance(transform.position, item.transform.position);
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

        Collider[] hits = Physics.OverlapSphere(transform.position, slotRadius, slotLayer);

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

            float dist = Vector3.Distance(transform.position, slot.snapPoint.position);
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

    // 🎨 Gizmo visualization for debugging
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        // Draw pickup detection sphere - origin from player position
        Gizmos.color = pickupGizmoColor;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
        DrawTransparentSphere(transform.position, pickupRadius, pickupGizmoColor);

        // Draw slot detection sphere - origin from player position
        Gizmos.color = slotGizmoColor;
        Gizmos.DrawWireSphere(transform.position, slotRadius);
        DrawTransparentSphere(transform.position, slotRadius, slotGizmoColor);
    }

    // Helper method to draw transparent sphere
    void DrawTransparentSphere(Vector3 position, float radius, Color color)
    {
        Gizmos.color = new Color(color.r, color.g, color.b, 0.1f);

        int segments = 16;
        float angleStep = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * angleStep * Mathf.Deg2Rad;
            float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;

            Vector3 p1 = position + new Vector3(Mathf.Cos(angle1) * radius, 0, Mathf.Sin(angle1) * radius);
            Vector3 p2 = position + new Vector3(Mathf.Cos(angle2) * radius, 0, Mathf.Sin(angle2) * radius);

            Gizmos.DrawLine(p1, p2);
        }
    }
    void HandleGhostPreview()
    {
        // ❌ If no item → no preview
        if (!holdController.HasItem())
        {
            GhostPreview.Instance?.Hide();
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, slotRadius, slotLayer);

        PlacementSlot bestSlot = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            var slot = hit.GetComponentInParent<PlacementSlot>();
            if (slot == null) continue;

            float dist = Vector3.Distance(transform.position, slot.snapPoint.position);

            if (dist < minDist)
            {
                minDist = dist;
                bestSlot = slot;
            }
        }

        if (bestSlot != null)
        {
            var resource = holdController.CurrentItem.GetComponent<ResourceObject>();

            if (resource != null)
            {
                bool isValid = bestSlot.CanAccept(holdController.CurrentItem);

                GhostPreview.Instance?.Show(
                    resource.data,
                    bestSlot.snapPoint,
                    isValid
                );
            }
        }
        else
        {
            GhostPreview.Instance?.Hide();
        }
    }
}