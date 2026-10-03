using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


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
    private bool isDead = false;
    private Coroutine blinkCoroutine;
    public GameObject deathEffect;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool mobileJumpPressed = false;

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

        if (isDead)
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

        // Keyboard
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed)
                horizontal = -1f;

            if (Keyboard.current.dKey.isPressed)
                horizontal = 1f;
        }

        // Mobile joystick
        if (MobileInputProvider.Instance != null)
        {
            float mobileHorizontal =
                MobileInputProvider.Instance.MoveInput.x;

            if (Mathf.Abs(mobileHorizontal) > 0.1f)
                horizontal = mobileHorizontal;
        }

        rb.linearVelocity = new Vector2(
            horizontal * moveSpeed,
            rb.linearVelocity.y
        );

        if (horizontal < 0)
            spriteRenderer.flipX = true;

        if (horizontal > 0)
            spriteRenderer.flipX = false;

        animator.SetBool("IsRunning", Mathf.Abs(horizontal) > 0.1f);
    }

    void Jump()
    {
        bool keyboardJump =
            Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame;

        bool jumpPressed =
            keyboardJump || mobileJumpPressed;

        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            animator.SetBool("IsJumping", true);
            animator.SetBool("IsFalling", false);

            Debug.Log("JUMP!");
        }

        // Reset the mobile input after checking it
        mobileJumpPressed = false;
    }

    public void MobileJump()
    {
        mobileJumpPressed = true;
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
            Die();
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        rb.linearVelocity = Vector2.zero;

        // Fade Lyra out
        StartCoroutine(FadeOut());

        // Play death explosion
        Instantiate(
            deathEffect,
            transform.position,
            transform.rotation
        );

        StartCoroutine(RespawnAfterDeath());
    }

    private IEnumerator FadeOut()
    {
        float duration = 0.15f;
        float elapsed = 0f;

        Color originalColor = spriteRenderer.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float alpha = Mathf.Lerp(
                1f,
                0f,
                elapsed / duration
            );

            spriteRenderer.color = new Color(
                originalColor.r,
                originalColor.g,
                originalColor.b,
                alpha
            );

            yield return null;
        }

        spriteRenderer.color = new Color(
            originalColor.r,
            originalColor.g,
            originalColor.b,
            0f
        );
    }

    private IEnumerator RespawnAfterDeath()
    {
        yield return new WaitForSeconds(0.7f);

        if (GameManager.Instance != null &&
            GameManager.Instance.HasCheckpoint())
        {
            transform.position =
                GameManager.Instance.GetRespawnPosition();
        }
        else if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
        }

        rb.linearVelocity = Vector2.zero;

        yield return StartCoroutine(BlinkIn());

        isDead = false;
    }

    private IEnumerator BlinkIn()
    {
        Color originalColor = spriteRenderer.color;

        int blinks = 3;
        float blinkTime = 0.08f;

        for (int i = 0; i < blinks; i++)
        {
            spriteRenderer.color = new Color(
                originalColor.r,
                originalColor.g,
                originalColor.b,
                0f
            );

            yield return new WaitForSeconds(blinkTime);

            spriteRenderer.color = new Color(
                originalColor.r,
                originalColor.g,
                originalColor.b,
                1f
            );

            yield return new WaitForSeconds(blinkTime);
        }

        spriteRenderer.color = new Color(
            originalColor.r,
            originalColor.g,
            originalColor.b,
            1f
        );
    }
}