using UnityEngine;

public abstract class TowerBase : MonoBehaviour, IGhostBuffable
{
    [Header("基础攻击参数")]
    public float baseDamage = 3f;
    public float baseAttackCooldown = 0.8f;
    public float baseRange = 3f;

    [Header("敌人层")]
    public LayerMask enemyLayer;

    [Header("视觉")]
    [SerializeField] protected SpriteRenderer sr;
    [SerializeField] protected Sprite spr_normal;
    [SerializeField] protected Sprite spr_buff;

    [Header("音效")]
    public SFXType buffingSFX;

    [Header("选中可视化")]
    [SerializeField] RangeIndicator rangeIndicator;

    // 当前实际生效的参数
    protected float currentDamage;
    protected float currentAttackCooldown;
    protected float currentRange;

    protected float cooldownTimer;
    protected bool isBuffedByGhost;

    // --- 暴露信息参数 ---
    public float BaseDamage => baseDamage;
    public float BaseAttackCooldown => baseAttackCooldown;
    public float BaseRange => baseRange;

    public float CurrentDamage => currentDamage;
    public float CurrentAttackCooldown => currentAttackCooldown;
    public float CurrentRange => currentRange;

    public bool IsBuffedByGhost => isBuffedByGhost;

    protected virtual void Start()
    {
        RecalculateStats();

        // 确保初始时范围圈是隐藏的
        if (rangeIndicator != null)
            rangeIndicator.SetVisible(false);
    }


    // SelectionManager 调用
    public virtual void SetSelected(bool selected)
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.SetVisible(selected);
        }

        // 你也可以在这里顺手改一下塔 sprite 的明暗 / 外轮廓高亮
        // 比如：
        // if (sr != null) sr.color = selected ? selectedColor : normalColor;
    }

    public virtual void SetGhostBuffed(bool value)
    {
        if (isBuffedByGhost == value)
            return;

        isBuffedByGhost = value;

        // 反馈
        if (sr != null)
            sr.sprite = isBuffedByGhost ? spr_buff : spr_normal;

        if (isBuffedByGhost)
            AudioMgr.I.PlaySFX(buffingSFX);

        RecalculateStats();
    }

    /// <summary>
    /// 外部升级后可强制刷新一次属性
    /// </summary>
    public void ForceRecalculateStats()
    {
        RecalculateStats();
    }

    /// <summary>
    /// 对外统一调用的重算函数，内部会处理冷却平滑。
    /// 子类只需要重写 RecalculateStatsInternal 来扩展自己的额外属性逻辑。
    /// </summary>
    protected void RecalculateStats()
    {
        float oldCooldown = currentAttackCooldown > 0f ? currentAttackCooldown : baseAttackCooldown;
        float oldTimer = cooldownTimer;

        RecalculateStatsInternal();

        // 冷却进度平滑：
        if (oldCooldown > 0f)
        {
            float remainingRatio = Mathf.Clamp01(oldTimer / oldCooldown);
            cooldownTimer = remainingRatio * currentAttackCooldown;

            if (oldTimer <= 0f)
            {
                cooldownTimer = 0f;
            }
        }
    }

    /// <summary>
    /// 子类可以在这里扩展自己的属性计算（例如 AOE 半径等）。
    /// </summary>
    protected virtual void RecalculateStatsInternal()
    {
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
    }

    protected virtual void Update()
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

    protected virtual EnemyHealth FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, currentRange, enemyLayer);
        if (hits.Length == 0)
            return null;

        EnemyHealth bestEnemy = null;
        float bestProgress = float.MinValue;

        foreach (var hit in hits)
        {
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

        return bestEnemy;
    }

    /// <summary>
    /// 子类实现具体的攻击方式（单体/aoe等）
    /// </summary>
    protected abstract void Shoot(EnemyHealth target);

#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        float r = Application.isPlaying ? (currentRange > 0 ? currentRange : baseRange) : baseRange;
        Gizmos.DrawWireSphere(transform.position, r);
    }
#endif
}
