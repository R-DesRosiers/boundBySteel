using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public int health = 5; 
    public float attackCooldown = 0.4f;
    private float attackTimer = 0f;
    public bool isGrounded; 
    private Rigidbody2D rb;
    private Vector3 startPosition;
    public GameObject sword; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position; 
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;
        float moveInput = 0f;

        if (Keyboard.current.aKey.isPressed)
            moveInput = -1f;

        if (Keyboard.current.dKey.isPressed)
            moveInput = 1f;

        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed,
            rb.linearVelocity.y
        );

        if (moveInput > 0)
        {
            transform.localScale = new Vector3(0.75f, 1.5f, 1f);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-0.75f, 1.5f, 1f);
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );
        }
        if (Mouse.current.leftButton.wasPressedThisFrame && attackTimer <= 0f)
        {
            StartCoroutine(Attack());
            attackTimer = attackCooldown;
        }
    }
    IEnumerator Attack()
    {
        sword.SetActive(true);

        yield return new WaitForSeconds(0.15f);

        sword.SetActive(false);
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
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