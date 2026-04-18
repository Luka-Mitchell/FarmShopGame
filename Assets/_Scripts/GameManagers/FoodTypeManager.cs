using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Obsolete: This class is no longer used. FoodType data is now stored in FoodData, and FoodStateManager tracks unlocks and quantities. */  

[System.Serializable]
public class Ingredient
{
    public string foodName;
    public int quantity;
}

[System.Serializable]
public class FoodType
{
    public string foodName;
    public Ingredient[] ingredients;
    //public FoodType[] ingredients;
    public float cookTime;
    public Sprite foodSprite;
    public bool isUnlocked = false;
    public int foodCost;
    public int foodXP;
}

public class FoodTypeManager : MonoBehaviour
{
    public static FoodTypeManager Instance;
    public List<FoodType> foodTypes;

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


	public FoodType GetFoodType(string name)
    {
        var food = foodTypes.Find(food => food.foodName == name);
        return food;
    }

}
