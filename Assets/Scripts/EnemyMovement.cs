using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
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
}
