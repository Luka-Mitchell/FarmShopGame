using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class BuildingSaveData
{
	public string name;
	public Vector3 location;
	public float productionProgress;
	public string food;

    public BuildingSaveData(string name, Vector3 location, float productionProgress, string food)
    {
        this.name = name;
        this.location = location;
        this.productionProgress = productionProgress;
        this.food = food;
    }
}

public class BuildingManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private Canvas vendorUICanvas;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private GridSystem gridSystem;

    private BuildingData selectedBuildingData;
    private GameObject ghostBuilding;
    private bool isPlacing = false;
    private bool canPlaceBuilding = true;

	void Start()
    {
        FinishBuilding();
    }

    public void SetBuilding(string selectedBuildingName)
    {
		selectedBuildingData = BuildingDataManager.Instance.GetBuildingData(selectedBuildingName);
        var state = BuildingStateManager.Instance.GetBuildingState(selectedBuildingName);

        if (selectedBuildingData != null && state != null && state.isUnlocked && state.currentQuantity < state.BuildLimit)
        {
            ghostBuilding = Instantiate(selectedBuildingData.Ghost);
            //EnsureGhostCollider();
            StartBuilding();
		}
        else
        {
            Debug.LogError("Cannot build more of this building");
        }
    }

    void Update()
    {
        if (isPlacing && ghostBuilding != null)
        {
			gridSystem.SnapToGrid(ghostBuilding, selectedBuildingData.GridWidth, selectedBuildingData.GridHeight);
			canPlaceBuilding = gridSystem.ValidatePlacement(ghostBuilding, selectedBuildingData.GridWidth, selectedBuildingData.GridHeight);

            if (Input.GetMouseButtonDown(0) && canPlaceBuilding)
            {
                PlaceBuilding();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelPlacement();
            }
        }
    }

    void PlaceBuilding()
    {
        var state = BuildingStateManager.Instance.GetBuildingState(selectedBuildingData.Name);
        if (state.currentQuantity >= state.BuildLimit)
        {
            Debug.Log("Amount exceeded: " + state.currentQuantity + "/" + state.BuildLimit);
            return;
        }

        if (ResourceManager.Instance.GetGoldAmount() < selectedBuildingData.Cost)
        {
            Debug.Log("Not enough gold to place this building!");
            return;
        }

        if (canPlaceBuilding && selectedBuildingData != null)
        {
			GameObject actualBuilding = Instantiate(selectedBuildingData.Prefab);
            actualBuilding.transform.position = ghostBuilding.transform.position;

			DeductResources(selectedBuildingData.Cost);
            BuildingStateManager.Instance.IncrementQuantity(selectedBuildingData.Name);
            Destroy(ghostBuilding);
            FinishBuilding();
        }
    }

    void CancelPlacement()
    {
        if (ghostBuilding != null)
        {
            Destroy(ghostBuilding);
        }

        FinishBuilding();
    }

    private void StartBuilding()
    {
        isPlacing = true;
        gridSystem.ShowGrid();
        if (cellIndicator != null) cellIndicator.SetActive(true);

        if (vendorUICanvas != null)
        {
            GameStateManager.Instance.isUIOpen = false;
            vendorUICanvas.gameObject.SetActive(false);
            Debug.Log("isUIOpen = " + GameStateManager.Instance.isUIOpen);
        }
	}

    private void FinishBuilding()
    {
        gridSystem.HideGrid();

        if (cellIndicator != null) cellIndicator.SetActive(false);
        selectedBuildingData = null;
        ghostBuilding = null;
        isPlacing = false;
	}

/*
	private void EnsureGhostCollider()
    {
        if (ghostBuilding.GetComponent<Collider2D>() == null)
        {
            BoxCollider2D collider = ghostBuilding.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }
    }
*/

    void DeductResources(int cost)
    {
        if (ResourceManager.Instance.GetGoldAmount() >= cost)
        {
            ResourceManager.Instance.UpdateGoldAmount(-cost);
        }
        else
        {
            Debug.Log("Not enough resources!");
        }
    }

    public void LoadData(GameData data)
    {
        Debug.Log("Trying to Instantiate all buildings in save data");
        if (data.buildingsDict == null) return;

        // instantiate each building
        foreach (KeyValuePair<string, BuildingSaveData> buildingPair in data.buildingsDict)
        {
            if (buildingPair.Value.name == "River") { Debug.Log("Not Instantiating River Object"); continue; }
            var buildingData = BuildingDataManager.Instance.GetBuildingData(buildingPair.Value.name);
            if (buildingData != null)
            {
                GameObject building = Instantiate(buildingData.Prefab);
                BuildingStateManager.Instance.IncrementQuantity(buildingPair.Value.name);
                Debug.Log("Instantiated " + buildingPair.Value.name + " from save data");
                building.GetComponent<IHasID>().ID = buildingPair.Key;
                building.GetComponent<IDataPersistence>().LoadData(data);
            }
		}
    }

    public void SaveData(GameData data)
    {
        Debug.Log("SaveData called from BuildingManager");
        // saving is done from each individual building
    }
}
