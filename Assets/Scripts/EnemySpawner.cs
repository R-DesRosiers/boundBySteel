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

    [Header("Timer Warning")]
    public AudioClip timerWarningSound;
    public float warningTime = 10f;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI roundText;

    [Header("Round Introduction")]
    public AudioSource audioSource;
    public AudioClip roundAnnouncement;
    public int roundNumber = 1;
    public float extraDelayAfterAnnouncement = 0.2f;

    [Header("Round References")]
    public PlayerController playerController;

    private bool isSpawning = false;
    private bool timerWarningPlayed = false;
    private bool roundStarted = false;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        isSpawning = false;
        timerWarningPlayed = false;
        roundStarted = false;

        if (roundText != null)
        {
            roundText.gameObject.SetActive(false);
        }

        if (timerText != null)
        {
            timerText.text = "";
        }
    }

    public void StartRound()
    { // start spawning, this way it would only begin after the zoom in is done 
        if (roundStarted) 
        {
            return;
        }

        roundStarted = true;
        isSpawning = true;
        timerWarningPlayed = false;

        StartCoroutine(BeginRound());
    }

    private IEnumerator BeginRound()
    {
        if (roundText != null)
        {
            roundText.text = "ROUND " + roundNumber;
            roundText.gameObject.SetActive(true);
        }

        if (
            audioSource != null &&
            roundAnnouncement != null
        )
        {
            audioSource.PlayOneShot(roundAnnouncement);

            yield return new WaitForSecondsRealtime(
                roundAnnouncement.length
            );
        }
        else
        {
            yield return new WaitForSecondsRealtime(1f);
        }

        if (roundText != null)
        {
            roundText.gameObject.SetActive(false);
        }

        yield return new WaitForSecondsRealtime(
            extraDelayAfterAnnouncement
        );

        StartCoroutine(SpawnRoutine());

        StartCoroutine(
            StopSpawnerAfterTime(spawnDuration)
        );

        if (playerController != null)
        {
            playerController.SetControlsLocked(false);
        }
        else
        {
            Debug.LogWarning(
                "EnemySpawner does not have a PlayerController assigned."
            );
        }
    }

    private IEnumerator SpawnRoutine()
    { // respawns enemies every x seconds 
        while (isSpawning)
        {
            yield return new WaitForSeconds(
                spawnInterval
            );

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

            if (
                !timerWarningPlayed &&
                timeRemaining <= warningTime
            )
            {
                if (
                    audioSource != null &&
                    timerWarningSound != null
                )
                {
                    audioSource.PlayOneShot(
                        timerWarningSound
                    );
                }

                timerWarningPlayed = true;
            }

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

        Debug.Log(
            "Spawner finished! Time's up."
        );
    }

    private void SpawnAtRandomPoint()
    { // use enemy prefabs to spawn in one of the created spawn points 
    // pick random one out of list^ 
        GameObject prefabToSpawn =
            ChooseEnemyPrefab();

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
    { // now we can do multiple enemies 
        if (!useMultipleEnemyTypes)
        {
            return enemyPrefab;
        }

        if (
            ghostPrefab != null &&
            archerPrefab != null
        )
        {
            if (Random.value < archerSpawnChance)
            {
                return archerPrefab;
            }

            return ghostPrefab;
        }

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