using UnityEngine;
using System.Collections.Generic;

public class Mixer : MonoBehaviour
{
    public List<MixSlot> slots;
    public Transform outputPoint;

    public void Mix()
    {
        Debug.Log("🟡 Mix Triggered");

        List<ResourceData> inputs = new List<ResourceData>();

        foreach (var slot in slots)
        {
            if (slot.IsOccupied)
                inputs.Add(slot.GetResource());
        }

        if (inputs.Count < 2)
        {
            Debug.Log("❌ Need at least 2 items");
            return;
        }

        var result = CraftingManager.Instance.GetResult(inputs[0], inputs[1]);

        if (result != null)
        {
            Debug.Log($"✅ Created: {result.resourceName}");

            Instantiate(result.prefab, outputPoint.position, Quaternion.identity);

            foreach (var slot in slots)
                slot.Clear();
        }
        else
        {
            Debug.Log("❌ Invalid combination");
        }
    }
}