using UnityEngine;
using System.Collections;

public class MixSlot : MonoBehaviour
{
    public Transform snapPoint;
    public float snapSpeed = 10f;

    private GameObject currentObject;
    private ResourceData storedResource;

    public bool IsOccupied => currentObject != null;
    public ResourceData GetResource() => storedResource;

    public bool TryPlace(GameObject obj)
    {
        if (IsOccupied) return false;

        var resourceObj = obj.GetComponent<ResourceObject>();
        if (resourceObj == null) return false;

        storedResource = resourceObj.data;
        currentObject = obj;

        obj.transform.SetParent(snapPoint);
        StartCoroutine(SmoothSnap(obj));

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Debug.Log($"📦 Placed: {storedResource.resourceName}");
        return true;
    }

    public void Clear()
    {
        if (currentObject != null)
        {
            Destroy(currentObject);
        }

        currentObject = null;
        storedResource = null;
    }

    IEnumerator SmoothSnap(GameObject obj)
    {
        float t = 0;
        float duration = 1f / snapSpeed;  // Calculate duration based on snapSpeed

        Vector3 startPos = obj.transform.localPosition;
        Quaternion startRot = obj.transform.localRotation;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            // Use smoothstep for smoother easing (eliminates quick snap)
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            obj.transform.localPosition = Vector3.Lerp(startPos, Vector3.zero, smoothT);
            obj.transform.localRotation = Quaternion.Slerp(startRot, Quaternion.identity, smoothT);

            yield return null;
        }

        // Ensure final position is exact
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
    }
}