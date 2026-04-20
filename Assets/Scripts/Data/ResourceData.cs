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
        if (string.IsNullOrEmpty(guid))
        {
            guid = Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}