using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelUpUnlockManager : MonoBehaviour
{
	public static LevelUpUnlockManager Instance;

	public List<LevelUnlockData> levelUnlocks;

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
	}

	public void UnlockLevel(int levelNumber, bool gameStart)
	{
		LevelUnlockData level = levelUnlocks[levelNumber];

		// Unlock the Builds
		foreach (BuildUnlockEntry buildUnlock in level.buildUnlocks)
		{
			if (buildUnlock == null || buildUnlock.buildingData == null) continue;
			var data = buildUnlock.buildingData;

			var state = BuildingStateManager.Instance.GetBuildingState(data.Name);
			if (state == null || !state.isUnlocked)
			{
				BuildingStateManager.Instance.UnlockBuilding(data.Name);
				if (!gameStart)
				{
					LevelupUIManager.Instance.SetBuildImages(data.BuildSprite);
				}
			}
			else if (!gameStart)
			{
				LevelupUIManager.Instance.SetBuildLimitImages(data.BuildSprite);
			}
			// Update maxQuantity
			if (state != null)
			{
				state.BuildLimit = buildUnlock.newLimit;
			}
		}

		// Unlock the foods
		foreach (FoodData food in level.foodUnlocks)
		{
			if (food == null) continue;
			FoodStateManager.Instance.UnlockFood(food.Name);
			InventoryManager.Instance.UnlockItem(food.Name);
			if (!gameStart)
			{
				LevelupUIManager.Instance.SetFoodImages(food.Sprite);
			}
		}

		if (!gameStart)
		{
			LevelupUIManager.Instance.SetNewLevel(levelNumber);
			LevelupUIManager.Instance.ShowUnlockUI();
		}
	}
}
