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

    private PlayerMovement.PlayerState playerState;
    private float currentZRoll = 0f;

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
            // Base the roll target on the smoothed turn velocity rather than raw spiking frame input
            float targetRoll = Mathf.Clamp((-smoothTurnVelocity) * 75f, -200f, 200f);

            // True frame-rate independent lerp using decay
            float rollLerpSpeed = 8f;
            float decayFactor = 1.0f - Mathf.Exp(-rollLerpSpeed * Time.deltaTime);
            currentZRoll = Mathf.Lerp(currentZRoll, targetRoll, decayFactor);

            // Apply orientation cleanly
            transform.localRotation = Quaternion.Euler(xRotation, 0f, currentZRoll);
        }
        else
        {
            Debug.LogError("No Player Rigidbody!");
        }
    }
}
