using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildUnlockEntry
{
    public BuildingData buildingData;
    public int newLimit;
}

[CreateAssetMenu(menuName = "Data/Level Unlock")]
public class LevelUnlockData : ScriptableObject
{
    public int levelNumber;
    public int xpToNextLevel;
    public List<BuildUnlockEntry> buildUnlocks;
    public List<FoodData> foodUnlocks;
}
