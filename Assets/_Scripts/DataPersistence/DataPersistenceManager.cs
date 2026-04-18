using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
	[Header("Debugging")]
	[SerializeField] private bool initializeDataIfNull = false;
	[SerializeField] private bool resetDataOnLoad = false;
	[SerializeField] private bool ResetDataAndSetDevMode = false;
	[Header("File Storage Config")]
	[SerializeField] private string fileName;

    public static DataPersistenceManager Instance {  get; private set; }

	private GameData gameData;
	private List<IDataPersistence> dataPersistenceObjects;
	private FileDataHandler dataHandler;

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.Log("More than one DataPersistenceManager in the scene. Newest one was destroyed");
			Destroy(gameObject);
			return;
		}
		else
		{
			Instance = this;
		}
		DontDestroyOnLoad(this.gameObject);

		this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
		SceneManager.sceneUnloaded += OnSceneUnloaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
		SceneManager.sceneUnloaded -= OnSceneUnloaded;
	}

	public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		Debug.Log($"Scene Loaded: {scene.name}");
		LoadGame();
	}
	public void OnSceneUnloaded(Scene scene)
	{
		Debug.Log($"Scene Unloaded: {scene.name}");
		SaveGame();
	}

	public void NewGame()
	{
		this.gameData = new GameData();
		dataHandler.Save(gameData);
	}

	public void LoadGame()
	{
		// load saved data from file using data handler
		this.gameData = dataHandler.Load();

		if (ResetDataAndSetDevMode)
		{
			Debug.Log("Setting dev mode data");
			gameData = new GameData(0);
			dataHandler.Save(gameData);
		}
		else if (resetDataOnLoad) 
		{
			Debug.Log("Resetting data");
			NewGame();
		}
		else if (this.gameData == null && initializeDataIfNull)
		{
			Debug.Log("No save data found. A new game was started");
			NewGame();
		}

		if (this.gameData == null)
		{
			Debug.Log("No save data found. A new game needs to be started before data can be loaded");
			return;
		}

		// push the loaded data to all other scripts that need it
		this.dataPersistenceObjects = FindAllDataPersistenceObjects();
		foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
		{
			dataPersistenceObj.LoadData(gameData);
		}

		if (ResetDataAndSetDevMode)
		{
			BuildingStateManager.Instance.SetDevModeData();
			FoodStateManager.Instance.SetDevModeData();
		}
	}

	public void SaveGame()
	{
		// if we dont have any data to save, log a warning
		if (this.gameData == null)
		{
			Debug.LogWarning("No data was found. A new game needs to be started before data can be saved");
			return;
		}

		// pass the data to other scripts so they can update it
		this.dataPersistenceObjects = FindAllDataPersistenceObjects();
		foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
		{
			dataPersistenceObj.SaveData(gameData);
		}

		// save data to file using file data handler
		dataHandler.Save(gameData);
	}

	private void OnApplicationQuit()
	{
		this.dataPersistenceObjects = FindAllDataPersistenceObjects();
		SaveGame();
	}

	private List<IDataPersistence> FindAllDataPersistenceObjects()
	{
		IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

		return new List<IDataPersistence>(dataPersistenceObjects);
	}

	public bool HasGameData()
	{
		return gameData != null;
	}
}
