using UnityEngine;

public class Sword : MonoBehaviour
{

    public Vector3 restAngles;
    public Vector3 swingAngles; 
    public float swingSpeed = 12f;

    public float swingTime = 1f;   
    float timer = 0;
  

    private Quaternion restRotation;
    private Quaternion swingRotation;
    private Quaternion targetRotation;

    void Start()
    {
        restRotation = Quaternion.Euler(restAngles);
        swingRotation = Quaternion.Euler(swingAngles);

        transform.localRotation = restRotation;
        targetRotation = restRotation;
    }


    void Update()
    {
        restRotation = Quaternion.Euler(restAngles);
        swingRotation = Quaternion.Euler(swingAngles);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, swingSpeed * Time.deltaTime);

        if (Input.GetMouseButtonDown(0))
        {
        targetRotation = swingRotation;
        print("swing");
        timer = swingTime;
        } 
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            targetRotation = restRotation;
        }
    }
}
