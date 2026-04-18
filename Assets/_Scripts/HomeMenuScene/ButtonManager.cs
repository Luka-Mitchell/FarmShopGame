using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
	[SerializeField] private Button newGameButton;
	[SerializeField] private Button continueGameButton;
	[SerializeField] private TMPro.TextMeshProUGUI continueGameText;

	[SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas settingsCanvas;
    [SerializeField] private Canvas instructionsCanvas;

    private void Start()
    {
		if (!DataPersistenceManager.Instance.HasGameData())
		{
			continueGameButton.interactable = false;
			continueGameText.color = Color.gray;
		}
        mainCanvas.enabled = true;
        settingsCanvas.enabled = false;
		instructionsCanvas.enabled = false;
	}

    public void OnNewGameClicked()
    {
		DisableMenuButtons();
		// initialize game data
		DataPersistenceManager.Instance.NewGame();
		// Load the game scene
        SceneManager.LoadSceneAsync("MainGameScene");
    }

	public void OnContinueGameClicked()
	{
		DisableMenuButtons();
		// Load the next scene which will load the data because of OnSceneLoaded
		SceneManager.LoadSceneAsync("MainGameScene");
	}

	public void OnSettingsClicked()
    {
		mainCanvas.enabled = false;
		settingsCanvas.enabled = true;
    }

	public void OnExitClicked()
	{
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SettingsBackButton()
    {
		mainCanvas.enabled = true;
		settingsCanvas.enabled = false;
    }

	public void InstructionsBackButton()
	{
		mainCanvas.enabled = true;
		instructionsCanvas.enabled = false;
	}

	private void DisableMenuButtons()
	{
		newGameButton.interactable = false;
		continueGameButton.interactable = false;
	}
}
