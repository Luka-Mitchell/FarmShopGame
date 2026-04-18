using UnityEngine;

public class PlayerMovement : MonoBehaviour, IDataPersistence
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Get input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Normalize to prevent faster diagonal movement
        movement = movement.normalized;

        // Move player
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        // control player animation
        animator.SetFloat("Up", movement.y);
        animator.SetFloat("Down", -movement.y);
        animator.SetFloat("Left", -movement.x);
        animator.SetFloat("Right", movement.x);

        if (movement.x > 0)
        {
            //GetComponent<Transform>().
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (movement.x < 0)
        {
            transform.rotation = Quaternion.Euler(0,0,0);
        }
    }

    public void LoadData(GameData data)
    {
        this.transform.position = data.playerPosition;
    }

    public void SaveData(GameData data)
    {
        data.playerPosition = this.transform.position;
	}
}
