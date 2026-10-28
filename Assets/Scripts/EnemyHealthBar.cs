using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    private Enemy enemy; 
    
    [Header("UI Reference")]
    public Image healthBarFill;

    void Start()
    {
        // Automatically finds the enemy component on the parent
        enemy = GetComponentInParent<Enemy>();
    }

    void Update()
    {
        // update health bar fill amt 
        if (enemy != null && healthBarFill != null)
        {
            float healthPercent = (float)enemy.health / enemy.maxHealth;
            healthBarFill.fillAmount = healthPercent;
        }
    }
}