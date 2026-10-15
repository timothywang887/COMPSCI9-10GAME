using UnityEngine;

public class Sword : MonoBehaviour
{
   [Header("Default")]


    public Vector3 defaultAngles;
   public Vector3 defaultPosition;


   [Header("Swing")]
   public Vector3 swingStartAngles;
   public Vector3 swingEndAngles;
   public float swingSpeed = 12f;
   public float swingTime = 1f;  
   public Vector3 swingStartPosition;
   public Vector3 swingEndPosition;
   public float swingMoveSpeed = 10;


   [Header("Jab")]
   public Vector3 jabStartAngles;
   public Vector3 jabEndAngles;
   public float jabSpeed = 12f;
   public float jabTime = 1f;  
   public Vector3 jabStartPosition;
   public Vector3 jabEndPosition;
   public float jabMoveSpeed = 10;

      [Header("Block")]
   public Vector3 blockStartAngles;
   public Vector3 blockEndAngles;
   public float blockSpeed = 12f;
   public float blockTime = 1f;  
   public Vector3 blockStartPosition;
   public Vector3 blockEndPosition;
   public float blockMoveSpeed = 10;






   float timer = 0;
   int attack = 0;
   float speed = 1;
   float movespeed = 1;
   float end = 0;
   private Vector3 targetPosition;
   private Quaternion swingStartRotation;
   private Quaternion swingEndRotation;
   private Quaternion jabStartRotation;
   private Quaternion jabEndRotation;

    private Quaternion blockStartRotation;
   private Quaternion blockEndRotation;
   private Quaternion targetRotation;
   private Quaternion defaultRotation;


   void Start()
   {
        swingStartRotation = Quaternion.Euler(swingStartAngles);
        swingEndRotation = Quaternion.Euler(swingEndAngles);
        jabStartRotation = Quaternion.Euler(jabStartAngles);
        jabEndRotation = Quaternion.Euler(jabEndAngles);
        defaultRotation = Quaternion.Euler(defaultAngles);


        transform.localRotation =defaultRotation;
        transform.localPosition = defaultPosition;
        targetRotation = defaultRotation;
        targetPosition = defaultPosition;
   }




   void Update()
   {
        timer -= Time.deltaTime;


       swingStartRotation = Quaternion.Euler(swingStartAngles);
       swingEndRotation = Quaternion.Euler(swingEndAngles);
       jabStartRotation = Quaternion.Euler(jabStartAngles);
       jabEndRotation = Quaternion.Euler(jabEndAngles);
       defaultRotation = Quaternion.Euler(defaultAngles);
       blockStartRotation = Quaternion.Euler(blockStartAngles);
       blockEndRotation = Quaternion.Euler(blockEndAngles);
       


       transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, speed * Time.deltaTime);
       transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, movespeed * Time.deltaTime);


       if (Input.GetMouseButtonDown(0))
       {
            attack = 1;
       targetRotation = swingStartRotation;
       targetPosition = swingStartPosition;
       print("swing");
       speed = swingSpeed;
       timer = swingTime;
              movespeed = swingMoveSpeed;
       }
       if (Input.GetMouseButtonDown(2))
       {
       attack = 2;
       targetRotation = jabStartRotation;
       targetPosition = jabStartPosition;
       print("jab");
       speed = jabSpeed;
       timer = jabTime;
       movespeed = jabMoveSpeed;
       }

       if (Input.GetMouseButtonDown(1))
       {
       attack = 3;
       targetRotation = blockStartRotation;
       targetPosition = blockStartPosition;
       print("block");
       speed = blockSpeed;
       timer = blockTime;
       movespeed = blockMoveSpeed;
       }
      
       if (attack == 1)
       {
                      if (timer<0)
           {
               if (end == 1)
               {
                   end = 0;
                   attack = 0;
                   targetPosition = defaultPosition;
                   targetRotation = defaultRotation;
               }
               else
               {
                   end = 1;
                   targetPosition = swingEndPosition;
                   targetRotation = swingEndRotation;
                   timer = swingTime;
               }
           }
       }
       if (attack == 2)
       {
           if (timer<0)
           {
               if (end == 1)
               {
                   end = 0;
                   attack = 0;
                   targetPosition = defaultPosition;
                   targetRotation = defaultRotation;
               }
               else
               {
                   end = 1;
                   targetPosition = jabEndPosition;
                   targetRotation = jabEndRotation;
                   timer = jabTime;
               }
           }
        }
        if (attack == 3)
       {
           if (timer<0)
           {
               if (end == 1)
               {
                   end = 0;
                   attack = 0;
                   targetPosition = defaultPosition;
                   targetRotation = defaultRotation;
               }
               else
               {
                   end = 1;
                   targetPosition = blockEndPosition;
                   targetRotation = blockEndRotation;
                   timer = blockTime;
               }
           }
       }
   }


}
