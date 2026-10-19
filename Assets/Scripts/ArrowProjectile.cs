using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 1;
    public float lifetime = 4f;

    private int direction = 1;

    private void Update()
    {
        transform.Translate(
            Vector2.right * direction * speed * Time.deltaTime
        );

        lifetime -= Time.deltaTime;

        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetDirection(float newDirection)
    {
        direction = newDirection < 0 ? -1 : 1;

        transform.localScale = new Vector3(
            direction,
            transform.localScale.y,
            transform.localScale.z
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore the archer that fired the arrow
        if (other.GetComponentInParent<ArcherEnemy>() != null)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            other.SendMessage(
                "TakeDamage",
                damage,
                SendMessageOptions.DontRequireReceiver
            );

            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}