using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour, IDataPersistence
{
    public static ResourceManager Instance; // Singleton instance for easy access

    [SerializeField] private TextMeshProUGUI goldText; // Reference to the UI text
    private int goldAmount = 100; // Player's current gold

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensure there's only one instance
        }
    }

    private void Start()
    {
        UpdateGoldUI(); // Initialize the UI
    }

    public int GetGoldAmount()
    {
        return goldAmount;
    }
    
    public void UpdateGoldAmount(int amount)
    {
        goldAmount += amount;
        UpdateGoldUI();
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = $"Gold: {goldAmount}";
        }
    }

    public void LoadData(GameData data)
    {
        this.goldAmount = data.gold;
    }

    public void SaveData(GameData data)
    {
        data.gold = this.goldAmount;
    }
}
