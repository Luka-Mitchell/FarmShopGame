using UnityEngine;
using UnityEngine.UI;

public class FoodButton : MonoBehaviour
{
    public string foodName;
    public TMPro.TextMeshProUGUI nameText;
    public Image image;
	public Image backgroundImage;
	public TMPro.TextMeshProUGUI[] ingredientTexts; // must be in same order as ingredients in FoodType recipe, should fix
    public FoodPrepManager foodManager;

	private bool isUnlocked = false;
	private GameObject gameManager;
    private FoodData food;
    private InventoryManager inventory;

	private void OnEnable()
	{
        GameStateManager.OnLevelUp += CheckForUnlock;
	}

    private void OnDisable()
    {
		GameStateManager.OnLevelUp -= CheckForUnlock;
	}

	private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
        food = FoodDataManager.Instance.GetFoodData(foodName);
        image.sprite = food.Sprite;
        nameText.text = foodName;
        CheckForUnlock();
	}

    void Update()
    {
        if (!FoodStateManager.Instance.GetFoodState(foodName).isUnlocked) { return; }
        for (int i = 0; i < ingredientTexts.Length; i++)
        {
            ingredientTexts[i].text = food.Ingredients[i].foodName + ": " + InventoryManager.Instance.GetQuantity(food.Ingredients[i].foodName) + "/" + food.Ingredients[i].quantity;
        }
    }

    private void OnButtonClick()
    {
        foodManager.SelectFoodToCook(food);
    }

    private void CheckForUnlock()
    {
        isUnlocked = FoodStateManager.Instance.GetFoodState(foodName).isUnlocked;
		nameText.enabled = isUnlocked;
		image.enabled = isUnlocked;
		backgroundImage.enabled = isUnlocked;
		foreach (var text in ingredientTexts)
		{
			text.enabled = isUnlocked;
		}
    }
}
