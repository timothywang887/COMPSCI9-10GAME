using UnityEngine;

public class TargetedSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public float spawnRate = 2.0f;
    public float spawnRadius = 5.0f;

    [Tooltip("Drag the 'original' reference object here (e.g., Player or Base). If left empty, it defaults to this spawner.")]
    public Transform originalObjectCenter; 

    private float timer = 0.0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            SpawnNearOriginal();
            timer = 0.0f;
        }
    }

    void SpawnNearOriginal()
    {
        // Fallback: If no original object is assigned, use this spawner's position
        Vector3 centerPoint = originalObjectCenter != null ? originalObjectCenter.position : transform.position;

        // Calculate offset bounded by your maximum radius
        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
        
        // Keep it flat on the horizontal plane if it's a 3D ground game
        randomOffset.y = 0; 

        Vector3 finalSpawnPosition = centerPoint + randomOffset;

        Instantiate(prefabToSpawn, finalSpawnPosition, transform.rotation);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 centerPoint = originalObjectCenter != null ? originalObjectCenter.position : transform.position;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(centerPoint, spawnRadius);
    }
}
