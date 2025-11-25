using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("基础攻击参数")]
    public float baseDamage = 3f;
    public float baseAttackCooldown = 0.8f;
    public float baseRange = 3f;

    [Header("子弹参数")]
    public Projectile projectilePrefab;
    public Transform firePoint;

    [Header("敌人层")]
    public LayerMask enemyLayer;

    [Header("Refs")]
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Sprite spr_normal;
    [SerializeField] Sprite spr_buff;

    // 当前实际生效的参数
    private float currentDamage;
    private float currentAttackCooldown;
    private float currentRange;

    private float cooldownTimer;
    private bool isBuffedByGhost;

    // --- 暴露信息参数 ---
    public float BaseDamage => baseDamage;
    public float BaseAttackCooldown => baseAttackCooldown;
    public float BaseRange => baseRange;

    public float CurrentDamage => currentDamage;
    public float CurrentAttackCooldown => currentAttackCooldown;
    public float CurrentRange => currentRange;

    void Start()
    {
        RecalculateStats();
    }

    public void SetGhostBuffed(bool value)
    {
        if (isBuffedByGhost == value)
            return;

        isBuffedByGhost = value;

        // 反馈
        sr.sprite = isBuffedByGhost ? spr_buff : spr_normal;
        if (isBuffedByGhost)
            AudioMgr.I.PlaySFX(SFXType.TowerBuffing);

        RecalculateStats();
    }

    /// <summary>
    /// 外部升级后可强制刷新一次属性
    /// </summary>
    public void ForceRecalculateStats()
    {
        RecalculateStats();
    }

    private void RecalculateStats()
    {
        float oldCooldown = currentAttackCooldown > 0f ? currentAttackCooldown : baseAttackCooldown;
        float oldTimer = cooldownTimer;

        // 1) 先用基础面板
        currentDamage = baseDamage;
        currentAttackCooldown = baseAttackCooldown;
        currentRange = baseRange;

        // 2) 再叠加幽灵 Aura 的 buff（如果正在 Aura 范围内）
        if (isBuffedByGhost && AuraBuffProvider.Instance != null)
        {
            var buff = AuraBuffProvider.Instance;

            float dmgMul = buff.GetDamageMultiplier();
            float atkSpdMul = buff.GetAttackSpeedMultiplier();
            float rangeMul = buff.GetRangeMultiplier();

            currentDamage *= dmgMul;
            currentRange *= rangeMul;

            // 攻速使用“倍率”来缩短冷却时间
            currentAttackCooldown = baseAttackCooldown / Mathf.Max(atkSpdMul, 0.01f);
        }

        // 3) 冷却进度平滑：
        //    保持“已经走掉的冷却比例”不变，避免 buff/失去 buff 时手感怪异
        if (oldCooldown > 0f)
        {
            // oldTimer 越小，说明越接近下一发
            float remainingRatio = Mathf.Clamp01(oldTimer / oldCooldown);  // 0~1：1 表示刚开始冷却
            cooldownTimer = remainingRatio * currentAttackCooldown;

            // 如果冷却本来已经结束，不要硬拉回去
            if (oldTimer <= 0f)
            {
                cooldownTimer = 0f;
            }
        }
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

    private EnemyHealth FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, currentRange, enemyLayer);
        if (hits.Length == 0)
            return null;

        EnemyHealth bestEnemy = null;
        float bestProgress = float.MinValue;

        foreach (var hit in hits)
        {
            // 必须是有 EnemyMover 的对象，才认为是“在路上的怪”
            var mover = hit.GetComponent<EnemyMover>();
            var health = hit.GetComponent<EnemyHealth>();

            if (mover == null || health == null)
                continue;

            float progress = mover.PathProgress01;

            // 选“离终点最近”的：进度最大
            if (progress > bestProgress)
            {
                bestProgress = progress;
                bestEnemy = health;
            }
        }

        // 如果周围怪都没有 EnemyMover，就返回 null
        return bestEnemy;
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
        Gizmos.color = Color.cyan;
        float r = Application.isPlaying ? (currentRange > 0 ? currentRange : baseRange) : baseRange;
        Gizmos.DrawWireSphere(transform.position, r);
    }
#endif
}
