using UnityEngine;
using UnityEngine.InputSystem;


public class LyraController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Respawn")]
    public Transform respawnPoint;
    public float fallLimit = -10f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

    }

    void Update()
    {
        if (Time.timeScale == 0f)
        return;
        
        CheckGround();
        Move();
        Jump();
        UpdateAnimationStates();
        CheckFall();
    }

    void Move()
    {
        float horizontal = 0f;

        if (Keyboard.current.aKey.isPressed)
            horizontal = -1f;

        if (Keyboard.current.dKey.isPressed)
            horizontal = 1f;

        rb.linearVelocity = new Vector2(
            horizontal * moveSpeed,
            rb.linearVelocity.y
        );

        if (horizontal < 0)
            spriteRenderer.flipX = true;

        if (horizontal > 0)
            spriteRenderer.flipX = false;

        animator.SetBool("IsRunning", horizontal != 0);
    }

    void Jump()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            animator.SetBool("IsJumping", true);
            animator.SetBool("IsFalling", false);
        }
    }

    void UpdateAnimationStates()
    {
        animator.SetBool("IsRunning", rb.linearVelocity.x != 0);

        if (isGrounded)
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
        }
        else
        {
            if (rb.linearVelocity.y > 0.1f)
            {
                animator.SetBool("IsJumping", true);
                animator.SetBool("IsFalling", false);
            }
            else if (rb.linearVelocity.y < -0.1f)
            {
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsFalling", true);
            }
        }
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    void CheckFall()
    {
        if (transform.position.y < fallLimit)
        {
            Respawn();
        }
    }

    void Respawn()
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = respawnPoint.position;
    }
}