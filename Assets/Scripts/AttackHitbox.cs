using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // LOG 1: Checks if physics collision is actually firing
        Debug.Log($"Hitbox touched: {other.name} (Tag: {other.tag})");

        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                // LOG 2: Confirms TakeDamage was called
                Debug.Log($"Successfully dealt {damage} damage to {enemy.name}!");
            }
            else
            {
                // LOG 3: Tag matched, but script missing
                Debug.LogError($"Object {other.name} has 'Enemy' tag, but Enemy.cs script was not found!");
            }
        }
    }
}