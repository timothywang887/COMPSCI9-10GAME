using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    // REDUCE THIS IN THE INSPECTOR (Try 2f to 5f) since we removed Time.deltaTime
    public float sensitivity = 2f; 
    
    [SerializeField] private Transform playerTransform;
    private Rigidbody playerRb;
    
    private float mouseX;
    private float mouseY;
    private float xRotation = 0f;

    public float wobbleSpeed = 1f;
    public float wobbleAmount = 0.1f;

    private float wobbleTime = 0f;
    private PlayerMovement.PlayerState playerState;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        // Grab the Rigidbody from the player target
        if (playerTransform != null)
        {
            playerRb = playerTransform.GetComponent<Rigidbody>();
            playerState = playerTransform.GetComponent<PlayerMovement>().currentState;
        }
    }

    void Update()
    {
        // Gather raw frame input without compounding delta time
        mouseX = Input.GetAxis("Mouse X") * sensitivity;
        mouseY = Input.GetAxis("Mouse Y") * sensitivity;
        if (playerState == PlayerMovement.PlayerState.Idle)
        {
        float wobbleY = Mathf.PerlinNoise(Time.time * wobbleSpeed, 0f);
        wobbleY = wobbleY * 2 - 1;
        wobbleY = wobbleY * wobbleAmount;
        mouseY += wobbleY;
        //camera wobbles randomly at a speed, perlin noise
        float wobble = Mathf.PerlinNoise(Time.time+10000f * wobbleSpeed, 0f);
        wobble = wobble * 2 - 1;
        wobble = wobble * wobbleAmount;
        mouseX += wobble;
        }
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

    }

    void LateUpdate()
    {
        // 1. Instantly apply camera pitch (Up/Down) since the camera has no Rigidbody
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 2. Safely rotate the player Rigidbody (Left/Right) using physics MoveRotation.
        // This allows Unity's "Interpolate" setting to actually smooth out the camera tracking!
        if (playerRb != null)
        {
            Quaternion deltaRotation = Quaternion.Euler(0f, mouseX, 0f);
            playerRb.MoveRotation(playerRb.rotation * deltaRotation);
        }
        else
        {
            // Fallback if no Rigidbody is attached
            playerTransform.Rotate(0f, mouseX, 0f);
        }
    }
}
