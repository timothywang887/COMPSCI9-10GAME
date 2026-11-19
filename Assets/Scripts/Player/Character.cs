using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private Mesh model;
    [SerializeField] private Collider collider;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetHealth()
    {
        return health;
    }

    public void SetHealth(float value)
    {
        health = value;
    }
}
