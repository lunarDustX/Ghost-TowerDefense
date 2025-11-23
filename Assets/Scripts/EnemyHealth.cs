using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 10f;

    [Header("经验相关")]
    public int expOnDeath = 5;

    private float currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        // TODO: 掉落、特效等
        // 通知经验系统
        if (GhostExperience.Instance != null)
        {
            GhostExperience.Instance.AddExp(expOnDeath);
        }

        Destroy(gameObject);
    }
}
