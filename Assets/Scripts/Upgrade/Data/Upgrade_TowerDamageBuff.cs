using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade/Tower Damage Buff")]
public class Upgrade_TowerDamageBuff : UpgradeData
{
    public float extraMultiplier = 0.3f; // 在原 multiplier 上 +0.3

    public override void Apply()
    {
        if (AuraBuffProvider.Instance != null)
        {
            AuraBuffProvider.Instance.damageMultiplier += extraMultiplier;

            // 让所有塔重新计算面板（可选增强）
            var towers = GameObject.FindObjectsOfType<Tower>();
            foreach (var t in towers)
            {
                t.ForceRecalculateStats();
            }
        }
    }
}
