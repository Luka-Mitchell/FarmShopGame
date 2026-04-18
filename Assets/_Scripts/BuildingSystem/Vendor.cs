using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Vendor : MonoBehaviour
{
    public float interactionDistance = 5f;
	public Canvas vendorUICanvas;
    public bool isUsingVendor = false;

    private GameObject player;
    private SpriteRenderer spriteRenderer;


    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        spriteRenderer = GetComponent<SpriteRenderer>();
        CloseVendorMenu();
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseVendorMenu();
        }
        if (!isUsingVendor) return;

        if (PlayerInRange())
        {
            // highlight green
            spriteRenderer.color = new Color(0.7f, 1f, 0.7f, 1f); //light green

            if (Input.GetMouseButtonDown(0)) // 0 is for left mouse button
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                int vendorLayerMask = LayerMask.GetMask("Clickable");
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, vendorLayerMask);

                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    OpenVendorMenu();
                }
            }
        }
        else
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f); //white
        }
    }

    private bool PlayerInRange()
    {
        return (Vector3.Distance(this.transform.position, player.transform.position) <= interactionDistance);
    }
    public void OpenVendorMenu()
    {
        if (GameStateManager.Instance.isUIOpen)
        {
            Debug.Log("A UI is already open, isUIOpen = " + GameStateManager.Instance.isUIOpen);
            return;
        } 
        vendorUICanvas.gameObject.SetActive(true);
        GameStateManager.Instance.isUIOpen = true;
        Debug.Log("Opened vendor, isUIOpen = " + GameStateManager.Instance.isUIOpen);
    }

    public void CloseVendorMenu()
    {
        vendorUICanvas.gameObject.SetActive(false);
        GameStateManager.Instance.isUIOpen = false;
        Debug.Log("Closed vendor, isUIOpen = " + GameStateManager.Instance.isUIOpen);
    }


}
