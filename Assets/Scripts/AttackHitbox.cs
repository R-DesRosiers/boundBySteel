using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 1;
    public Transform attacker;

    private void Awake()
    {
        if (attacker == null)
        {
            attacker = transform.root;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Hitbox touched: {other.name}");

        Enemy enemy = other.GetComponentInParent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            float horizontalDirection =
                Mathf.Sign(
                    enemy.transform.position.x -
                    attacker.position.x
                );

            enemy.Knockback(
                new Vector2(horizontalDirection, 0f)
            );

            Debug.Log(
                $"Successfully dealt {damage} damage and applied knockback to {enemy.name}!"
            );
        }
        else
        {
            Debug.Log("The object touched does not have an Enemy script.");
        }
    }
}