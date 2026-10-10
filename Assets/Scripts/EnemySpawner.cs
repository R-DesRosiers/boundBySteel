using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;          
    public Transform[] spawnPoints;         
    public float spawnInterval = 4f;        

    [Header("Survival Timer")]
    public float spawnDuration = 30f;       // How many seconds the spawner stays active

    private bool isSpawning = true;

    void Start()
    {
        // Start spawning ghosts
        StartCoroutine(SpawnRoutine());

        // Start a countdown timer to shut the spawner down automatically
        StartCoroutine(StopSpawnerAfterTime(spawnDuration));
    }

    IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (enemyPrefab != null && spawnPoints.Length > 0 && isSpawning)
            {
                SpawnAtRandomPoint();
            }
        }
    }

    IEnumerator StopSpawnerAfterTime(float time)
    {
        // Wait for the duration of the level/wave
        yield return new WaitForSeconds(time);

        // Turn off the spawner
        StopSpawner();
        Debug.Log("Spawner finished! Time's up.");
    }

    void SpawnAtRandomPoint()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform targetPoint = spawnPoints[randomIndex];
        Instantiate(enemyPrefab, targetPoint.position, Quaternion.identity);
    }

    public void StopSpawner()
    {
        isSpawning = false;
    }
}