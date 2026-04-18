using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildingRuntimeState
{
    public int currentQuantity;
    public bool isUnlocked;
    public int BuildLimit;
}

public class BuildingStateManager : MonoBehaviour, IDataPersistence
{
    public static BuildingStateManager Instance;

    private SerializableDictionary<string, BuildingRuntimeState> buildingStates;

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

        buildingStates = new SerializableDictionary<string, BuildingRuntimeState>();
    }

    public BuildingRuntimeState GetBuildingState(string name)
    {
        return buildingStates.TryGetValue(name, out var state) ? state : null;
    }

    public void UnlockBuilding(string name)
    {
        if (buildingStates.TryGetValue(name, out var state))
        {
            state.isUnlocked = true;
        }
        else
        {
            // If not exists, create
            Debug.Log("Unlocking building that doesn't exist in state manager: " + name + ". Adding new state with BuildLimit = 1");
            var data = BuildingDataManager.Instance.GetBuildingData(name);
            buildingStates[name] = new BuildingRuntimeState { currentQuantity = 0, isUnlocked = true, BuildLimit = 1 };
        }
    }

    public void IncrementQuantity(string name)
    {
        if (buildingStates.TryGetValue(name, out var state))
        {
            state.currentQuantity++;
        }
        else
        {
            // If not exists, create
            Debug.Log("Incrementing quantity for building that doesn't exist in state manager: " + name + ". Adding new state with BuildLimit = 1");
            var data = BuildingDataManager.Instance.GetBuildingData(name);
            buildingStates[name] = new BuildingRuntimeState { currentQuantity = 1, isUnlocked = true, BuildLimit = 1 };
        }
    }

    public void SetDevModeData()
    {
        if (BuildingDataManager.Instance != null)
            {
                foreach (var dataItem in BuildingDataManager.Instance.buildingDataList)
                {
                    buildingStates[dataItem.Name] = new BuildingRuntimeState
                    {
                        currentQuantity = 0,
                        isUnlocked = true,
                        BuildLimit = 10
                    };
                }
            }
    }

    // IDataPersistence implementation
    public void LoadData(GameData data)
    {
        if (data.buildingStates != null)
        {
            buildingStates = data.buildingStates;
        }
        else
        {
            // Initialize defaults if no save data or new save data
            if (BuildingDataManager.Instance != null)
            {
                foreach (var dataItem in BuildingDataManager.Instance.buildingDataList)
                {
                    buildingStates[dataItem.Name] = new BuildingRuntimeState
                    {
                        currentQuantity = 0,
                        isUnlocked = false,
                        BuildLimit = 0
                    };
                }
            }
        }
    }

    public void SaveData(GameData data)
    {
        data.buildingStates = buildingStates;
    }
}