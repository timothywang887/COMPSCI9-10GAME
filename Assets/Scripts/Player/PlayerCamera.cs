using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

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

        mouseX = Input.GetAxis("Mouse X") * sensitivity;
        mouseY = Input.GetAxis("Mouse Y") * sensitivity;
        playerState = playerTransform.GetComponent<PlayerMovement>().currentState;
        if (playerState == PlayerMovement.PlayerState.Idle)
        {
            float wobbleY = Mathf.PerlinNoise(Time.time * wobbleSpeed, 0f);
            wobbleY = wobbleY * 2 - 1;
            wobbleY = wobbleY * wobbleAmount;
            mouseY += wobbleY;
            //camera wobbles randomly with perlin noise
            float wobble = Mathf.PerlinNoise(Time.time + 10000f * wobbleSpeed, 0f);
            wobble = wobble * 2 - 1;
            wobble = wobble * wobbleAmount;
            mouseX += wobble;
        }
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

    }

    void LateUpdate()
    {
   
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

       
        if (playerRb != null)
        {
            Quaternion deltaRotation = Quaternion.Euler(0f, mouseX, 0f);
            playerRb.MoveRotation(playerRb.rotation * deltaRotation);
            //if (Mathf.Abs(mouseX) > 1.0f)
            //{
                Debug.Log("Sharp turn detected! MouseX: " + mouseX);

                float targetRoll = Mathf.Clamp((-mouseX) * 75f, -20f, 20f);

               
                currentZRoll = Mathf.Lerp(currentZRoll, targetRoll, 10f * Time.deltaTime);

              
                transform.localRotation = Quaternion.Euler(xRotation, 0f, currentZRoll);
           // }

        }
        else
        {
        Debug.LogError("No Player Rigidbody!");
        }
    }
}
