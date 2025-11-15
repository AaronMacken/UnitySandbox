using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Cinemachine;

public class Player : MonoBehaviour {
    // Player State Manager
    private StateMachine stateMachine;
    public PlayerIdleState IdleState;
    public PlayerMoveState MoveState;

    public Rigidbody2D rigidBody;

    [Header("Movement Variables")]
    public float moveSpeed;
    public float jumpForce;
    public float jumpHoldForce;
    public float dashForce;

    [Header("Time Variables")]
    private float jumpTimeCounter;
    public float maxJumpDuration;
    public float dashDuration;
    public float dashCooldown;

    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;
    public Transform groundCheckPoint;

    [Header("Frequently Changing Variables")]
    public bool isGrounded;
    private bool isJumping;
    public bool isDashing;
    public bool isDashOffCooldown;
    public float moveInput;
    private int playerDirection;

    [Header("Camera Falling")]
    public Transform player;
    public Camera mainCamera;
    public CinemachineCamera vCam1;
    public CinemachineCamera vCam2;
    public float maxFallSpeed = -20;
    private bool hasSwitched = false;

    // testing vars
    [Header("Testing")]
    public bool isSlowMotionForTesting;
    public Vector2 rigidBodyVelocity;

    void Awake() {
        stateMachine = new StateMachine();
        IdleState = new PlayerIdleState();
        MoveState = new PlayerMoveState();

        playerDirection = 1;
        moveSpeed = 10f;

        jumpForce = 20f;
        jumpHoldForce = 50f;
        maxJumpDuration = .3f;

        dashForce = 20f;
        dashCooldown = 0.5f;
        dashDuration = .2f;
        isDashOffCooldown = true;
    }

    void Start() {
        rigidBody = GetComponent<Rigidbody2D>();
        stateMachine.ChangeState(IdleState, this);
    }

    void Update() {
        stateMachine.Tick();

        Debug.DrawRay(player.position, Vector2.right * playerDirection * 1f, Color.white);

        Vector3 viewPosition = mainCamera.WorldToViewportPoint(player.position);

        if (!hasSwitched && viewPosition.y < 0f) {
            hasSwitched = true;
            vCam1.Priority = 0;
            vCam2.Priority = 1;
        }
    }

    void FixedUpdate() {
        stateMachine.FixedTick();
        CheckGrounded();

        RaycastHit2D wallHit = Physics2D.Raycast(player.position, Vector2.right * playerDirection, 0.1f, groundLayer);

        // Can I delete this?
        if (wallHit.collider != null && rigidBody.linearVelocity.y < 0) {
            // We're sliding down while touching a wall → nudge slightly away
            rigidBody.position += Vector2.left * playerDirection * 0.01f;
        }


        Time.timeScale = isSlowMotionForTesting ? 0.2f : 1f;

        if (hasSwitched) {
            rigidBody.MoveRotation(rigidBody.rotation - 360f * Time.fixedDeltaTime);
        }

        if (isDashing) {
            rigidBody.gravityScale = 0;
            rigidBody.linearVelocity = new Vector2(playerDirection * dashForce, 0f);
        }

        if (jumpTimeCounter == 0) {
            isJumping = false;
        }

        if (isJumping && jumpTimeCounter >= 0) {
            rigidBody.AddForce(Vector2.up * jumpHoldForce, ForceMode2D.Force);

            // measurement of time since the last physics step
            jumpTimeCounter = Mathf.Max(0, jumpTimeCounter - Time.fixedDeltaTime);
        }

        if (!isJumping && !isDashing) {
            rigidBody.gravityScale = 20;
        }

        rigidBodyVelocity = rigidBody.linearVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Debug.Log("Collided with: " + collision.gameObject.name);
    }

    public void OnJump(InputAction.CallbackContext context) {
        if (context.started && isGrounded) {
            isGrounded = false;
            isJumping = true;
            jumpTimeCounter = maxJumpDuration;
            rigidBody.gravityScale = 10;
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        if (context.canceled) {
            isJumping = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context) {
        moveInput = context.ReadValue<Vector2>().x;
        if (moveInput > 0) playerDirection = 1;
        if (moveInput < 0) playerDirection = -1;
    }

    public void OnDash(InputAction.CallbackContext context) {
        if (context.performed && isDashOffCooldown) {
            isDashing = true;
            isDashOffCooldown = false;
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash() {
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }

    // -- Raycasting Logic -- //
    private void CheckGrounded() {
        RaycastHit2D hit = Physics2D.Raycast(
            groundCheckPoint.position,   // start position (your GroundCheck object)
            Vector2.down,                // direction to cast
            groundCheckDistance,         // how far to check
            groundLayer                  // only check "Ground" layer
        );

        bool hasLanded = hit.collider != null;

        if (hasLanded) {
            isDashOffCooldown = hit.collider != null;
            isGrounded = hit.collider != null;
        }
    }

    // -- Test callback to draw the draw in editor -- //
    private void OnDrawGizmos() {
        if (groundCheckPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            groundCheckPoint.position,
            groundCheckPoint.position + Vector3.down * groundCheckDistance * 2f
        );
    }
}
