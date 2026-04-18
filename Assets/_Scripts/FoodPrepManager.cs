using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FoodPrepManager : MonoBehaviour, IDataPersistence, IHasID
{
    public Canvas cookingMenuCanvas;  // UI for selecting food options
    public Image cookingGauge;  // Timer gauge for cooking progress
    public GameObject gaugeCanvas;
    public float interactionDistance = 5f;
    public GameObject outline = null;
	public string buildingName;

	public string ID { get; set; }

	protected GameObject gameManager;
    protected bool isCooking = false;
    protected bool foodReady = false;
    //private float cookingTime = 5f;  // Adjust based on recipe
    protected float cookingProgress = 0f;
    protected FoodData selectedFood = null;
    protected InventoryManager inventory;
    protected GameObject player;
	//private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		ID = System.Guid.NewGuid().ToString();
		Debug.Log("Set ID for " + buildingName + ": " + ID);

		cookingMenuCanvas.gameObject.SetActive(false);  // Hide menu initially
		cookingGauge.fillAmount = 0f;
		gaugeCanvas.SetActive(false);
	}

	private void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager");
        if (outline != null) { outline.SetActive(false); }
        inventory = gameManager.GetComponent<InventoryManager>();
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCookingMenu();
        }

        if (PlayerInRange())
        {
            // highlight green
            //spriteRenderer.color = new Color(0.7f, 1f, 0.7f, 1f); //light green
            if (outline != null) { outline.SetActive(true); }

            if (Input.GetMouseButtonDown(0)) // 0 is for left mouse button
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                int ovenLayerMask = LayerMask.GetMask("Clickable");
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, ovenLayerMask);

                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    // Oven has been clicked
                    OnOvenClicked();
                }
            }
        }
        else
        {
            //spriteRenderer.color = new Color(1f, 1f, 1f, 1f); //white
            if (outline != null) { outline.SetActive(false); }
        }

        if (isCooking && DayCycleManager.Instance.IsDayActive)
        {
            cookingProgress += Time.deltaTime / selectedFood.CookTime;
            cookingGauge.fillAmount = cookingProgress;

            if (cookingProgress >= 1f)
            {
                cookingProgress = 1f;
                isCooking = false;
                foodReady = true;
                //Debug.Log(selectedFood.foodName + " is ready!");
            }
        }
    }

    private bool PlayerInRange()
    {
        return (Vector3.Distance(transform.position, player.transform.position) <= interactionDistance);
    }

    private void OnOvenClicked()
    {
        // Code that you want to execute when the oven is clicked
        if (foodReady)
        {
            PickUpFood();
        }
        //else if (!isCooking)
        else if (!isCooking && !GameStateManager.Instance.isUIOpen)
        {
            OpenCookingMenu();
        }
        else if (GameStateManager.Instance.isUIOpen)
        {
            Debug.Log("A UI is already open, isUIOpen = " + GameStateManager.Instance.isUIOpen);
        }
    }

    private void OpenCookingMenu()
    {
        cookingMenuCanvas.gameObject.SetActive(true);
        GameStateManager.Instance.isUIOpen = true;
        Debug.Log("Opened cooking menu, isUIOpen = " + GameStateManager.Instance.isUIOpen);
    }

    public void CloseCookingMenu()
    {
        cookingMenuCanvas.gameObject.SetActive(false);
        GameStateManager.Instance.isUIOpen = false;
        Debug.Log("Closed cooking menu, isUIOpen = " + GameStateManager.Instance.isUIOpen);
    }

    public void SelectFoodToCook(FoodData food)
    {
        bool hasIngredients = true;
        // check if we have the required ingredients
        for (int i = 0; i < food.Ingredients.Length; i++)
        {
            if (inventory.GetQuantity(food.Ingredients[i].foodName) < food.Ingredients[i].quantity)
            {
                hasIngredients = false;
            }
        }

        if (hasIngredients && DayCycleManager.Instance.IsDayActive)
        {
            selectedFood = food;
            cookingProgress = 0f;
            cookingGauge.fillAmount = 0f;
            StartCooking();

            // take required ingredients from inventory
            for (int i = 0; i < food.Ingredients.Length; i++)
            {
                inventory.UpdateQuantity(food.Ingredients[i].foodName, -food.Ingredients[i].quantity);
            }

            cookingMenuCanvas.gameObject.SetActive(false);  // Close menu after selection
            GameStateManager.Instance.isUIOpen = false;
            Debug.Log("Started cooking, isUIOpen = " + GameStateManager.Instance.isUIOpen);
        }
        else
        {
            Debug.Log("Error: don't have required ingredients");
        }
        
    }


    private void StartCooking()
    {
        //Debug.Log("Cooking " + selectedFood.foodName);
        isCooking = true;
        foodReady = false;
        gaugeCanvas.SetActive(true);
    }

    protected virtual void PickUpFood()
    {
        gameManager.GetComponent<InventoryManager>().UpdateQuantity(selectedFood.Name, 1);
        //Debug.Log("Collected: " + selectedFood.foodName);
        foodReady = false;
        cookingProgress = 0f;
        cookingGauge.fillAmount = 0f;
        selectedFood = null;
        gaugeCanvas.SetActive(false);
    }

    public void LoadData(GameData data)
    {
		data.buildingsDict.TryGetValue(ID, out BuildingSaveData buildingData);
		if (buildingData != null)
		{
			Debug.Log("Loading Building Data: " + buildingName);
			this.transform.position = buildingData.location;
			this.cookingProgress = buildingData.productionProgress;
			cookingGauge.fillAmount = cookingProgress;
			if (buildingData.food != null)
            {
				this.selectedFood = FoodDataManager.Instance.GetFoodData(buildingData.food);
			}
            // infer and set up production state
			if (selectedFood != null)
            {
				gaugeCanvas.SetActive(true);
				foodReady = false;
				isCooking = true;
			}
            else
            {
                this.isCooking = false;
                this.foodReady = false;
				gaugeCanvas.SetActive(false);
			}
		}
		else
		{
			Debug.LogError("building with ID {" + ID + "} could not be found in save data");
		}
	}

    public void SaveData(GameData data)
    {
		if (data.buildingsDict.ContainsKey(ID))
		{
			data.buildingsDict.Remove(ID);
		}

		data.buildingsDict.Add(ID, new BuildingSaveData(buildingName, this.transform.position, cookingProgress, selectedFood != null ? selectedFood.Name : null));
		Debug.Log("Saving Building Data: " + buildingName);
	}
}

