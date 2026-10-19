using UnityEngine;

public class ArcherEnemy : MonoBehaviour
{
    public Transform player;
    public Transform arrowSpawn;
    public GameObject arrowPrefab;

    [Header("Combat Settings")]
    public float attackRange = 8f;
    public float attackCooldown = 2f;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float preferredDistance = 8f;
    public float distanceTolerance = 0.5f;

    private Animator animator;
    private Rigidbody2D rb;
    private float cooldownTimer;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (player == null)
        {
            return;
        }

        FacePlayer();

        float horizontalDistance = Mathf.Abs(
            player.position.x - transform.position.x
        );

        if (horizontalDistance <= attackRange && cooldownTimer <= 0)
        {
            Attack();
        }
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            return;
        }

        float horizontalDifference =
            player.position.x - transform.position.x;

        float horizontalDistance = Mathf.Abs(horizontalDifference);

        float movementDirection = 0f;

        // Move toward the player if they are too far away
        if (horizontalDistance > preferredDistance + distanceTolerance)
        {
            movementDirection = Mathf.Sign(horizontalDifference);
        }
        // Move away from the player if they are too close
        else if (horizontalDistance < preferredDistance - distanceTolerance)
        {
            movementDirection = -Mathf.Sign(horizontalDifference);
        }

        if (movementDirection != 0)
        {
            Vector2 newPosition = rb.position;

            newPosition.x += movementDirection *
                             moveSpeed *
                             Time.fixedDeltaTime;

            rb.MovePosition(newPosition);
        }
    }

    private void FacePlayer()
    {
        if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");

        cooldownTimer = attackCooldown;

        Invoke(nameof(ShootArrow), 0.35f);
    }

    private void ShootArrow()
    {
        Debug.Log("ShootArrow was called");
        if (arrowPrefab == null || arrowSpawn == null || player == null)
        {
            return;
        }

        GameObject newArrow = Instantiate(
            arrowPrefab,
            arrowSpawn.position,
            Quaternion.identity
        );

        ArrowProjectile arrow =
            newArrow.GetComponent<ArrowProjectile>();

        if (arrow == null)
        {
            Debug.LogError(
                "The arrow prefab is missing the ArrowProjectile script."
            );

            return;
        }

        float direction;

        if (player.position.x < transform.position.x)
        {
            direction = -1;
        }
        else
        {
            direction = 1;
        }

        arrow.SetDirection(direction);
    }

    public void SetPlayer(Transform detectedPlayer)
    {
        player = detectedPlayer;
    }

    public void ClearPlayer(Transform detectedPlayer)
    {
        if (player == detectedPlayer)
        {
            player = null;
        }
    }
}