using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Hitbox touched: {other.name}");

        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            Debug.Log(
                $"Successfully dealt {damage} damage to {enemy.name}!"
            );
        }
        else
        {
            Debug.Log("The object touched does not have an Enemy script.");
        }
    }
}