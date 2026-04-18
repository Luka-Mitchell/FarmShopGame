using System.Collections;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public bool IsReadyToOrder { get; private set; } = true;

    public SpriteRenderer orderImageRenderer;

    public FoodData order;
    private Sprite orderIcon;
    public Vector3 targetPosition;
    private float moveSpeed;

    void Awake()
    {
        targetPosition = transform.position;
    }

    public FoodData GetOrder() { return order; }

    public void SetOrder(FoodData foodOrder)
    {
        order = foodOrder;

        orderImageRenderer.sprite = order.Sprite;
    }

    public void MoveTowards(Vector3 position, float speed)
    {
        IsReadyToOrder = false;
		targetPosition = position;
        moveSpeed = speed;
        StartCoroutine(MoveToPosition());
    }

    private IEnumerator MoveToPosition()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
			yield return null;
		}
        IsReadyToOrder = true;
        Debug.Log("Customer Finished Moving");
	}

    public void CompleteOrder()
    {
        // update gold/xp
        Debug.Log("Order complete for: " + order.Name);
        ResourceManager.Instance.UpdateGoldAmount(order.Cost);
        GameStateManager.Instance.AddXP(order.XP);
        Destroy(gameObject);
    }
}
