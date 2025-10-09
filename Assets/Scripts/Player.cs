using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Player : MonoBehaviour {
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

    void Awake() {
        moveSpeed = 10f;
        jumpForce = 20f;
        jumpHoldForce = 50f;
        dashForce = 20f;
        dashDuration = 0.15f;
        dashCooldown = 0.5f;
        maxJumpDuration = .3f;
        playerDirection = 1;
    }

    void Start() {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        if (!isDashing) {
            rigidBody.linearVelocity = new Vector2(moveInput * moveSpeed, rigidBody.linearVelocity.y);
        }

        if (jumpTimeCounter == 0) {
            ResetJumpingValues();
        }

        if (isJumping && jumpTimeCounter >= 0) {
            rigidBody.AddForce(Vector2.up * jumpHoldForce, ForceMode2D.Force);
            jumpTimeCounter = Mathf.Max(0, jumpTimeCounter - Time.fixedDeltaTime); // measurement of time since the last physics step
        }
    }

    public void OnJump(InputAction.CallbackContext context) {
        if (context.started && isGrounded) {
            isJumping = true;
            isGrounded = false;
            SetAscendingGravity();
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpTimeCounter = maxJumpDuration;
        }

        if (context.canceled) {
            ResetJumpingValues();
        }
    }

    public void OnDash(InputAction.CallbackContext context) {
        if (context.performed && canDash) {
            StartCoroutine(Dash());
        }
    }

    public void OnMove(InputAction.CallbackContext context) {
        moveInput = context.ReadValue<Vector2>().x;
        if (moveInput > 0) playerDirection = 1;
        else if (moveInput < 0) playerDirection = -1;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            ResetVerticalVelocity();
            isGrounded = true;
            canDash = true;
            SetAscendingGravity();
        }
    }

    private IEnumerator Dash() {
        rigidBody.gravityScale = 0;
        // ResetVerticalVelocity();

        canDash = false;
        isDashing = true;

        rigidBody.linearVelocity = new Vector2(playerDirection * dashForce, 0f);

        yield return new WaitForSeconds(dashDuration);

        rigidBody.gravityScale = 20;
        isDashing = false;

        if (isGrounded) {
            canDash = true;
        }
    }

    private void SetAscendingGravity() {
        rigidBody.gravityScale = 10;
    }

    private void ResetVerticalVelocity() {
        rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, 0);
    }

    private void ResetJumpingValues() {
        // rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, 0);
        isJumping = false;
        rigidBody.gravityScale = 20;
    }
}
