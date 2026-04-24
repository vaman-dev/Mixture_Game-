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

        if (CraftingManager.Instance == null)
        {
            Debug.LogError("❌ CraftingManager missing!");
            return;
        }

        if (outputPoint == null)
        {
            Debug.LogError("❌ OutputPoint not assigned!");
            return;
        }

        List<ResourceData> inputs = new List<ResourceData>();

        foreach (var slot in slots)
        {
            if (slot.isOccupied && slot.GetData() != null)
            {
                inputs.Add(slot.GetData());
                Debug.Log($"📥 Input: {slot.GetData().resourceName}");
            }
        }

        if (inputs.Count < 2)
        {
            Debug.Log("❌ Need at least 2 items");
            return;
        }

        var result = CraftingManager.Instance.GetResult(inputs[0], inputs[1]);

        if (result == null)
        {
            Debug.Log("❌ Invalid combination");
            return;
        }

        if (result.prefab == null)
        {
            Debug.LogError("❌ Result prefab missing!");
            return;
        }

        Instantiate(result.prefab, outputPoint.position, Quaternion.identity);

        Debug.Log("📦 Output spawned");

        foreach (var slot in slots)
        {
            if (slot.currentItem != null)
            {
                Destroy(slot.currentItem.gameObject);
            }

            slot.ClearSlot();
        }
    }
}