using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Alchemy/Recipe")]
public class RecipeData : ScriptableObject
{
    [Header("Inputs")]
    public ResourceData inputA;
    public ResourceData inputB;

    [Header("Output")]
    public ResourceData output;

    // Debug-friendly name
    public string GetRecipeName()
    {
        if (inputA != null && inputB != null && output != null)
        {
            return $"{inputA.resourceName} + {inputB.resourceName} → {output.resourceName}";
        }
        return "Incomplete Recipe";
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (inputA == null || inputB == null || output == null)
            return;

        // ✅ Correct comparison
        if (inputA == inputB)
        {
            Debug.LogWarning($"⚠️ Recipe uses same resource twice: {inputA.resourceName}", this);
        }

        // ✅ Correct comparison
        if (output == inputA || output == inputB)
        {
            Debug.LogWarning($"⚠️ Output is same as input in recipe: {GetRecipeName()}", this);
        }
    }
#endif
}