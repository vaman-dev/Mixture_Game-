using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Resource", menuName = "Alchemy/Resource")]
public class ResourceData : ScriptableObject
{
    public string resourceName;
    public GameObject prefab;

    [SerializeField, HideInInspector]
    private string guid;

    public string ID => guid;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Generate if missing
        if (string.IsNullOrEmpty(guid))
        {
            guid = Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }

    private void OnEnable()
    {
        CheckDuplicateID();
    }

    private void CheckDuplicateID()
    {
        var all = Resources.FindObjectsOfTypeAll<ResourceData>();

        foreach (var other in all)
        {
            if (other != this && other.ID == this.ID)
            {
                Debug.LogWarning($"⚠️ Duplicate ID detected on {name}. Regenerating...");

                guid = Guid.NewGuid().ToString();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
    }

    // 🧪 Manual fix button
    [ContextMenu("Force Regenerate ID")]
    public void ForceRegenerateID()
    {
        guid = Guid.NewGuid().ToString();
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"🔄 New ID generated for {resourceName}");
    }
#endif
}