using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    private Enemy enemy; 
    
    [Header("UI Reference")]
    public Image healthBarFill;

    void Start()
    {
        // Automatically finds the Enemy component on the parent
        enemy = GetComponentInParent<Enemy>();
    }

    void Update()
    {
        // Dynamically update the health bar fill amount
        if (enemy != null && healthBarFill != null)
        {
            float healthPercent = (float)enemy.health / enemy.maxHealth;
            healthBarFill.fillAmount = healthPercent;
        }
    }
}