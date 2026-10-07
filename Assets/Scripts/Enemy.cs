using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 3;
    public float moveSpeed = 2f;
    public float stopDistance = 1.5f;

    public int attackDamage = 1;
    public float attackCooldown = 1f;
    private float attackTimer = 0f;

    public float knockbackForce = 5f;
    public float knockbackUpwardForce = 2f; 
    public float knockbackDuration = 0.2f;

    private Transform player;
    private Rigidbody2D rb;

    private float knockbackTimer = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null)
            return;

        attackTimer -= Time.deltaTime;

        // If enemy is being knocked back, don't chase or attack
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            rb.linearVelocity = new Vector2(
                direction.x * moveSpeed,
                rb.linearVelocity.y
            );
        }
        else
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );

            if (attackTimer <= 0f)
            {
                PlayerController playerController = player.GetComponent<PlayerController>();

                if (playerController != null)
                {
                    playerController.TakeDamage(attackDamage);
                }

                attackTimer = attackCooldown;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Knockback(Vector2 direction)
    {
        if (rb != null)
        {
            knockbackTimer = knockbackDuration;

            Vector2 knockback = new Vector2(
                direction.x * knockbackForce,
                knockbackUpwardForce
            );

            rb.linearVelocity = knockback;
        }
    }
}