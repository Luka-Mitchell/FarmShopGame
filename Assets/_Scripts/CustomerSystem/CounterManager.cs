using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterManager : MonoBehaviour
{
    public float interactionDistance = 5f;
    public GameObject customerManagerObject;

    private GameObject gameManager;
    private GameObject player;
    private SpriteRenderer spriteRenderer;
    private CustomerManager customerManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager");
        player = GameObject.FindWithTag("Player");
        spriteRenderer = GetComponent<SpriteRenderer>();
        customerManager = customerManagerObject.GetComponent<CustomerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerInRange())
        {
            // highlight green
            spriteRenderer.color = new Color(0.4150943f, 0.7f, 0.05678178f, 1f); //light green

            if (Input.GetMouseButtonDown(0)) // 0 is for left mouse button
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                int counterLayerMask = LayerMask.GetMask("Clickable");
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, counterLayerMask);

                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    Debug.Log("Counter clicked");
                    CompleteOrder();
                }
            }
        }
        else
        {
            spriteRenderer.color = new Color(0.4150943f, 0.180884f, 0.05678178f, 1f); // brown
        }
    }

    private void CompleteOrder()
    {
        FoodData nextOrder = customerManager.GetNextOrder();

        if (gameManager.GetComponent<InventoryManager>().GetQuantity(nextOrder.Name) < 1)
        {
            Debug.Log("Order is not in stock");
            return;
        }

        if (!customerManager.IsCustomerReady()) 
        {
            Debug.Log("Next customer is still moving to counter");
            return;
        }

		customerManager.ServeCurrentCustomer();
		gameManager.GetComponent<InventoryManager>().UpdateQuantity(nextOrder.Name, -1);
		return;
    }

    private bool PlayerInRange()
    {
        return (Vector3.Distance(transform.position, player.transform.position) <= interactionDistance);
    }
}
