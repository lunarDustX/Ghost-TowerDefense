using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade/Move Speed")]
public class Upgrade_MoveSpeed : UpgradeData
{
    public float moveSpeedIncrease = 1.0f; // 直接+，也可以用百分比

    public override void Apply()
    {
        var ghost = FindObjectOfType<GhostController>();
        if (ghost != null)
        {
            ghost.moveSpeed += moveSpeedIncrease;
        }
    }
}
