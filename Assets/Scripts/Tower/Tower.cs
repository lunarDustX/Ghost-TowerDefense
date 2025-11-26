using UnityEngine;

public class Tower : TowerBase
{
    [Header("子弹参数")]
    public Projectile projectilePrefab;
    public Transform firePoint;

    protected override void Shoot(EnemyHealth target)
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("[Tower] 缺少 projectilePrefab 或 firePoint", this);
            return;
        }

        Projectile proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.Init(target.transform, currentDamage);
    }
}
