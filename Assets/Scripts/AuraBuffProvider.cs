using UnityEngine;

public class AuraBuffProvider : MonoBehaviour
{
    public static AuraBuffProvider Instance { get; private set; }

    [Header("---------- 塔伤害加成 ----------")]
    [Tooltip("线性叠加：+0.1 = +10% 伤害")]
    public float towerDamageAdd = 0f;         // 多次相同升级：0.1, 0.2, 0.3...
    [Tooltip("乘法叠加：×1.2 = 伤害整体×1.2")]
    public float towerDamageMul = 1f;         // 稀有/技能类：1.2, 1.5...

    [Header("---------- 塔攻速加成 ----------")]
    [Tooltip("线性叠加：+0.1 = +10% 攻速")]
    public float towerAttackSpeedAdd = 0f;    // 多次相同升级：0.1, 0.2, ...
    [Tooltip("乘法叠加：×1.2 = 攻速整体×1.2")]
    public float towerAttackSpeedMul = 1f;

    [Header("---------- 塔射程加成 ----------")]
    [Tooltip("线性叠加：+0.1 = +10% 射程")]
    public float towerRangeAdd = 0f;
    [Tooltip("乘法叠加：×1.2 = 射程整体×1.2")]
    public float towerRangeMul = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// 最终伤害倍率： (1 + 线性加成) * 乘法加成
    /// </summary>
    public float GetDamageMultiplier()
    {
        return (1f + towerDamageAdd) * towerDamageMul;
    }

    /// <summary>
    /// 最终攻速倍率：（1 + 线性加成）* 乘法加成
    /// 注意：塔的冷却时间 = baseCooldown / 攻速倍率
    /// </summary>
    public float GetAttackSpeedMultiplier()
    {
        return (1f + towerAttackSpeedAdd) * towerAttackSpeedMul;
    }

    /// <summary>
    /// 最终射程倍率
    /// </summary>
    public float GetRangeMultiplier()
    {
        return (1f + towerRangeAdd) * towerRangeMul;
    }
}