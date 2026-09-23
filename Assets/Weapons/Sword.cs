using UnityEngine;

public class Sword : MonoBehaviour
{
    float atkdr = 0;
    float slash = 0;
    float atkrt = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float atkdr = 0.15f;
        float slash = 0;
        float atkrt = 0;
    }

    // Update is called once per frame
    void Update()
    {
       if (Input.GetMouseButtonDown(0) && atkrt <= 0)
       {
        slash=1;
        atkrt=0.5f;
       }
       if (atkrt > 0)
       { 
           atkrt -= Time.deltaTime;
       }
        if (slash > 0)
        {
            atkdr -= Time.deltaTime;
            print("Swash");
        }
        if (atkdr <= 0)
        {
            slash = 0;
            atkdr = 0.15f;
        }
    }
}
