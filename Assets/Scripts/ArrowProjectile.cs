using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 1;
    public float lifetime = 4f;

    private Vector2 movementDirection = Vector2.right;

    private void Update()
    {
        transform.position +=
            (Vector3)(movementDirection * speed * Time.deltaTime);

        lifetime -= Time.deltaTime;

        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void SetTarget(Vector2 targetPosition)
    {
        movementDirection = (
            targetPosition -
            (Vector2)transform.position
        ).normalized;

        // Makes the arrow point toward its target
        transform.right = movementDirection;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore the archer that fired the arrow
        if (other.GetComponentInParent<ArcherEnemy>() != null)
        {
            return;
        }

        PlayerController player =
            other.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            player.TakeDamage(damage,transform.position);
            Destroy(gameObject);
            return;
        }

        // Destroy the arrow when it hits a solid object
        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}