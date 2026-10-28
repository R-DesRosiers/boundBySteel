using UnityEngine;
using System.Collections;

public class CinemachineIntro : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    public GameObject overviewCamera;
    public GameObject playerCamera;

    [Header("Round References")]
    public PlayerController playerController;
    public EnemySpawner enemySpawner;

    [Header("Timing")]
    public float overviewDuration = 1.5f;

    private void Start()
    {
        if (playerController != null)
        {
            playerController.SetControlsLocked(true);
        }

        if (overviewCamera != null)
        {
            overviewCamera.SetActive(true);
        }

        if (playerCamera != null)
        {
            playerCamera.SetActive(false);
        }

        StartCoroutine(
            SwitchToPlayerCamera()
        );
    }

    private IEnumerator SwitchToPlayerCamera()
    { // will show the whole level then zoom in on the player (like most level games) that way 
    // players can see the whole level before playing 
        yield return new WaitForSecondsRealtime(
            overviewDuration
        );

        if (overviewCamera != null)
        {
            overviewCamera.SetActive(false);
        }

        if (playerCamera != null)
        {
            playerCamera.SetActive(true);
        }

        if (enemySpawner != null)
        {
            enemySpawner.StartRound();
        }
        else if (playerController != null)
        {
            playerController.SetControlsLocked(false);
        }
    }
}