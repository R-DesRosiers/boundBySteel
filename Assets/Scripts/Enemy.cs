using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 3;
    public float moveSpeed = 2f;
    public float stopDistance = 1.5f;

    public int attackDamage = 1;
    public float attackCooldown = 1f;
    private float attackTimer = 0f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        attackTimer -= Time.deltaTime;
        if (distance > stopDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        }
        else if (attackTimer <= 0f)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.TakeDamage(attackDamage);
            }

            attackTimer = attackCooldown;
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
}