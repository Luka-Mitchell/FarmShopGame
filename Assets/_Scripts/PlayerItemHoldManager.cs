using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemHoldManager : MonoBehaviour
{
    public GameObject heldItemObject;

    private SpriteRenderer spriteRenderer;
    private FoodData heldItem;

    // Start is called before the first frame update
    void Start()
    {
        heldItemObject.SetActive(false);
        spriteRenderer = heldItemObject.GetComponent<SpriteRenderer>();
    }

    public void PickUpItem(FoodData food)
    {
        spriteRenderer.sprite = food.Sprite;
        heldItem = food;
        heldItemObject.SetActive(true);
    }

    public void DropItem()
    {
        heldItemObject.SetActive(false);
        heldItem = null;
    }

    public FoodData GetItem()
    {
        return heldItem;
    }

}
