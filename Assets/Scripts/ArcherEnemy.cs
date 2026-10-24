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

    protected override void Start()
    {
        base.Start();

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

        if (player == null || knockbackTimer > 0f)
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
            knockbackTimer > 0f
        )
        {
            return;
        }

        float horizontalDifference =
            player.position.x - transform.position.x;

        float horizontalDistance =
            Mathf.Abs(horizontalDifference);

        float movementDirection = 0f;

        // Move toward the player if too far away
        if (
            horizontalDistance >
            preferredDistance + distanceTolerance
        )
        {
            movementDirection = Mathf.Sign(horizontalDifference);
        }
        // Move away from the player if too close
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
    {
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
        // Always fire the first arrow
        ShootArrow();

        // Sometimes fire a second arrow
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

        // Arrow starts at the archer's bow
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

        // Aim toward either the player's head or lower body
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
}