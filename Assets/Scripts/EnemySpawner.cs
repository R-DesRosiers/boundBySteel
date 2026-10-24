using UnityEngine;
using System.Collections;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;

    [Header("Optional Multiple Enemy Types")]
    public bool useMultipleEnemyTypes = false;
    public GameObject ghostPrefab;
    public GameObject archerPrefab;

    [Range(0f, 1f)]
    public float archerSpawnChance = 0.5f;

    public Transform[] spawnPoints;
    public float spawnInterval = 4f;

    [Header("Survival Timer")]
    public float spawnDuration = 30f;

    [Header("UI")]
    public TextMeshProUGUI timerText;

    private bool isSpawning = true;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
        StartCoroutine(StopSpawnerAfterTime(spawnDuration));
    }

    private IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (
                spawnPoints != null &&
                spawnPoints.Length > 0 &&
                isSpawning
            )
            {
                SpawnAtRandomPoint();
            }
        }
    }

    private IEnumerator StopSpawnerAfterTime(float time)
    {
        float timeRemaining = time;

        while (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;

            if (timerText != null)
            {
                timerText.text =
                    "Enemies spawning: " +
                    Mathf.CeilToInt(timeRemaining);
            }

            yield return null;
        }

        StopSpawner();

        if (timerText != null)
        {
            timerText.text = "Spawning complete!";
        }

        Debug.Log("Spawner finished! Time's up.");
    }

    private void SpawnAtRandomPoint()
    {
        GameObject prefabToSpawn = ChooseEnemyPrefab();

        if (prefabToSpawn == null)
        {
            Debug.LogWarning(
                "No enemy prefab is assigned to the spawner."
            );

            return;
        }

        int randomIndex =
            Random.Range(0, spawnPoints.Length);

        Transform targetPoint =
            spawnPoints[randomIndex];

        Instantiate(
            prefabToSpawn,
            targetPoint.position,
            Quaternion.identity
        );
    }

    private GameObject ChooseEnemyPrefab()
    {
        // Single-enemy mode
        if (!useMultipleEnemyTypes)
        {
            return enemyPrefab;
        }

        // If both enemy types are assigned, randomly choose
        if (ghostPrefab != null && archerPrefab != null)
        {
            if (Random.value < archerSpawnChance)
            {
                return archerPrefab;
            }

            return ghostPrefab;
        }

        // Fallback if only one multiple-enemy prefab is assigned
        if (ghostPrefab != null)
        {
            return ghostPrefab;
        }

        if (archerPrefab != null)
        {
            return archerPrefab;
        }

        return enemyPrefab;
    }

    public void StopSpawner()
    {
        isSpawning = false;
    }
}