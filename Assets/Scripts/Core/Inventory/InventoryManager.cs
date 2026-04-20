using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private HashSet<ResourceData> discovered = new HashSet<ResourceData>();

    private void Awake()
    {
        Instance = this;
    }

    public void Add(ResourceData resource)
    {
        if (resource == null) return;

        if (discovered.Add(resource))
        {
            Debug.Log("✅ New Resource Discovered: " + resource.resourceName);
        }
        else
        {
            Debug.Log("⚠️ Already discovered: " + resource.resourceName);
        }
    }

    public bool Contains(ResourceData resource)
    {
        return discovered.Contains(resource);
    }
}