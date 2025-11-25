using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 10f;

    [Header("经验相关")]
    public int expOnDeath = 5;

    private float currentHealth;
    private EnemyHitFlash hitFlash;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    // 敌人死亡/被销毁时全局广播
    public static event Action<EnemyHealth> OnAnyEnemyDestroyed;

    void Awake()
    {
        currentHealth = maxHealth;
        hitFlash = GetComponent<EnemyHitFlash>();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        // 受击反馈
        hitFlash?.PlayFlash();
        AudioMgr.I.PlaySFX(SFXType.ProjectileHit);

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

    private void OnDestroy()
    {
        // 任何原因（被打死 / 到终点自己 Destroy / 被别的系统清理）都会触发
        OnAnyEnemyDestroyed?.Invoke(this);
    }
}
