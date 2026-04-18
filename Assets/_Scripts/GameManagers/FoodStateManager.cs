using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FoodRuntimeState
{
    public bool isUnlocked;
}

public class FoodStateManager : MonoBehaviour, IDataPersistence
{
    public static FoodStateManager Instance;

    private SerializableDictionary<string, FoodRuntimeState> foodStates;

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

        foodStates = new SerializableDictionary<string, FoodRuntimeState>();
    }

    public FoodRuntimeState GetFoodState(string name)
    {
        return foodStates.TryGetValue(name, out var state) ? state : null;
    }

    public void UnlockFood(string name)
    {
        if (foodStates.TryGetValue(name, out var state))
        {
            state.isUnlocked = true;
        }
        else
        {
            // If not exists, create
            foodStates[name] = new FoodRuntimeState { isUnlocked = true };
        }
    }

    public void SetDevModeData()
    {
        if (FoodDataManager.Instance != null)
            {
                foreach (FoodData dataItem in FoodDataManager.Instance.foodDataList)
                {
                    UnlockFood(dataItem.Name);
                    InventoryManager.Instance.UnlockItem(dataItem.Name);
                }
            }
    }

    // IDataPersistence implementation
    public void LoadData(GameData data)
    {
        if (data.foodStates != null)
        {
            foodStates = data.foodStates;
        }
        else
        {
            // Initialize defaults if no save data or new save data
            if (FoodDataManager.Instance != null)
            {
                foreach (var dataItem in FoodDataManager.Instance.foodDataList)
                {
                    foodStates[dataItem.Name] = new FoodRuntimeState
                    {
                        isUnlocked = false
                    };
                }
            }
        }
    }

    public void SaveData(GameData data)
    {
        data.foodStates = foodStates;
    }
}