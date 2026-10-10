using UnityEngine;

public class GhostEnemy : Enemy
{
    [Header("Ghost Settings")]
    public float floatHeight = 0.5f;
    public float floatSpeed = 2f;
    private float startY;

    protected override void Start()
    {
        base.Start(); // Runs the parent Start() to find player and components
        startY = transform.position.y;

        if (rb != null)
        {
            rb.gravityScale = 0f; // Ghosts ignore gravity
        }
    }

    protected override void Update()
    {
        base.Update(); // Handles attack timers and knockback checks

        if (player == null || knockbackTimer > 0f)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            // Move toward player in all directions + add a subtle bob
            Vector2 direction = (player.position - transform.position).normalized;
            float targetY = player.position.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            
            Vector2 targetVel = new Vector2(
                direction.x * moveSpeed, 
                (targetY - transform.position.y) * moveSpeed
            );
            rb.linearVelocity = targetVel;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;

            // Attack logic inherited from base Enemy class
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

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage); // Handles health reduction and death

        if (health > 0 && anim != null)
        {
            anim.SetTrigger("Hit"); // Triggers the ghost-specific hurt animation
        }
    }
}