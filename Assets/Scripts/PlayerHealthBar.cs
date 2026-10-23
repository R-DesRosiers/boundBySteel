using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public PlayerController player;
    public Image healthImage;

    private void Start()
    {
        if (healthImage == null)
        {
            healthImage = GetComponent<Image>();
        }
    }

    private void Update()
    {
        if (player != null && healthImage != null)
        {
            healthImage.fillAmount =
                (float)player.health / player.maxHealth;
        }
    }
}