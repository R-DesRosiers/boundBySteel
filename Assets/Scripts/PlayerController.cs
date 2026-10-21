using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
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

    [Header("Wall Slide Settings")]
    public Transform wallCheckLeft;
    public Transform wallCheckRight;
    public float wallCheckRadius = 0.15f;
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

    [Header("Attack Hitbox")]
    public GameObject attackHitbox;
    public float attackHitboxDuration = 0.15f;
    public float attackDuration = 0.6f;

    [Header("Ground Check Setup")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.15f;
    public bool IsGrounded;

    // Private state variables
    private bool isSliding = false;
    private float slideTimer = 0f;

    private bool isAttacking = false;
    private bool attackQueued = false;
    private float attackTimer = 0f;
    private int attackIndex = 0;

    private bool isDead = false;
    private bool previousWallState = false;

    private bool touchingLeftWall = false;
    private bool touchingRightWall = false;

    // Components
    private Animator animator;
    private Rigidbody2D rb;
    private Vector3 startPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

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

        // Ground detection
        IsGrounded =
            groundCheck != null &&
            Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );

        // Left wall detection
        touchingLeftWall =
            wallCheckLeft != null &&
            Physics2D.OverlapCircle(
                wallCheckLeft.position,
                wallCheckRadius,
                wallLayer
            );

        // Right wall detection
        touchingRightWall =
            wallCheckRight != null &&
            Physics2D.OverlapCircle(
                wallCheckRight.position,
                wallCheckRadius,
                wallLayer
            );

        // The player is touching a wall if either check detects one
        IsTouchingWall = touchingLeftWall || touchingRightWall;

        // Debug wall contact
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

        // Determine whether the player is wall sliding
        IsWallSliding =
            IsTouchingWall &&
            !IsGrounded &&
            rb.linearVelocity.y < 0f &&
            !isAttacking &&
            !isSliding;

        // Limit downward speed while wall sliding
        if (IsWallSliding)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                -wallSlideSpeed
            );
        }

        // Update timers
        attackTimer -= Time.deltaTime;
        slideTimer -= Time.deltaTime;

        // Ground slide
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

        // Normal movement
        if (!isSliding)
        {
            HandleMovement();
        }

        // Mirror the visual based on which wall is being touched
        if (IsWallSliding && playerVisual != null)
        {
            Vector3 scale = playerVisual.transform.localScale;

            if (touchingLeftWall)
            {
                // Wall is on the left, so face left
                scale.x = Mathf.Abs(scale.x);
            }
            else if (touchingRightWall)
            {
                // Wall is on the right, so face right
                scale.x = -Mathf.Abs(scale.x);
            }

            playerVisual.transform.localScale = scale;
        }

        // Jump input
        if (
            Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame &&
            !isSliding &&
            !isAttacking
        )
        {
            // Wall jump
            if (IsTouchingWall && !IsGrounded)
            {
                PerformWallJump();
            }
            // Normal ground jump
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

        // Attack input
        if (
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame &&
            !isSliding
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

        // Animator parameters
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

        // Face movement direction when normally moving
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

        // If touching the left wall, jump right
        if (touchingLeftWall)
        {
            jumpDirection = 1f;
            Debug.Log("wall jumping right from left wall");
        }
        // If touching the right wall, jump left
        else
        {
            jumpDirection = -1f;
            Debug.Log("wall jumping left from right wall");
        }

        rb.linearVelocity = new Vector2(
            jumpDirection * wallJumpHorizontalForce,
            wallJumpVerticalForce
        );

        // Face away from the wall after jumping
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
    }

    private IEnumerator Attack()
    {
        isAttacking = true;

        if (animator != null)
        {
            animator.SetInteger("AttackIndex", attackIndex);
            animator.SetTrigger("Attack");
        }

        attackIndex++;

        if (attackIndex >= 3)
        {
            attackIndex = 0;
        }

        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);
        }

        float elapsed = 0f;

        while (elapsed < attackHitboxDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }

        float remainingAttackTime =
            attackDuration - attackHitboxDuration;

        if (remainingAttackTime > 0f)
        {
            yield return new WaitForSeconds(remainingAttackTime);
        }

        isAttacking = false;

        if (attackQueued && !isDead)
        {
            attackQueued = false;
            attackTimer = attackCooldown;

            StartCoroutine(Attack());
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

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        rb.linearVelocity = Vector2.zero;

        if (deathMessage != null)
        {
            deathMessage.gameObject.SetActive(true);
        }

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(deathMessageDuration);

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

            Gizmos.DrawWireSphere(
                wallCheckLeft.position,
                wallCheckRadius
            );
        }

        if (wallCheckRight != null)
        {
            Gizmos.color = Color.cyan;

            Gizmos.DrawWireSphere(
                wallCheckRight.position,
                wallCheckRadius
            );
        }
    }
}