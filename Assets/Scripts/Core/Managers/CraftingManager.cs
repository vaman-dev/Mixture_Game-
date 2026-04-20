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
            string id1 = recipe.inputA.ID;
            string id2 = recipe.inputB.ID;

            var key = id1.CompareTo(id2) < 0 ? (id1, id2) : (id2, id1);

            recipeMap[key] = recipe.output;
        }
    }

    public ResourceData GetResult(ResourceData a, ResourceData b)
    {
        var key = a.ID.CompareTo(b.ID) < 0 ? (a.ID, b.ID) : (b.ID, a.ID);

        if (recipeMap.TryGetValue(key, out var result))
        {
            InventoryManager.Instance?.Add(result);
            return result;
        }

        return null;
    }
}