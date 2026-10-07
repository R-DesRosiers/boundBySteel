using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public GameObject playerVisual; 
    public float jumpForce = 8f;

    [Header("Dodge / Dash Settings")]
    public float dodgeSpeed = 12f;
    public float dodgeDuration = 0.2f;
    public float dodgeCooldown = 1f;

    [Header("Combat & Health")]
    public int health = 5;
    public float attackCooldown = 0.4f;
    private float attackTimer = 0f;
    public GameObject sword;

    [Header("Ground Check Setup")]
    public Transform groundCheck;
    public LayerMask groundLayer; 
    public float groundCheckRadius = 0.15f; 
    public bool IsGrounded;

    // Private State Variables
    private bool isDodging = false;
    private float dodgeTimer = 0f;
    private bool isAttacking = false;

    // Components
    private Animator animator; 
    private Rigidbody2D rb;
    private Vector3 startPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        
        // Fetch animator attached to the visual child
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // 1. Perform Ground Check
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 2. Update Timers
        attackTimer -= Time.deltaTime;
        dodgeTimer -= Time.deltaTime;

        // 3. Dodge / Dash Action
        if (Keyboard.current != null && Keyboard.current.shiftKey.wasPressedThisFrame && !isDodging && dodgeTimer <= 0f)
        {
            StartCoroutine(Dodge());
        }

        // 4. Movement Handling (Only when not dodging)
        if (!isDodging)
        {
            float moveInput = 0f;

            if (Keyboard.current != null)
            {
                if (Keyboard.current.aKey.isPressed) moveInput = -1f;
                if (Keyboard.current.dKey.isPressed) moveInput = 1f;
            }

            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

            animator.SetBool("IsRunning", moveInput != 0f);

            // Turn the player visual (fixed to scale playerVisual instead of transform)
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

        // 5. Jump Action
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && IsGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // 6. Attack Action
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && attackTimer <= 0f && !isAttacking)
        {
            StartCoroutine(Attack());
            attackTimer = attackCooldown;
        }

        // 7. Update Animator State Parameters
        animator.SetBool("IsGrounded", IsGrounded);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    IEnumerator Dodge()
    {
        isDodging = true;
        dodgeTimer = dodgeCooldown;

        // Trigger Dash animation state
        animator.SetTrigger("Dash");

        // Direction based on the local scale of playerVisual
        float direction = playerVisual.transform.localScale.x > 0 ? 1f : -1f;

        rb.linearVelocity = new Vector2(direction * dodgeSpeed, rb.linearVelocity.y);

        yield return new WaitForSeconds(dodgeDuration);

        isDodging = false;
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");

        float targetAngle = GetMouseAngle();
        float elapsed = 0f;
        float swingDuration = 0.15f;

        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / swingDuration;
            float angle = Mathf.Lerp(targetAngle - 45f, targetAngle + 45f, t);

            yield return null;
        }

        isAttacking = false;
    }

    float GetMouseAngle()
    {
        if (Mouse.current == null || Camera.main == null) return 0f;

        Vector3 mousePosition = Mouse.current.position.ReadValue();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 direction = mousePosition - transform.position;

        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Respawn();
        }
    }

    void Respawn()
    {
        health = 5;
        transform.position = startPosition;
        rb.linearVelocity = Vector2.zero;
    }
}