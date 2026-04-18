using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[System.Serializable]
public class CustomerData
{
    public Vector3 currentPosition;
    public Vector3 targetPosition;
	public FoodData order;

    public CustomerData(Vector3 currentPosition, Vector3 targetPosition, FoodData order)
    {
        this.currentPosition = currentPosition;
        this.targetPosition = targetPosition;
        this.order = order;
    }
}


public class CustomerManager : MonoBehaviour, IDataPersistence
{
    public Transform counterPosition; // Position at the counter
    public Transform queueStartPosition; // Starting position of the queue
    public Transform queueEndPosition; // Position of the last Customer in the queue
    public float moveSpeed = 2f; // Movement speed for Customers
    public GameObject customerPrefab; // Prefab for Customer characters
    public int maxQueueSize = 10;
    public int startQueueSize = 0;
    public int baseCustomersPerDay = 10;

    private Queue<Customer> customerQueue = new Queue<Customer>();
	private float spawnDelay = 0;
	private int customersPerDay = 0;
	private float lastSpawnTime = 0f;

    private void OnEnable()
    {
        DayCycleManager.OnDayStart += UpdateCustomersPerDay;
        DayCycleManager.OnDayEnd += ResetCustomers;
    }

    private void OnDisable()
    {
        DayCycleManager.OnDayStart -= UpdateCustomersPerDay;
        DayCycleManager.OnDayEnd -= ResetCustomers;
    }

    private void UpdateCustomersPerDay()
    {
		/*
        int dayFactor = DayCycleManager.Instance.currentDay;
        int levelFactor = GameStateManager.Instance.playerLevel;
        float starFactor = DayCycleManager.Instance.shopStarRating;
        customersPerDay = (int)(baseCustomersPerDay + (dayFactor + levelFactor) * starFactor / 5);
        */
		customersPerDay = baseCustomersPerDay;
        spawnDelay = DayCycleManager.Instance.dayDuration / customersPerDay;
    }

    private void ResetCustomers()
    {
        
        List<Customer> customerList = customerQueue.ToList();
        for (int i = 0; i < customerList.Count; i++)
        {
            Destroy(customerList[i].gameObject);
        }
        customerQueue.Clear();
    }

    void Start()
    {
        //SpawnQueue();
    }

    private void SpawnQueue()
    {
        for (int i = 0; i < startQueueSize; i++)
        {
            Vector3 spawnPosition = Vector3.Lerp(queueStartPosition.position, queueEndPosition.position, (float)i / maxQueueSize);
            GameObject newCustomer = Instantiate(customerPrefab, spawnPosition, Quaternion.identity);
            Customer customer = newCustomer.GetComponent<Customer>();
            customer.SetOrder(GetRandomOrder());
            //customer.SetOrder(FoodTypeManager.Instance.GetFoodType("Bread"));
            customerQueue.Enqueue(customer);
        }
    }

    private FoodData GetRandomOrder()
    {
        List<FoodData> unlockedDishes = new List<FoodData>();
        if (FoodDataManager.Instance != null && FoodStateManager.Instance != null)
        {
            foreach (var foodData in FoodDataManager.Instance.foodDataList)
            {
                var state = FoodStateManager.Instance.GetFoodState(foodData.Name);
                if (state != null && state.isUnlocked && foodData.Cost > 0)
                {
                    unlockedDishes.Add(foodData);
                }
            }
        }
        int num = Random.Range(0, unlockedDishes.Count);
        if (unlockedDishes.Count == 0)
        {
            return null;
        }
        return unlockedDishes[num];
    }

    private void FixedUpdate()
    {
        //Debug.Log($"DayActive: {dayActive} | customerQueue.Count: {customerQueue.Count} | maxQueueSize: {maxQueueSize}");
        if (DayCycleManager.Instance.IsDayActive && Time.fixedTime - lastSpawnTime >= spawnDelay && customerQueue.Count < maxQueueSize)
        {
            lastSpawnTime = Time.fixedTime;

            Vector3 spawnPosition = Vector3.Lerp(queueStartPosition.position, queueEndPosition.position, (float)customerQueue.Count / maxQueueSize);
            GameObject newCustomer = Instantiate(customerPrefab, spawnPosition, Quaternion.identity);
            Customer customer = newCustomer.GetComponent<Customer>();
            customer.SetOrder(GetRandomOrder());
            customerQueue.Enqueue(customer);
        }
    }

    public FoodData GetNextOrder()
    {
        if (customerQueue.Count > 0)
        {
            return customerQueue.Peek().GetOrder();
        }
        else
        {
            return null;
        }
    }

    public bool IsCustomerReady()
    {
        return customerQueue.Peek().IsReadyToOrder;
    }

    public void ServeCurrentCustomer()
    {
        if (customerQueue.Count > 0)
        {
            Customer servedCustomer = customerQueue.Dequeue();
            Transform customerTransform = servedCustomer.transform;
            servedCustomer.CompleteOrder();
            AdvanceQueue(customerTransform);
        }
    }

    private void AdvanceQueue(Transform firstPosition)
    {
        /*
        foreach (Customer customer in customerQueue)
        {
            customer.MoveTowards(counterPosition.position, moveSpeed);
        }
        */

        List<Customer> customerList = customerQueue.ToList();
        for (int i = customerList.Count-1; i >= 0; i--)
        {
            if (i == 0)
            {
                customerList[i].MoveTowards(firstPosition.position, moveSpeed);
            }
            else
            {
                customerList[i].MoveTowards(customerList[i - 1].transform.position, moveSpeed);
            }
        }
    }

    public void LevelUp()
    {
        customersPerDay = Mathf.RoundToInt(1.5f * customersPerDay);

	}

    public void LoadData(GameData data)
    {
        Debug.Log("Loading Customer Data");
        if (data.customerList != null)
        {
            foreach (CustomerData customerData in data.customerList)
            {
                Debug.Log($"Test: customerdata = {customerData} ");
                Customer customer = Instantiate(customerPrefab, customerData.currentPosition, Quaternion.identity).GetComponent<Customer>();
                customer.SetOrder(customerData.order);
                customer.MoveTowards(customerData.targetPosition, moveSpeed);
                customerQueue.Enqueue(customer);
		    }
        }
        this.lastSpawnTime = data.lastSpawnTime;
        UpdateCustomersPerDay();

	}

    public void SaveData(GameData data)
    {
        // negative to account for time passed last save, when starting from time=0
        data.lastSpawnTime = this.lastSpawnTime - Time.fixedTime;

		data.customerList.Clear();
        foreach (Customer customer in customerQueue)
        {
            
            CustomerData customerData = new(customer.gameObject.transform.position, customer.targetPosition, customer.order);
			Debug.Log($"Saving Customer Data");
			data.customerList.Add(customerData);
        }
    }
}
