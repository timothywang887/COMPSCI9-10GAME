using UnityEngine;

public class TargetedSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public float enemySpawnRate = 2.0f;
    public float enemySpawnRadius = 3.0f;

    [Tooltip("Drag the 'original' reference object here (e.g., Player or Base). If left empty, it defaults to this spawner.")]
    public Transform originalObjectCenter; 

    private float timer = 0.0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= enemySpawnRate)
        {
            SpawnNearOriginal();
            timer = 0.0f;
        }
    }

    void SpawnNearOriginal()
    {
        Vector3 centerPoint = originalObjectCenter != null ? originalObjectCenter.position : transform.position;
        Vector3 randomOffset = Random.insideUnitSphere * enemySpawnRadius;
        randomOffset.y = 0; 

        Vector3 finalSpawnPosition = centerPoint + randomOffset;

        GameObject spawnedClone = Instantiate(prefabToSpawn, finalSpawnPosition, transform.rotation);

        if (spawnedClone.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.useGravity = true;
        }
        else
        {
            Rigidbody newRb = spawnedClone.AddComponent<Rigidbody>();
            newRb.useGravity = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 centerPoint = originalObjectCenter != null ? originalObjectCenter.position : transform.position;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(centerPoint, enemySpawnRadius);
    }
}