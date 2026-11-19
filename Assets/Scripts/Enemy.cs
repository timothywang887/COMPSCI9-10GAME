using UnityEngine;
using UnityEngine.AI;

public class Enemy : NonPlayer
{
    
    //private float hp = 
    [SerializeField] private UnityEngine.AI.NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private float pathCheckFrequency = 5;
    private float pathCheckTimer = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pathCheckTimer += Time.deltaTime;
        if(pathCheckTimer >= 1/pathCheckFrequency) {
            agent.SetDestination(player.position);
            pathCheckTimer = 0;
        }
    }

    public void TakeDamage(float damage)
    {
        SetHealth(GetHealth() - damage);
        print(GetHealth());
        if (GetHealth() <= 0)
        {
            Die();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (CompareTag("Enemy") && collision.collider.GetComponentInParent<Sword>() != null)
        {
            TakeDamage(15f);
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
