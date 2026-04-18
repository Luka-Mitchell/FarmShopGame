using System.Collections.Generic;
using UnityEngine;

public class BuildingDataManager : MonoBehaviour
{
    public static BuildingDataManager Instance;

    public List<BuildingData> buildingDataList; // Assign in inspector
    private Dictionary<string, BuildingData> buildingDataDict;

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
        buildingDataDict = new Dictionary<string, BuildingData>();
        foreach (var data in buildingDataList)
        {
            if (!buildingDataDict.ContainsKey(data.Name))
            {
                buildingDataDict[data.Name] = data;
            }
        }
    }

    public BuildingData GetBuildingData(string name)
    {
        return buildingDataDict.TryGetValue(name, out var data) ? data : null;
    }
}