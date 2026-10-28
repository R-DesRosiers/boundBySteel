using UnityEngine;
using System.Collections;

public class ArcherEnemy : Enemy
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackSound;

    [Header("Archer References")]
    public Transform arrowSpawn;
    public GameObject arrowPrefab;

    [Header("Archer Combat Settings")]
    public float attackRange = 8f;

    [Range(0f, 1f)]
    public float twoArrowBurstChance = 0.35f;

    public float burstDelay = 0.2f;

    [Header("Arrow Height Settings")]
    [Range(0f, 1f)]
    public float highArrowChance = 0.5f;

    public float highArrowOffset = 0.8f;
    public float lowArrowOffset = 0.15f;

    [Header("Archer Movement Settings")]
    public float preferredDistance = 8f;
    public float distanceTolerance = 0.5f;

    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.15f;

    private Collider2D bodyCollider;

    protected override void Start()
    {
        base.Start();
        // set up collider & arrows 
        bodyCollider = GetComponent<Collider2D>();

        if (bodyCollider == null)
        {
            Debug.LogWarning(
                "Archer could not find a Collider2D for ground detection."
            );
        }

        if (anim == null)
        {
            anim = GetComponentInChildren<Animator>();
        }

        if (arrowSpawn == null)
        {
            Debug.LogWarning(
                "Arrow Spawn has not been assigned on the Archer."
            );
        }

        if (arrowPrefab == null)
        {
            Debug.LogWarning(
                "Arrow Prefab has not been assigned on the Archer."
            );
        }
    }

    protected override void Update()
    {
        base.Update();

        if (
            player == null ||
            knockbackTimer > 0f ||
            !IsGrounded()
        )
        {
            return;
        }

        FacePlayer();

        float horizontalDistance = Mathf.Abs(
            player.position.x - transform.position.x
        );

        if (
            horizontalDistance <= attackRange &&
            attackTimer <= 0f
        )
        {
            Attack();
        }
    }

    private void FixedUpdate()
    {
        if (
            player == null ||
            rb == null ||
            knockbackTimer > 0f ||
            !IsGrounded()
        )
        {
            return;
        }

        float horizontalDifference =
            player.position.x - transform.position.x;

        float horizontalDistance =
            Mathf.Abs(horizontalDifference);

        float movementDirection = 0f;

        // distances use attack range (circle that will detect player if inside)
        // Move toward if too far
        if (
            horizontalDistance >
            preferredDistance + distanceTolerance
        )
        {
            movementDirection = Mathf.Sign(horizontalDifference);
        }
        // Move away if too close
        else if (
            horizontalDistance <
            preferredDistance - distanceTolerance
        )
        {
            movementDirection = -Mathf.Sign(horizontalDifference);
        }

        if (movementDirection != 0f)
        {
            Vector2 newPosition = rb.position;

            newPosition.x +=
                movementDirection *
                moveSpeed *
                Time.fixedDeltaTime;

            rb.MovePosition(newPosition);
        }
    }

    private bool IsGrounded()
    { // to check if its grounded (that way it doesn't move mid air)
        if (bodyCollider == null)
        {
            return false;
        }

        Bounds bounds = bodyCollider.bounds;

        Vector2 boxCenter = new Vector2(
            bounds.center.x,
            bounds.min.y
        );

        Vector2 boxSize = new Vector2(
            bounds.size.x * 0.8f,
            0.05f
        );

        RaycastHit2D groundHit = Physics2D.BoxCast(
            boxCenter,
            boxSize,
            0f,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        return groundHit.collider != null;
    }

    private void FacePlayer()
    {
        if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
        else
        {
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }

    private void Attack()
    { // attack (vary arrow numbers (bursts), and play bow sound)
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }

        attackTimer = attackCooldown;

        Invoke(nameof(BeginArrowBurst), 0.35f);
    }

    private void BeginArrowBurst()
    {
        StartCoroutine(ShootArrowBurst());
    }

    private IEnumerator ShootArrowBurst()
    {
        // always 1 arrow! 
        ShootArrow();

        // sometimes add a second arrow
        if (Random.value <= twoArrowBurstChance)
        {
            yield return new WaitForSeconds(burstDelay);

            ShootArrow();
        }
    }

    private void ShootArrow()
    {
        if (arrowPrefab == null)
        {
            Debug.LogWarning(
                "Cannot shoot: Arrow Prefab is missing."
            );

            return;
        }

        if (arrowSpawn == null)
        {
            Debug.LogWarning(
                "Cannot shoot: Arrow Spawn is missing."
            );

            return;
        }

        if (player == null)
        {
            return;
        }

        float arrowHeightOffset;

        if (Random.value <= highArrowChance)
        {
            arrowHeightOffset = highArrowOffset;
        }
        else
        {
            arrowHeightOffset = lowArrowOffset;
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

        Vector2 targetPosition = new Vector2(
            player.position.x,
            player.position.y + arrowHeightOffset
        );

        arrow.SetTarget(targetPosition);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (health > 0 && anim != null)
        {
            anim.SetTrigger("Hit");
        }
    }

    // visualize the body collider by drawing it 
    private void OnDrawGizmosSelected()
    {
        if (bodyCollider == null)
        {
            return;
        }

        Bounds bounds = bodyCollider.bounds;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(
            new Vector2(
                bounds.center.x,
                bounds.min.y - groundCheckDistance / 2f
            ),
            new Vector2(
                bounds.size.x * 0.8f,
                groundCheckDistance
            )
        );
    }
}