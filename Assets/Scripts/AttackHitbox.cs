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

    private void OnEnable()
    {
        Debug.Log("ATTACK HITBOX ENABLED");
    }

    private void OnDisable()
    {
        Debug.Log("ATTACK HITBOX DISABLED");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(
            "Hitbox touched: " + other.name +
            " | Tag: " + other.tag +
            " | Layer: " + LayerMask.LayerToName(other.gameObject.layer) +
            " | Position: " + other.transform.position
        );

        Enemy enemy = other.GetComponentInParent<Enemy>();

        if (enemy != null)
        {
            Debug.Log(
                enemy.name +
                " health before hit: " +
                enemy.health
            );

            enemy.TakeDamage(damage);

            float horizontalDirection = Mathf.Sign(
                enemy.transform.position.x -
                attacker.position.x
            );

            enemy.Knockback(
                new Vector2(horizontalDirection, 0f)
            );

            Debug.Log(
                enemy.name +
                " health after hit: " +
                enemy.health
            );

            Debug.Log(
                "Successfully dealt " +
                damage +
                " damage and applied knockback to " +
                enemy.name +
                "!"
            );

            Debug.Log(
                "Hitbox position: " + transform.position +
                " | Enemy position: " + enemy.transform.position
            );
        }
        else
        {
            Debug.Log(
                "The object touched does not have an Enemy script."
            );
        }
    }
}