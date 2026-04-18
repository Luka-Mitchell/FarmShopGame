using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelupUIManager : MonoBehaviour
{
    public static LevelupUIManager Instance;

    public GameObject unlockUI; // Reference to the UI object
    public TMPro.TextMeshProUGUI titleText;
    public List<Image> unlockedFoods;  // List for unlocked tools
    public List<Image> unlockedBuilds;  // List for unlocked ingredients
    public List<Image> unlockedBuildLimits;  // List for unlocked recipes

    private int unlockedFoodsCount = 0;
    private int unlockedBuildsCount = 0;
    private int unlockedBuildLimitsCount = 0;

    private bool isUIActive = false;

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

	private void Start()
    {
        unlockUI.SetActive(false); // Ensure the UI is off at the start
    }

    private void Update()
    {
        if (isUIActive && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseUnlockUI();
        }
    }

    // Call this method when the player levels up
    public void ShowUnlockUI()
    {
        unlockUI.SetActive(true);
        isUIActive = true;
    }

    public void SetNewLevel(int level)
    {
        titleText.text = "You Leveled Up - Level " + level;
    }


    public void SetFoodImages(Sprite newImage)
    {
        unlockedFoods[unlockedFoodsCount].sprite = newImage;
        unlockedFoods[unlockedFoodsCount].enabled = true;
        unlockedFoodsCount++;
    }
    public void SetBuildImages(Sprite newImage)
    {
        unlockedBuilds[unlockedBuildsCount].sprite = newImage;
        unlockedBuilds[unlockedBuildsCount].enabled = true;
        unlockedBuildsCount++;
    }
    public void SetBuildLimitImages(Sprite newImage)
    {
        unlockedBuildLimits[unlockedBuildLimitsCount].sprite = newImage;
        unlockedBuildLimits[unlockedBuildLimitsCount].enabled = true;
        unlockedBuildLimitsCount++;
    }

    // Close the UI and reset all images
    private void CloseUnlockUI()
    {
        unlockUI.SetActive(false);
        isUIActive = false;
        ClearAllImages();
    }

    // Clears all images in the lists
    private void ClearAllImages()
    {
        foreach (var image in unlockedFoods)
        {
            image.enabled = false;
        }
        foreach (var image in unlockedBuilds)
        {
            image.enabled = false;
        }
        foreach (var image in unlockedBuildLimits)
        {
            image.enabled = false;
        }
        unlockedFoodsCount = 0;
        unlockedBuildsCount = 0;
        unlockedBuildLimitsCount = 0;
}
}