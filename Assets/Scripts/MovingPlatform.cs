using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 4f; // How high it goes from where you place it
    public float speed = 3f;

    private Rigidbody2D rb;
    private Vector2 startPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Remember exactly where you placed it in the editor
        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        // PingPong bounces between 0 and moveDistance smoothly over time
        float pingPong = Mathf.PingPong(Time.time * speed, moveDistance);
        
        // Move relative to its original starting spot
        Vector2 targetPosition = new Vector2(startPosition.x, startPosition.y + pingPong);
        rb.MovePosition(targetPosition);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}