using UnityEngine;

public class AuraBuffProvider : MonoBehaviour
{
    public static AuraBuffProvider Instance { get; private set; }

    [Header("基础 Aura buff")]
    public float baseDamageMultiplier = 1.0f;
    public float baseAttackSpeedMultiplier = 1.5f;
    public float baseRangeMultiplier = 1.0f;

    // 最终生效倍率（基础 * 升级树）
    private float finalDamageMul = 1f;
    private float finalAtkSpeedMul = 1f;

    private void Awake()
    {
        Instance = this;
        ApplyTowerBuffFromUpgrade(1f, 1f);
    }

    public float GetDamageMultiplier() => finalDamageMul;
    public float GetAttackSpeedMultiplier() => finalAtkSpeedMul;
    public float GetRangeMultiplier() => baseRangeMultiplier;

    // 给 HeroUpgradeTree 调用的接口
    public void ApplyTowerBuffFromUpgrade(float atkSpeedMulFromTree, float dmgMulFromTree)
    {
        finalDamageMul = baseDamageMultiplier * dmgMulFromTree;
        finalAtkSpeedMul = baseAttackSpeedMultiplier * atkSpeedMulFromTree;
    }
}