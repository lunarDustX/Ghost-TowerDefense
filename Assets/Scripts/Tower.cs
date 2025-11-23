using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("基础攻击参数")]
    public float baseDamage = 3f;
    public float baseAttackCooldown = 0.8f;
    public float baseRange = 3f;

    [Header("子弹预制体")]
    public Projectile projectilePrefab;
    public Transform firePoint;

    [Header("敌人层")]
    public LayerMask enemyLayer;

    // 当前实际生效的参数（包含幽灵 buff 后）
    private float currentDamage;
    private float currentAttackCooldown;
    private float currentRange;

    private float cooldownTimer;
    private bool isBuffedByGhost;

    void Start()
    {
        RecalculateStats();
    }

    public void SetGhostBuffed(bool value)
    {
        if (isBuffedByGhost == value)
            return;

        isBuffedByGhost = value;
        RecalculateStats();
    }

    private void RecalculateStats()
    {
        currentDamage = baseDamage;
        currentAttackCooldown = baseAttackCooldown;
        currentRange = baseRange;

        if (isBuffedByGhost && AuraBuffProvider.Instance != null)
        {
            var buff = AuraBuffProvider.Instance;
            currentDamage *= buff.damageMultiplier;
            currentAttackCooldown *= buff.attackCooldownMultiplier;
            currentRange *= buff.rangeMultiplier;
        }
    }

    public void ForceRecalculateStats()
    {
        RecalculateStats();
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            EnemyHealth target = FindTarget();
            if (target != null)
            {
                Shoot(target);
                cooldownTimer = currentAttackCooldown;
            }
        }
    }

    // TODO: 之后改为离终点最近的敌人
    private EnemyHealth FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, currentRange, enemyLayer);

        if (hits.Length == 0)
            return null;

        Collider2D best = null;
        float bestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            float d = Vector2.Distance(transform.position, hit.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = hit;
            }
        }

        if (best != null)
        {
            return best.GetComponent<EnemyHealth>();
        }

        return null;
    }

    private void Shoot(EnemyHealth target)
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("[Tower] 缺少 projectilePrefab 或 firePoint", this);
            return;
        }

        Projectile proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.Init(target.transform, currentDamage);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // 用当前范围画圈（播放时是 buff 后的；编辑器用 baseRange）
        Gizmos.color = Color.cyan;
        float r = Application.isPlaying ? currentRange : baseRange;
        Gizmos.DrawWireSphere(transform.position, r);
    }
#endif
}
