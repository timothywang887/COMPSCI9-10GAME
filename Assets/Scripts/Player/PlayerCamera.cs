using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float sensitivity = 2f;

    [SerializeField] private Transform playerTransform;
    private Rigidbody playerRb;

    private float accumulatedMouseX = 0f;
    private float accumulatedMouseY = 0f;
    private float smoothTurnVelocity = 0f; // Used to damp the target roll generation

    private float xRotation = 0f;

    public float wobbleSpeed = 1f;
    public float wobbleAmount = 0.1f;

    [Header("Movement Tilt Settings")]
    public float tiltAmountX = 2f;        // Pitch tilt intensity (forward/backward)
    public float tiltAmountY = 2f;        // Vertical movement tilt intensity (jumping/falling)
    public float tiltAmountZ = 3f;        // Roll tilt intensity (strafing left/right)
    public float movementTiltSpeed = 5f;  // Speed of the tilt reaction

    private PlayerMovement.PlayerState playerState;
    private float currentZRoll = 0f;

    // Smoothed movement tilt values
    private float smoothTiltX = 0f;
    private float smoothTiltZ = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (playerTransform != null)
        {
            playerRb = playerTransform.GetComponent<Rigidbody>();
        }
    }

    void Update()
    {
        // Gather raw frame inputs
        float rawMouseX = Input.GetAxis("Mouse X") * sensitivity;
        float rawMouseY = Input.GetAxis("Mouse Y") * sensitivity;

        if (playerTransform != null)
        {
            playerState = playerTransform.GetComponent<PlayerMovement>().currentState;
            if (playerState == PlayerMovement.PlayerState.Idle)
            {
                float wobbleY = Mathf.PerlinNoise(Time.time * wobbleSpeed, 0f);
                wobbleY = (wobbleY * 2f - 1f) * wobbleAmount;
                rawMouseY += wobbleY;

                float wobbleX = Mathf.PerlinNoise((Time.time + 10000f) * wobbleSpeed, 0f);
                wobbleX = (wobbleX * 2f - 1f) * wobbleAmount;
                rawMouseX += wobbleX;
            }
        }

        // Smooth out the turning speed itself so the target roll doesn't spike aggressively
        float turnSmoothSpeed = 15f; 
        smoothTurnVelocity = Mathf.Lerp(smoothTurnVelocity, rawMouseX, 1.0f - Mathf.Exp(-turnSmoothSpeed * Time.deltaTime));

        // Accumulate for physics use
        accumulatedMouseX += rawMouseX;
        accumulatedMouseY += rawMouseY;
    }

    void FixedUpdate()
    {
        // Consume accumulated inputs for the physics loop
        float appliedMouseX = accumulatedMouseX;
        float appliedMouseY = accumulatedMouseY;

        accumulatedMouseX = 0f;
        accumulatedMouseY = 0f;

        xRotation -= appliedMouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (playerRb != null)
        {
            Quaternion deltaRotation = Quaternion.Euler(0f, appliedMouseX, 0f);
            playerRb.MoveRotation(playerRb.rotation * deltaRotation);
        }
    }

    void LateUpdate()
    {
        if (playerRb != null)
        {
            // 1. Calculate the local velocity of the player Rigidbody using Vector3
            Vector3 localVelocity = playerTransform.InverseTransformDirection(playerRb.linearVelocity);

            // 2. Map velocities to target tilts
            // Combined Pitch: Forward/backward movement (.z) AND vertical jumping/falling (.y)
            float targetTiltX = (localVelocity.z * tiltAmountX) + (localVelocity.y * tiltAmountY); 
            
            // Strafing right (positive X velocity) rolls the camera left (negative Z tilt)
            float targetTiltZ = -localVelocity.x * tiltAmountZ;

            // 3. Smooth the movement tilts safely using frame-rate independent exponential decay
            float tiltDecayFactor = 1.0f - Mathf.Exp(-movementTiltSpeed * Time.deltaTime);
            smoothTiltX = Mathf.Lerp(smoothTiltX, targetTiltX, tiltDecayFactor);
            smoothTiltZ = Mathf.Lerp(smoothTiltZ, targetTiltZ, tiltDecayFactor);

            // 4. Handle Mouse Turning Z-Roll (Mouse roll combined with movement roll)
            float targetMouseRoll = Mathf.Clamp((-smoothTurnVelocity) * 75f, -200f, 200f) * 0.5f;
            float rollLerpSpeed = 8f;
            Debug.Log(Mathf.Exp(-rollLerpSpeed * Time.deltaTime));
            float rollDecayFactor = 1f - Mathf.Exp(-rollLerpSpeed * Time.deltaTime);
            currentZRoll = Mathf.Lerp(currentZRoll, targetMouseRoll, rollDecayFactor);

            // 5. Combine everything cleanly into the final local rotation
            transform.localRotation = Quaternion.Euler(xRotation + smoothTiltX, 0f, currentZRoll + smoothTiltZ);
        }
        else
        {
            Debug.LogError("No Player Rigidbody!");
        }
    }
}
