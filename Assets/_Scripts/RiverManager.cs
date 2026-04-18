using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class RiverManager : MonoBehaviour
{
    public float collectionTime = 5f; // Time required to collect water
    public float interactionRange = 3f; // Range within which the player can interact with the river
    public int collectionAmount = 1;
    //public Color inRangeColor = Color.green; // Color when player is within range
    public Image gauge;      // UI Image for the progress gauge
    public GameObject canvas;
    public List<GameObject> riverComponents;

    private GameObject gameManager;
    //private Color defaultColor; // Default river color
    private Transform player; // Reference to the player
    //private Renderer riverRenderer; // Renderer of the river to change its color
    private bool isCollecting = false; // Is the player currently collecting water?
    private float progress = 0f; // Tracks the progress of water collection
    private Vector3 collectionPosition;

    private void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager");
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        //riverRenderer = GetComponent<Renderer>();
        //defaultColor = riverRenderer.material.color;
        canvas.SetActive(false);
    }

    private void Update()
    {
        if (IsPlayerInRange())
        {
            //riverRenderer.material.color = inRangeColor;

            if (Input.GetMouseButtonDown(0) && !isCollecting && DayCycleManager.Instance.IsDayActive) // Left mouse click to collect water
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                int layerMask = LayerMask.GetMask("Clickable");
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);

                foreach (GameObject river in riverComponents)
                {
                    if (hit.collider != null && hit.collider.gameObject == river)
                    {
                        StartCollectingWater();
                    }
                }
                
                
            }
        }

        if (isCollecting && player.position != collectionPosition)
        {
            //riverRenderer.material.color = defaultColor;
            CancelWaterCollection();
        }

        if (isCollecting && DayCycleManager.Instance.IsDayActive)
        {
            CollectWater();
        }
    }

    private bool IsPlayerInRange()
    {
        //return Physics2D.Distance(player, this.GetComponent<Collider2D>()).distance <= interactionRange;
        return (Math.Abs(player.position.x - transform.position.x) <= interactionRange);
    }

    private void StartCollectingWater()
    {
        canvas.SetActive(true);
        isCollecting = true;
        progress = 0f;
        gauge.fillAmount = progress;
        collectionPosition = player.position;
        Debug.Log("Started collecting water...");
    }

    private void CollectWater()
    {
        progress += Time.deltaTime / collectionTime;
        gauge.fillAmount = progress;

        if (progress >= 1f)
        {
            CompleteWaterCollection();
        }
    }

    private void CompleteWaterCollection()
    {
        canvas.SetActive(false);
        isCollecting = false;
        gameManager.GetComponent<InventoryManager>().UpdateQuantity("Water", collectionAmount);
        Debug.Log("Water collected!");
    }

    private void CancelWaterCollection()
    {
        canvas.SetActive(false);
        isCollecting = false;
        progress = 0f;
        Debug.Log("Water collection cancelled!");
    }
}
