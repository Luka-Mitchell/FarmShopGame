using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour, IDataPersistence
{
	public static GameStateManager Instance;

	public int playerLevel = 1;
    public int xp = 0;
    //public int initialXpToNextLevel = 42;
    public int xpToNextLevel = 0;
    public bool isUIOpen = false;

    public int totalCustomers=0;
    public int servedCustomers=0;

    [SerializeField] private TMPro.TextMeshProUGUI xpText;
    [SerializeField] private TMPro.TextMeshProUGUI levelText;

	public delegate void LevelUpEvent();
	public static event LevelUpEvent OnLevelUp;

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

	private void OnEnable()
    {
        DayCycleManager.OnDayEnd += CalculateStarRating;
    }

    private void OnDisable()
    {
        DayCycleManager.OnDayEnd -= CalculateStarRating;
    }

    private void CalculateStarRating()
    {
        if (totalCustomers == 0) return;
        else DayCycleManager.Instance.shopStarRating = (float)servedCustomers / totalCustomers * 5;

        /*
        float satisfactionRate = (float)servedCustomers / totalCustomers;
        if (satisfactionRate >= 0.9f) DayManager.Instance.shopStarRating = 5;
        else if (satisfactionRate >= 0.75f) DayManager.Instance.shopStarRating = 4;
        else if (satisfactionRate >= 0.5f) DayManager.Instance.shopStarRating = 3;
        else if (satisfactionRate >= 0.25f) DayManager.Instance.shopStarRating = 2;
        else DayManager.Instance.shopStarRating = 1;
        */

        Debug.Log("Star Rating: " + DayCycleManager.Instance.shopStarRating);
    }

    private void Start()
    {
        //xpToNextLevel = initialXpToNextLevel;
        GameStartUnlocks();
    }

    public void AddXP(int amount)
    {
        xp += amount;
        if (xp >= xpToNextLevel)
        {
            LevelUp();
        }
        UpdateUI();
    }

    private void LevelUp()
    {
        playerLevel++;
        xp -= xpToNextLevel;
        xpToNextLevel = LevelUpUnlockManager.Instance.levelUnlocks[playerLevel].xpToNextLevel;
		//Mathf.RoundToInt(xpToNextLevel * 1.2f); // Increase XP threshold
		LevelUpUnlockManager.Instance.UnlockLevel(playerLevel, false);
        OnLevelUp?.Invoke(); 
		Debug.Log("Level Up! New Level: " + playerLevel);
    }

    private void GameStartUnlocks()
    {
        if (playerLevel == 1)
        {
			xpToNextLevel = LevelUpUnlockManager.Instance.levelUnlocks[playerLevel].xpToNextLevel;
			LevelUpUnlockManager.Instance.UnlockLevel(playerLevel, true);
		}
		UpdateUI();
	}

    private void UpdateUI()
    {
        xpText.text = "XP: " + xp + "/" + xpToNextLevel;
        levelText.text = "Level: " + playerLevel;
    }

    public void LoadData(GameData data)
    {
        this.playerLevel = data.playerLevel;
        this.xp = data.xp;
    }

    public void SaveData(GameData data)
    {
        data.playerLevel = this.playerLevel;
        data.xp = this.xp;
    }

}
