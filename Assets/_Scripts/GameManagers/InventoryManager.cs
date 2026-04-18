using System.Linq;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine.UI;
//using static UnityEditor.Progress;

[System.Serializable]
public class ItemData
{
    public string itemName;
    public TMPro.TextMeshProUGUI quantityText;
    public Image image;
    public int quantity = 0;
    public bool isUnlocked = false; // Keep for UI state, but derive from FoodStateManager
}

public class InventoryManager : MonoBehaviour, IDataPersistence
{
    public static InventoryManager Instance;

    public Canvas inventoryCanvas;  // The inventory UI Canvas
    public Canvas mainUICanvas;
    public List<ItemData> itemsList;

    private bool isInventoryOpen = false;
    //private Dictionary<string, int> inventoryItems = new Dictionary<string, int>();
    private GameObject gameManager;

    private void Start()
    {
		gameManager = GameObject.FindWithTag("GameManager");
		DayCycleManager.OnDayEnd += ClearDishes;
		CloseInventory();
        SetUpInventory();
	}

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

	private void Update()
    {
        // Toggle inventory when I is pressed
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (isInventoryOpen)
            {
                CloseInventory();
            }
            else if (!GameStateManager.Instance.isUIOpen)
            {
                OpenInventory();
            }
            else
            {
                Debug.Log("A UI is already open, isUIOpen = " + GameStateManager.Instance.isUIOpen);
            }
		}
        if (isInventoryOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInventory();
        }
    }

    public void CloseInventory()
    {
        isInventoryOpen = false;
		inventoryCanvas.gameObject.SetActive(false);
		mainUICanvas.gameObject.SetActive(true);
        GameStateManager.Instance.isUIOpen = false;
        Debug.Log("Closed inventory, isUIOpen = " + GameStateManager.Instance.isUIOpen);
	}

    public void OpenInventory()
    {
        isInventoryOpen = true;
        inventoryCanvas.gameObject.SetActive(true);
		mainUICanvas.gameObject.SetActive(false);
        GameStateManager.Instance.isUIOpen = true;
        Debug.Log("Opened inventory, isUIOpen = " + GameStateManager.Instance.isUIOpen);
	}

    // Call this function whenever resource amount changes
    public void UpdateQuantity(string itemName, int amount)
    {
        ItemData item = itemsList.FirstOrDefault(item => item.itemName == itemName);
        var state = FoodStateManager.Instance.GetFoodState(itemName);
        if (state != null && state.isUnlocked)
        {
			item.quantity += amount;
			item.quantityText.text = item.quantity.ToString();
		}
	}

    public int GetQuantity(string itemName)
    {
		ItemData item = itemsList.FirstOrDefault(item => item.itemName == itemName);
        var state = FoodStateManager.Instance.GetFoodState(itemName);
        if (state == null || !state.isUnlocked)
        {
			Debug.Log("Item Not Unlocked");
            return 0;
        }

		return item.quantity;
    }

    public void UnlockItem(string itemName)
    {
		ItemData item = itemsList.FirstOrDefault(item => item.itemName == itemName);
        Debug.Log("Unlocking food in inventory: " + (item != null ? item.itemName : "Item not found"));
        if (item == null) return;
        item.isUnlocked = true;
        item.image.enabled = true;
        item.quantityText.enabled = true;
        item.quantityText.text = item.quantity.ToString();
	}

    private void ClearDishes()
    {
        if (FoodDataManager.Instance != null)
        {
            foreach (var foodData in FoodDataManager.Instance.foodDataList)
            {
                var state = FoodStateManager.Instance.GetFoodState(foodData.Name);
                if (state != null && state.isUnlocked && foodData.Cost > 0)
                {
                    UpdateQuantity(foodData.Name, -GetQuantity(foodData.Name));
                }
            }
        }
    }

    private void SetUpInventory()
    {
        if (FoodDataManager.Instance != null)
        {
            foreach (var foodData in FoodDataManager.Instance.foodDataList)
            {
                ItemData item = itemsList.FirstOrDefault(item => item.itemName == foodData.Name);
                if (item != null)
                {
                    item.image.sprite = foodData.Sprite;

                    // enable only when the food has already been unlocked
                    var state = FoodStateManager.Instance.GetFoodState(foodData.Name);
                    if (state != null && state.isUnlocked)
                    {
                        item.image.enabled = true;
                        item.quantityText.enabled = true;
                        item.quantityText.text = item.quantity.ToString();
                        item.isUnlocked = true;
                    }
                    else
                    {
                        item.image.enabled = false;
                        item.quantityText.enabled = false;
                    }
                }
            }
        }
	}

    public void LoadData(GameData data)
    {
        if (data.inventoryList == null) return;

        for (int i = 0; i < data.inventoryList.Count; i++)
        {
            itemsList[i].quantity = data.inventoryList[i].quantity;
			itemsList[i].isUnlocked = data.inventoryList[i].isUnlocked;
			
			// Enable UI elements if item is unlocked
			if (itemsList[i].isUnlocked)
			{
				itemsList[i].image.enabled = true;
				itemsList[i].quantityText.enabled = true;
				itemsList[i].quantityText.text = itemsList[i].quantity.ToString();
			}
		}
    }

    public void SaveData(GameData data)
    {
        data.inventoryList = new List<ItemData>();
		for (int i = 0; i < itemsList.Count; i++)
		{
            data.inventoryList.Add(itemsList[i]);
		}
	}
}
