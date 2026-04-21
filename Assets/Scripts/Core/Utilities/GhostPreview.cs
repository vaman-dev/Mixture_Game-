using UnityEngine;

public class GhostPreview : MonoBehaviour
{
    public static GhostPreview Instance;

    [Header("Materials")]
    public Material ghostMaterial;

    [Header("Colors")]
    public Color validColor = new Color(0f, 1f, 0f, 0.3f);   // Green
    public Color invalidColor = new Color(1f, 0f, 0f, 0.3f); // Red

    public float ghostScaleOffset = 1.02f;

    private GameObject currentGhost;
    private ResourceData currentData;
    private Transform currentTarget;

    private Renderer[] ghostRenderers;

    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if (currentGhost != null)
        {
            float pulse = Mathf.Sin(Time.time * 6f) * 0.05f + 1f;
            currentGhost.transform.localScale = Vector3.one * ghostScaleOffset * pulse;
        }
    }

    // 👻 Show preview
    public void Show(ResourceData data, Transform target, bool isValid)
    {
        if (data == null || data.prefab == null || target == null)
            return;

        // Prevent unnecessary recreation
        if (currentGhost != null && currentData == data && currentTarget == target)
        {
            UpdateColor(isValid);
            return;
        }

        Hide();

        currentData = data;
        currentTarget = target;

        currentGhost = Instantiate(data.prefab, target.position, target.rotation);

        SetupGhost(currentGhost);

        currentGhost.transform.SetParent(target);
        currentGhost.transform.localPosition = Vector3.zero;
        currentGhost.transform.localRotation = Quaternion.identity;
        currentGhost.transform.localScale *= ghostScaleOffset;

        UpdateColor(isValid);
    }

    // 🎨 Setup ghost visuals
    void SetupGhost(GameObject obj)
    {
        ghostRenderers = obj.GetComponentsInChildren<Renderer>();

        foreach (var r in ghostRenderers)
        {
            if (ghostMaterial != null)
                r.material = new Material(ghostMaterial); // unique instance
        }

        foreach (var col in obj.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        var rb = obj.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    public void UpdateColor(bool isValid)
    {
        if (ghostRenderers == null) return;

        Color targetColor = isValid ? validColor : invalidColor;

        foreach (var r in ghostRenderers)
        {
            foreach (var mat in r.materials)
            {
                // Get current color
                Color current = mat.GetColor("_BaseColor");

                // Smooth transition
                Color newColor = Color.Lerp(current, targetColor, Time.deltaTime * 10f);

                mat.SetColor("_BaseColor", newColor);
            }
        }
    }

    // ❌ Hide
    public void Hide()
    {
        if (currentGhost != null)
        {
            Destroy(currentGhost);
        }

        currentGhost = null;
        currentData = null;
        currentTarget = null;
        ghostRenderers = null;
    }
}