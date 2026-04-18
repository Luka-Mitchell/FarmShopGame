using System.Collections.Generic;
using UnityEngine;

public class FoodDataManager : MonoBehaviour
{
    public static FoodDataManager Instance;

    public List<FoodData> foodDataList; // Assign in inspector
    private Dictionary<string, FoodData> foodDataDict;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Build dictionary for O(1) lookup
        foodDataDict = new Dictionary<string, FoodData>();
        foreach (var data in foodDataList)
        {
            if (!foodDataDict.ContainsKey(data.Name))
            {
                foodDataDict[data.Name] = data;
            }
        }
    }

    public FoodData GetFoodData(string name)
    {
        return foodDataDict.TryGetValue(name, out var data) ? data : null;
    }
}