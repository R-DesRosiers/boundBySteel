using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackSound;
    public AudioClip youLoseSound;

    [Header("Death Screen")]
    public TMP_Text deathMessage;
    public float deathMessageDuration = 2f;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public GameObject playerVisual;
    public float jumpForce = 8f;

    [Header("Slide Settings")]
    public float slideSpeed = 10f;
    public float slideDuration = 0.4f;
    public float slideCooldown = 1f;

    [Header("Slide Collider")]
    public BoxCollider2D playerCollider;

    [Range(0.2f, 0.8f)]
    public float slideHeightPercent = 0.5f;

    [Header("Wall Slide Settings")]
    public Transform wallCheckLeft;
    public Transform wallCheckRight;
    public Vector2 wallCheckSize = new Vector2(0.2f, 1.2f);
    public float wallSlideSpeed = 2f;
    public LayerMask wallLayer;

    [Header("Wall Jump Settings")]
    public float wallJumpHorizontalForce = 6f;
    public float wallJumpVerticalForce = 8f;

    public bool IsTouchingWall;
    public bool IsWallSliding;

    [Header("Combat & Health")]
    public int maxHealth = 5;
    public int health = 5;
    public float attackCooldown = 0.4f;
    public GameObject sword;

    [Header("Player Knockback")]
    public float knockbackForce = 6f;
    public float knockbackUpwardForce = 2f;
    public float knockbackDuration = 0.2f;

    [Header("Attack Hitbox")]
    public GameObject attackHitbox;
    public float attackHitboxDelay = 0.1f;
    public float attackHitboxDuration = 0.15f;
    public float attackDuration = 0.6f;

    [Header("Ground Check Setup")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.15f;
    public bool IsGrounded;

    private bool isSliding = false;
    private float slideTimer = 0f;

    private bool isAttacking = false;
    private bool attackQueued = false;
    private float attackTimer = 0f;
    private int attackIndex = 0;

    private bool isDead = false;
    private bool controlsLocked = true;

    private float knockbackTimer = 0f;
    private bool previousWallState = false;

    private bool touchingLeftWall = false;
    private bool touchingRightWall = false;

    private Animator animator;
    private Rigidbody2D rb;
    private Vector3 startPosition;

    private Vector2 standingColliderSize;
    private Vector2 standingColliderOffset;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        rb = GetComponent<Rigidbody2D>();

        if (playerCollider == null)
        {
            playerCollider = GetComponent<BoxCollider2D>();
        }

        if (playerCollider != null)
        {
            standingColliderSize = playerCollider.size;
            standingColliderOffset = playerCollider.offset;
        }
        else
        {
            Debug.LogWarning(
                "PlayerController could not find a BoxCollider2D."
            );
        }

        startPosition = transform.position;
        health = maxHealth;

        animator = GetComponentInChildren<Animator>();

        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        // Do not process movement or attack input while paused.
        if (Time.timeScale <= 0f)
        {
            return;
        }

        if (controlsLocked)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );

            if (animator != null)
            {
                animator.SetBool("IsRunning", false);
            }

            return;
        }

        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;
            return;
        }

        IsGrounded =
            groundCheck != null &&
            Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );

        Collider2D leftWallHit = null;

        if (wallCheckLeft != null)
        {
            leftWallHit = Physics2D.OverlapBox(
                wallCheckLeft.position,
                wallCheckSize,
                0f,
                wallLayer
            );
        }

        Collider2D rightWallHit = null;

        if (wallCheckRight != null)
        {
            rightWallHit = Physics2D.OverlapBox(
                wallCheckRight.position,
                wallCheckSize,
                0f,
                wallLayer
            );
        }

        touchingLeftWall = leftWallHit != null;
        touchingRightWall = rightWallHit != null;

        IsTouchingWall =
            touchingLeftWall ||
            touchingRightWall;

        if (IsTouchingWall != previousWallState)
        {
            if (IsTouchingWall)
            {
                Debug.Log("Player is touching a wall.");
            }
            else
            {
                Debug.Log("Player left the wall.");
            }

            previousWallState = IsTouchingWall;
        }

        IsWallSliding =
            IsTouchingWall &&
            !IsGrounded &&
            rb.linearVelocity.y <= 0f &&
            !isAttacking &&
            !isSliding;

        if (IsWallSliding)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                -wallSlideSpeed
            );
        }

        attackTimer -= Time.deltaTime;
        slideTimer -= Time.deltaTime;

        if (
            Keyboard.current != null &&
            Keyboard.current.shiftKey.wasPressedThisFrame &&
            IsGrounded &&
            !isSliding &&
            !isAttacking &&
            slideTimer <= 0f
        )
        {
            StartCoroutine(Slide());
        }

        if (!isSliding)
        {
            HandleMovement();
        }

        if (IsWallSliding && playerVisual != null)
        {
            Vector3 scale = playerVisual.transform.localScale;

            if (touchingLeftWall)
            {
                scale.x = Mathf.Abs(scale.x);
            }
            else if (touchingRightWall)
            {
                scale.x = -Mathf.Abs(scale.x);
            }

            playerVisual.transform.localScale = scale;
        }

        if (
            Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame &&
            !isSliding &&
            !isAttacking
        )
        {
            if (IsTouchingWall && !IsGrounded)
            {
                PerformWallJump();
            }
            else if (IsGrounded)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    jumpForce
                );

                if (animator != null)
                {
                    animator.SetTrigger("Jump");
                }
            }
        }

        if (
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame &&
            !isSliding &&
            !IsPointerOverUI()
        )
        {
            if (isAttacking)
            {
                attackQueued = true;
            }
            else if (attackTimer <= 0f)
            {
                StartCoroutine(Attack());
                attackTimer = attackCooldown;
            }
        }

        if (animator != null)
        {
            animator.SetBool("IsGrounded", IsGrounded);
            animator.SetBool("IsWallSliding", IsWallSliding);
            animator.SetFloat("yVelocity", rb.linearVelocity.y);
        }
    }

    private void HandleMovement()
    {
        float moveInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed)
            {
                moveInput = -1f;
            }

            if (Keyboard.current.dKey.isPressed)
            {
                moveInput = 1f;
            }
        }

        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed,
            rb.linearVelocity.y
        );

        if (animator != null)
        {
            animator.SetBool(
                "IsRunning",
                moveInput != 0f
            );
        }

        if (playerVisual != null)
        {
            if (moveInput > 0f)
            {
                playerVisual.transform.localScale = new Vector3(
                    Mathf.Abs(playerVisual.transform.localScale.x),
                    playerVisual.transform.localScale.y,
                    playerVisual.transform.localScale.z
                );
            }
            else if (moveInput < 0f)
            {
                playerVisual.transform.localScale = new Vector3(
                    -Mathf.Abs(playerVisual.transform.localScale.x),
                    playerVisual.transform.localScale.y,
                    playerVisual.transform.localScale.z
                );
            }
        }
    }

    private void PerformWallJump()
    {
        float jumpDirection;

        if (touchingLeftWall)
        {
            jumpDirection = 1f;
            Debug.Log("Wall jumping right from left wall.");
        }
        else
        {
            jumpDirection = -1f;
            Debug.Log("Wall jumping left from right wall.");
        }

        rb.linearVelocity = new Vector2(
            jumpDirection * wallJumpHorizontalForce,
            wallJumpVerticalForce
        );

        if (playerVisual != null)
        {
            Vector3 scale = playerVisual.transform.localScale;

            if (touchingLeftWall)
            {
                scale.x = Mathf.Abs(scale.x);
            }
            else if (touchingRightWall)
            {
                scale.x = -Mathf.Abs(scale.x);
            }

            playerVisual.transform.localScale = scale;
        }

        if (animator != null)
        {
            animator.SetTrigger("WallJump");
        }

        IsWallSliding = false;
    }

    private IEnumerator Slide()
    {
        isSliding = true;
        slideTimer = slideCooldown;

        SetSlidingCollider();

        if (animator != null)
        {
            animator.SetTrigger("Slide");
        }

        float direction =
            playerVisual != null &&
            playerVisual.transform.localScale.x > 0f
            ? 1f
            : -1f;

        rb.linearVelocity = new Vector2(
            direction * slideSpeed,
            rb.linearVelocity.y
        );

        yield return new WaitForSeconds(slideDuration);

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );

        isSliding = false;
        RestoreStandingCollider();
    }

    private void SetSlidingCollider()
    {
        if (playerCollider == null)
        {
            return;
        }

        float newHeight =
            standingColliderSize.y * slideHeightPercent;

        playerCollider.size = new Vector2(
            standingColliderSize.x,
            newHeight
        );

        float heightDifference =
            standingColliderSize.y - newHeight;

        playerCollider.offset =
            standingColliderOffset +
            new Vector2(
                0f,
                -heightDifference / 2f
            );
    }

    private void RestoreStandingCollider()
    {
        if (playerCollider == null)
        {
            return;
        }

        playerCollider.size = standingColliderSize;
        playerCollider.offset = standingColliderOffset;
    }

    private IEnumerator Attack()
    {
        if (isDead || controlsLocked || Time.timeScale <= 0f)
        {
            yield break;
        }

        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        isAttacking = true;

        if (animator != null)
        {
            animator.SetInteger(
                "AttackIndex",
                attackIndex
            );

            animator.SetTrigger("Attack");
        }

        attackIndex++;

        if (attackIndex >= 3)
        {
            attackIndex = 0;
        }

        // Delay the hitbox so it lines up with the sword animation.
        if (attackHitboxDelay > 0f)
        {
            yield return new WaitForSeconds(
                attackHitboxDelay
            );
        }

        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);
        }

        yield return new WaitForSeconds(
            attackHitboxDuration
        );

        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }

        float remainingAttackTime =
            attackDuration -
            attackHitboxDelay -
            attackHitboxDuration;

        if (remainingAttackTime > 0f)
        {
            yield return new WaitForSeconds(
                remainingAttackTime
            );
        }

        isAttacking = false;

        if (attackQueued && !isDead)
        {
            attackQueued = false;
            attackTimer = attackCooldown;

            StartCoroutine(Attack());
        }
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null &&
               EventSystem.current.IsPointerOverGameObject();
    }

    public void SetControlsLocked(bool locked)
    {
        controlsLocked = locked;

        if (locked)
        {
            isSliding = false;
            isAttacking = false;
            attackQueued = false;

            if (attackHitbox != null)
            {
                attackHitbox.SetActive(false);
            }

            if (rb != null)
            {
                rb.linearVelocity = new Vector2(
                    0f,
                    rb.linearVelocity.y
                );
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(
        int damage,
        Vector2 attackerPosition
    )
    {
        TakeDamage(damage);

        if (!isDead)
        {
            ApplyKnockback(attackerPosition);
        }
    }

    public void ApplyKnockback(Vector2 attackerPosition)
    {
        if (rb == null || isDead)
        {
            return;
        }

        float horizontalDirection = Mathf.Sign(
            transform.position.x -
            attackerPosition.x
        );

        if (horizontalDirection == 0f)
        {
            horizontalDirection = 1f;
        }

        rb.linearVelocity = new Vector2(
            horizontalDirection * knockbackForce,
            knockbackUpwardForce
        );

        knockbackTimer = knockbackDuration;
    }

    public void RestoreFullHealth()
    {
        health = maxHealth;
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        if (
            audioSource != null &&
            youLoseSound != null
        )
        {
            audioSource.PlayOneShot(youLoseSound);
        }

        rb.linearVelocity = Vector2.zero;

        if (deathMessage != null)
        {
            deathMessage.gameObject.SetActive(true);
        }

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(
            deathMessageDuration
        );

        SceneManager.LoadScene("MainMenu");
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundCheckRadius
            );
        }

        if (wallCheckLeft != null)
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawWireCube(
                wallCheckLeft.position,
                wallCheckSize
            );
        }

        if (wallCheckRight != null)
        {
            Gizmos.color = Color.cyan;

            Gizmos.DrawWireCube(
                wallCheckRight.position,
                wallCheckSize
            );
        }
    }
}