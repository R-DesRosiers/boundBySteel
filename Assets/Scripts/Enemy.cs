using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 3; 
    [HideInInspector] public int health = 3;
    public float moveSpeed = 2f;
    public float stopDistance = 1.5f;

    [Header("Combat")]
    public int attackDamage = 1;
    public float attackCooldown = 1f;
    protected float attackTimer = 0f;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackUpwardForce = 2f; 
    public float knockbackDuration = 0.2f;
    protected float knockbackTimer = 0f;

    protected Transform player;
    protected Rigidbody2D rb;
    protected Animator anim;

    protected virtual void Start()
    { // sets up health, finds object w player tag, etc. 
        health = maxHealth; 
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
            
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if (player == null)
            return;

        attackTimer -= Time.deltaTime;

        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;
            return;
        }
    }

    // Virtual so child classes can add custom behavior
    public virtual void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

   protected virtual void Die()
    {
        if (player != null)
        {
            PlayerController playerController =
                player.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.RestoreFullHealth();
            }
        }
        Destroy(gameObject);
    }

    public void Knockback(Vector2 direction)
    { // knocks enemy (vertical & horizontal) when hit 
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