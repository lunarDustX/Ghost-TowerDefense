using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade/Aura Radius")]
public class Upgrade_AuraRadius : UpgradeData
{
    public float radiusIncrease = 0.5f;

    public override void Apply()
    {
        var aura = FindObjectOfType<GhostAura>();
        if (aura == null) return;

        aura.radius += radiusIncrease;
        aura.RefreshRadius();
    }
}