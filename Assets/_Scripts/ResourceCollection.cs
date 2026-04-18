using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResourceCollection : MonoBehaviour, IDataPersistence, IHasID
{
    public float collectTime = 5f;
    public Image gauge;      // UI Image for the progress gauge
    public GameObject canvas;
    public int collectionAmount = 1;
    //public TextMeshProUGUI inventoryQuantityText;  // Text to show quantity
    public string buildingName;
    public string itemName;
    public float interactionDistance = 3f;
    public GameObject resourceObject;
	public string ID { get; set; }

	private GameObject gameManager;
    //private bool isCollecting = false;
    private float progress = 0f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private GameObject player;
    

    private void Awake()
    {
        ID = System.Guid.NewGuid().ToString();
        Debug.Log("Set ID for " + buildingName + ": " + ID);
    }

    private void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager");
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        player = GameObject.FindWithTag("Player");
        UpdateGauge();
        canvas.SetActive(true);
        if (resourceObject != null)
        {
            resourceObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Update harvest progress if the player is on the farm
        if (DayCycleManager.Instance.IsDayActive && progress < 1f)
        {
            progress += Time.deltaTime / collectTime;
            UpdateGauge();

        }
        else if (DayCycleManager.Instance.IsDayActive && resourceObject != null)
        {
            resourceObject.SetActive(true);
        }

        if (PlayerInRange())
        {
            spriteRenderer.color = new Color(0.7f, 1f, 0.7f, 1f);

            if (Input.GetMouseButtonDown(0) && DayCycleManager.Instance.IsDayActive)
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                int layerMask = LayerMask.GetMask("Clickable");
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);

                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    Collect();
                }
            }
        }
        else
        {
            spriteRenderer.color = originalColor;
        }
    }

    private bool PlayerInRange()
    {
        return (Vector3.Distance(this.transform.position, player.transform.position) <= interactionDistance);
    }

    private void UpdateGauge()
    {
        if (gauge != null)
        {
            // Update the fill amount of the gauge to show progress
            gauge.fillAmount = progress;
        }
    }

    private void Collect()
    {
        // Check if harvesting is complete
        if (progress >= 1f)
        {
            if (resourceObject != null)
            {
                resourceObject.SetActive(false);
            }
            progress = 0f; // Reset progress
            UpdateGauge();
            gameManager.GetComponent<InventoryManager>().UpdateQuantity(itemName, collectionAmount);
            //Debug.Log("Collected" + itemName);
        }
    }

	public void LoadData(GameData data)
	{
		if (this.buildingName == "River") { return; }
		data.buildingsDict.TryGetValue(ID, out BuildingSaveData buildingData);
        if (buildingData != null)
        {
            this.transform.position = buildingData.location;
            this.progress = buildingData.productionProgress;
			Debug.Log("Loading Building Data: " + buildingName);
		}
        else
        {
            Debug.LogError("Error: building with ID {" + ID + "} could not be found in save data");
        }
	}

	public void SaveData(GameData data)
	{
		if (this.buildingName == "River") { return; }
		if (data.buildingsDict.ContainsKey(ID))
        {
            data.buildingsDict.Remove(ID);
        }
        
        data.buildingsDict.Add(ID, new BuildingSaveData(buildingName, this.transform.position, progress, null));
        Debug.Log("Saving Building Data: " + buildingName);
	}
}
