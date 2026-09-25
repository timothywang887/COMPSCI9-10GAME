using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject prefabToSpawn; 
    public float spawnRate = 2.0f; 
    
    [Tooltip("The maximum distance from the spawner the object can appear.")]
    public float spawnRadius = 5.0f; 

    private float timer = 0.0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            SpawnObjectRandomly();
            timer = 0.0f; 
        }
    }

    void SpawnObjectRandomly()
    {
        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
        Vector3 finalSpawnPosition = transform.position + randomOffset;
        Instantiate(prefabToSpawn, finalSpawnPosition, transform.rotation);
    }

    // This lets you see the spawn area circle in your Scene View window while editing!
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}