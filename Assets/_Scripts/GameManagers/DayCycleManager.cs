using UnityEngine;
using System.Collections;

public class DayCycleManager : MonoBehaviour, IDataPersistence
{
    public static DayCycleManager Instance; // Singleton for easy access

	public bool IsDayActive { get { return isDayActive; } private set { isDayActive = value; } }

	public TMPro.TextMeshProUGUI dayEndText;
    public TMPro.TextMeshProUGUI dayTimerText;
	public int currentDay = 0;
    public float dayDuration = 120f; // Duration in seconds
    public float shopStarRating = 5; // 1 to 5 stars
    
    private bool isDayActive = false;
    private float currentTime = 0;
    private int startingHour = 9;
    private int endingHour = 17;

    public delegate void DayEvent();
    public static event DayEvent OnDayStart;
    public static event DayEvent OnDayEnd;

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
        dayEndText.enabled = false;
        if (currentDay == 0)
        {
			StartDay();
		}
        else if (!isDayActive)
        {
			dayEndText.enabled = true;
		}
	}

	private void Update()
	{
        if (isDayActive && currentTime >= dayDuration)
        {
            currentTime = dayDuration;
			EndDay();
		}
        else if (isDayActive)
        {
			currentTime += Time.deltaTime;
            UpdateDayTimerUI();
		}

        if (!isDayActive && Input.GetKeyDown(KeyCode.Return))
        {
            StartDay();
        }
	}

	private void UpdateDayTimerUI()
	{
        float hourDuration = dayDuration / (endingHour - startingHour);
        float minuteDuration = hourDuration / 60;
        int currentHour = (int)Mathf.Floor(currentTime / hourDuration) + startingHour;
        int currentMinute = (int)Mathf.Floor(currentTime % hourDuration / minuteDuration);

        dayTimerText.text = $"{currentHour}:{currentMinute:D2}";
	}


	private void StartDay()
    {
        currentDay++;
		currentTime = 0;
		isDayActive = true;
		dayEndText.enabled = false;
		OnDayStart?.Invoke();
        Debug.Log("Day " + currentDay + " started!");
    }

    private void EndDay()
    {
        isDayActive = false;
		dayEndText.enabled = true;
        dayTimerText.text = endingHour + ":00";
		OnDayEnd?.Invoke();
        Debug.Log("Day " + currentDay + " ended!");
        DataPersistenceManager.Instance.SaveGame();
	}

    public void LoadData(GameData data)
    {
        this.currentDay = data.day;
        this.currentTime = data.currentTime;
        this.isDayActive = data.isDayActive;
    }

    public void SaveData(GameData data)
    {
		data.day = this.currentDay;
        data.currentTime = this.currentTime;
		data.isDayActive = this.isDayActive;
	}
}
