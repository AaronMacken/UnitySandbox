using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;
    public float dashForce;
    public float dashDuration;
    public float dashCooldown;

    public float maxJumpDuration;
    public float jumpHoldForce;

    private Rigidbody2D rigidBody;
    private bool isGrounded;
    private float moveInput;
    private int playerDirection;
    private bool isDashing = false;
    private bool canDash = true;

    private bool isJumping = false;
    private float jumpTimeCounter;

    void Awake()
    {
        moveSpeed = 5f;
        jumpForce = 5f;
        dashForce = 20f;
        dashDuration = 0.25f;
        dashCooldown = 0.5f;
        maxJumpDuration = 1f;
        jumpHoldForce = 5f;
        playerDirection = 1;
    }

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            rigidBody.linearVelocity = new Vector2(moveInput * moveSpeed, rigidBody.linearVelocity.y);
        }

        if (isJumping && jumpTimeCounter > 0)
        {
            rigidBody.AddForce(Vector2.up * jumpHoldForce, ForceMode2D.Force);
            jumpTimeCounter -= Time.fixedDeltaTime; // measurement of time since the last physics step
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>().x;
        if (moveInput > 0) playerDirection = 1;
        else if (moveInput < 0) playerDirection = -1;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded) // pressed jump
        {
            isJumping = true;
            isGrounded = false;
            rigidBody.gravityScale = 0;
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpTimeCounter = maxJumpDuration;
        }
        else if (context.canceled) // released jump
        {
            isJumping = false;
            rigidBody.gravityScale = 3;
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        float originalGravity = rigidBody.gravityScale;

        rigidBody.gravityScale = 0;
        rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, 0);

        canDash = false;
        isDashing = true;

        rigidBody.linearVelocity = new Vector2(playerDirection * dashForce, 0f);

        yield return new WaitForSeconds(dashDuration);

        rigidBody.gravityScale = originalGravity;
        isDashing = false;

        if (isGrounded)
        {
            canDash = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            canDash = true;
            rigidBody.gravityScale = 0;
        }
    }
}
