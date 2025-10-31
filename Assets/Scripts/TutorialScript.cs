using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialScript : MonoBehaviour {
    private Rigidbody2D rigidBody;
    private float moveInput;

    void Start() {
        rigidBody = GetComponent<Rigidbody2D>();
        Debug.Log("WTF");
    }

    void FixedUpdate() {
        rigidBody.linearVelocity = new Vector2(moveInput * 10f, rigidBody.linearVelocityY);
    }

    public void OnMove(InputAction.CallbackContext context) {
        if (context.started) {
            moveInput = context.ReadValue<Vector2>().x;
        }
    }

    public void OnJump(InputAction.CallbackContext context) {
        if (context.started) {
            Debug.Log("Jump was clicked!");
        }
    }

    public void OnDash(InputAction.CallbackContext context) {
        if (context.started) {
            Debug.Log("Jump was clicked!");
        }
    }
}
