using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public Vector2 input;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public bool isGrounded = false;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;

    private Vector3 moveVelocity;
    public enum PlayerState { Idle, Walking}
    public PlayerState currentState = PlayerState.Idle;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 1. Gather input
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // 2. Calculate movement direction relative to the player's local orientation
        Vector3 moveDir = (transform.forward * input.y) + (transform.right * input.x);
        
        // 3. Set target velocity (leaving Y untouched so gravity still works)
        moveVelocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);

        // 4. Check ground status
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // 5. Check for Jump input
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        if (input.magnitude > 0)
        {
            currentState = PlayerState.Walking;
        }
        else
        {
            currentState = PlayerState.Idle;
        }
    }

    void FixedUpdate()
    {
        // 6. Apply the velocity consistently in FixedUpdate
        // Note: Do NOT multiply by Time.deltaTime when setting velocity directly
        rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
    }
}
