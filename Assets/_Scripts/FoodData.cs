using UnityEngine;

[CreateAssetMenu(menuName = "Data/Food")]
public class FoodData : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Ingredient[] Ingredients { get; private set; }
    [field: SerializeField] public float CookTime { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public int Cost { get; private set; }
    [field: SerializeField] public int XP { get; private set; }
}