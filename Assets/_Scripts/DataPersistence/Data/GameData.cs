using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameData
{
    // game data variables
    public int playerLevel;
    public int xp;

    public int gold;

    public int day;
    public float currentTime;
    public bool isDayActive;

    public Vector3 playerPosition;

    public List<ItemData> inventoryList;
    public SerializableDictionary<string, BuildingRuntimeState> buildingStates;
    public SerializableDictionary<string, FoodRuntimeState> foodStates;
    public SerializableDictionary<string, BuildingSaveData> buildingsDict;
    public List<CustomerData> customerList;
    public float lastSpawnTime;


	public GameData()
    {
        // initialize new game data with default initial values
        playerLevel = 1;
        xp = 0;
        gold = 100;
        day = 0;
        currentTime = 0;
        isDayActive = true;
        playerPosition = new Vector3(10, 0, 0);
        inventoryList = null;      //new List<ItemData>();
        buildingsDict = new SerializableDictionary<string, BuildingSaveData>();
        buildingStates = null;     //new SerializableDictionary<string, BuildingRuntimeState>();
        foodStates = null;         //new SerializableDictionary<string, FoodRuntimeState>();
        customerList = new List<CustomerData>();
        lastSpawnTime = 0;
    }

    // constructor with inf gold for testing purposes
    public GameData(int devMode) : this()
    {
        gold = 99999;
        playerLevel = 10;
    }
}
