using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance;

    public List<RecipeData> recipes;

    private Dictionary<(string, string), ResourceData> recipeMap;

    private void Awake()
    {
        Instance = this;
        BuildRecipeMap();
    }

    void BuildRecipeMap()
    {
        recipeMap = new Dictionary<(string, string), ResourceData>();

        foreach (var recipe in recipes)
        {
            if (recipe == null || recipe.inputA == null || recipe.inputB == null || recipe.output == null)
            {
                Debug.LogWarning("⚠️ Invalid recipe skipped");
                continue;
            }

            string id1 = recipe.inputA.ID;
            string id2 = recipe.inputB.ID;

            var key = id1.CompareTo(id2) < 0 ? (id1, id2) : (id2, id1);

            if (!recipeMap.ContainsKey(key))
            {
                recipeMap.Add(key, recipe.output);
            }
            else
            {
                Debug.LogError($"❌ Duplicate recipe key: {recipe.GetRecipeName()}");
            }
        }

        Debug.Log($"✅ Recipes Loaded: {recipeMap.Count}");
    }

    public ResourceData GetResult(ResourceData a, ResourceData b)
    {
        if (a == null || b == null)
        {
            Debug.LogWarning("❌ Null input to crafting");
            return null;
        }

        var key = a.ID.CompareTo(b.ID) < 0 ? (a.ID, b.ID) : (b.ID, a.ID);

        if (recipeMap.TryGetValue(key, out var result))
        {
            Debug.Log($"✨ {a.resourceName} + {b.resourceName} = {result.resourceName}");

            InventoryManager.Instance?.Add(result);
            return result;
        }

        Debug.Log($"❌ No recipe for {a.resourceName} + {b.resourceName}");
        return null;
    }
}