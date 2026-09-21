// IMPORTANT: This script should be deleted, and its functionality moved to either Character.cs or Enemy.cs, as applicable.
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float Health = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(5f);
            print("space");
        }
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        print(Health);
        if (Health <= 0)
        {
            Die();
        }
    }


    public void Die()
    {
        Destroy(gameObject);
    }
}
