using UnityEngine;

[CreateAssetMenu(menuName = "Data/Building")]
public class BuildingData : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public int Cost { get; private set; }
    [field: SerializeField] public Sprite BuildSprite { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public GameObject Ghost { get; private set; }
    [field: SerializeField] public int GridWidth { get; private set; } = 1;
    [field: SerializeField] public int GridHeight { get; private set; } = 1;
}
