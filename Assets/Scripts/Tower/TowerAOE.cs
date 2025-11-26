using UnityEngine;

public class TowerAoe : TowerBase
{
    [Header("AOE 爆炸参数")]
    public float baseExplosionRadius = 1.5f;

    // 当前 AOE 半径
    private float currentExplosionRadius;
    public float BaseExplosionRadius => baseExplosionRadius;
    public float CurrentExplosionRadius => currentExplosionRadius;

    [Header("子弹参数")]
    public AoeProjectile projectilePrefab;
    public Transform firePoint;

    /// <summary>
    /// 扩展基类的属性计算：在基类计算完 damage/range/cooldown 后，再处理爆炸半径
    /// </summary>
    protected override void RecalculateStatsInternal()
    {
        base.RecalculateStatsInternal();

        // 默认爆炸半径 = baseExplosionRadius
        currentExplosionRadius = baseExplosionRadius;

        // 如果你希望 AOE 半径也吃“范围 buff”，可以用 currentRange/baseRange 的比例来放大
        if (baseRange > 0f)
        {
            float rangeScale = currentRange / baseRange;
            currentExplosionRadius = baseExplosionRadius * rangeScale;
        }
    }

    protected override void Shoot(EnemyHealth target)
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("[TowerAoe] 缺少 projectilePrefab 或 firePoint", this);
            return;
        }

        // AOE 子弹不跟踪，取当下目标的世界坐标
        Vector3 targetPos = target.transform.position;

        AoeProjectile proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.Init(targetPos, currentDamage, currentExplosionRadius, enemyLayer);
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // 爆炸范围（以塔中心预览，实际在子弹命中点）
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
        float er = Application.isPlaying
            ? (currentExplosionRadius > 0 ? currentExplosionRadius : baseExplosionRadius)
            : baseExplosionRadius;
        Gizmos.DrawWireSphere(transform.position, er);
    }
#endif
}
