using UnityEngine;
using System.Collections;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 4f;

    [Header("Survival Timer")]
    public float spawnDuration = 30f;

    [Header("UI")]
    public TextMeshProUGUI timerText;

    private bool isSpawning = true;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
        StartCoroutine(StopSpawnerAfterTime(spawnDuration));
    }

    IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (enemyPrefab != null &&
                spawnPoints != null &&
                spawnPoints.Length > 0 &&
                isSpawning)
            {
                SpawnAtRandomPoint();
            }
        }
    }

    IEnumerator StopSpawnerAfterTime(float time)
    {
        float timeRemaining = time;

        while (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;

            if (timerText != null)
            {
                timerText.text = "Enemies spawning: "
                    + Mathf.CeilToInt(timeRemaining);
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

    void SpawnAtRandomPoint()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform targetPoint = spawnPoints[randomIndex];

        Instantiate(
            enemyPrefab,
            targetPoint.position,
            Quaternion.identity
        );
    }

    public void StopSpawner()
    {
        isSpawning = false;
    }
}