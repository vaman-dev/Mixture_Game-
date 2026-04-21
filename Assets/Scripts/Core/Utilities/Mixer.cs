using System.Collections.Generic;
using UnityEngine;

public class Mixer : MonoBehaviour
{
    [Header("Slots")]
    public List<PlacementSlot> slots;

    [Header("Output")]
    public Transform outputPoint;

    public void Mix()
    {
        Debug.Log("🟡 Mix Triggered");

        List<ResourceData> inputs = new List<ResourceData>();

        // 🔍 Collect inputs from slots
        foreach (var slot in slots)
        {
            if (slot.isOccupied && slot.currentItem != null)
            {
                var resource = slot.currentItem.GetComponent<ResourceObject>();

                if (resource != null)
                {
                    inputs.Add(resource.data);
                    Debug.Log($"📥 Input: {resource.data.resourceName}");
                }
            }
        }

        // ❌ Need at least 2 inputs
        if (inputs.Count < 2)
        {
            Debug.Log("❌ Need at least 2 items to mix");
            return;
        }

        // 🧪 Craft result
        var result = CraftingManager.Instance.GetResult(inputs[0], inputs[1]);

        if (result != null)
        {
            Debug.Log($"✅ MIX SUCCESS: {inputs[0].resourceName} + {inputs[1].resourceName} = {result.resourceName}");

            if (result.prefab == null)
            {
                Debug.LogError("❌ Result prefab is NULL!");
                return;
            }

            if (outputPoint == null)
            {
                Debug.LogError("❌ Output point not assigned!");
                return;
            }

            // 📦 Spawn result
            Instantiate(result.prefab, outputPoint.position, Quaternion.identity);
            Debug.Log("📦 Output spawned");

            // 🧹 Clear slots
            foreach (var slot in slots)
            {
                if (slot.currentItem != null)
                {
                    Destroy(slot.currentItem.gameObject);
                }

                slot.ClearSlot();
            }
        }
        else
        {
            Debug.Log($"❌ Invalid combination: {inputs[0].resourceName} + {inputs[1].resourceName}");
        }
    }
}