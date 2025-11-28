using UnityEngine;
using System;

public enum HeroUpgradeBranch
{
    MoveSpeed,         // 玩家移速
    AuraRadius,        // Aura 范围
    SkillCooldown,     // 主动技能冷却
    SkillSlowEffect,   // 主动技能减速效果
    TowerAttackSpeed,  // Aura 内塔攻速
    TowerDamage        // Aura 内塔伤害
}

[System.Serializable]
public class HeroUpgradeBranchState
{
    public HeroUpgradeBranch branch;
    [Range(0, 3)]
    public int currentLevel = 0;
    public int maxLevel = 3;
}

/// <summary>
/// 负责：
/// - 记录 6 条分支当前等级
/// - 管理技能点
/// - 对外提供各种“最终倍率/加成”查询
/// - 提供 Upgrade 方法给 UI 调用
/// </summary>
public class HeroUpgradeTree : MonoBehaviour
{
    public static HeroUpgradeTree Instance { get; private set; }

    [Header("分支状态（在 Inspector 里直接配置 6 条）")]
    public HeroUpgradeBranchState[] branches;

    [Header("当前可用技能点")]
    public int availablePoints = 0;

    /// <summary>当分支等级变化时（用于刷新 UI）</summary>
    public event Action<HeroUpgradeBranch> OnBranchLevelChanged;
    /// <summary>当技能点数量变化时</summary>
    public event Action<int> OnPointsChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #region 技能点管理
    public void AddPoints(int amount)
    {
        if (amount <= 0) return;
        availablePoints += amount;
        OnPointsChanged?.Invoke(availablePoints);
    }

    public bool HasPoints => availablePoints > 0;
    #endregion

    #region 分支等级读写
    public int GetLevel(HeroUpgradeBranch branch)
    {
        var s = FindBranch(branch);
        return s != null ? s.currentLevel : 0;
    }

    public bool IsMaxed(HeroUpgradeBranch branch)
    {
        var s = FindBranch(branch);
        return s != null && s.currentLevel >= s.maxLevel;
    }

    public bool CanUpgrade(HeroUpgradeBranch branch)
    {
        var s = FindBranch(branch);
        if (s == null) return false;
        if (availablePoints <= 0) return false;
        if (s.currentLevel >= s.maxLevel) return false;
        // 这里以后可以加“前置条件”等逻辑
        return true;
    }

    public bool TryUpgrade(HeroUpgradeBranch branch)
    {
        if (!CanUpgrade(branch))
            return false;

        var s = FindBranch(branch);
        s.currentLevel++;
        availablePoints--;

        OnBranchLevelChanged?.Invoke(branch);
        OnPointsChanged?.Invoke(availablePoints);

        // 升级后，把最新加成应用到各系统
        ApplyAllUpgrades();

        return true;
    }

    private HeroUpgradeBranchState FindBranch(HeroUpgradeBranch branch)
    {
        if (branches == null) return null;
        foreach (var b in branches)
        {
            if (b.branch == branch)
                return b;
        }
        return null;
    }
    #endregion

    #region 各分支对应的“数值公式”

    /// <summary>玩家移速倍率</summary>
    public float GetMoveSpeedMultiplier()
    {
        int lv = GetLevel(HeroUpgradeBranch.MoveSpeed);
        return 1f + 0.2f * lv;
    }

    /// <summary>Aura 范围倍率</summary>
    public float GetAuraRadiusMultiplier()
    {
        int lv = GetLevel(HeroUpgradeBranch.AuraRadius);
        return 1f + 0.3f * lv;
    }

    /// <summary>技能冷却倍率</summary>
    public float GetSkillCooldownMultiplier()
    {
        int lv = GetLevel(HeroUpgradeBranch.SkillCooldown);
        return 1f - 0.2f * lv;
    }

    /// <summary>技能减速效果倍率</summary>
    public float GetSkillSlowEffectMultiplier()
    {
        int lv = GetLevel(HeroUpgradeBranch.SkillSlowEffect);
        return 1f + 0.5f * lv;
    }

    /// <summary>攻速倍率</summary>
    public float GetTowerAttackSpeedMultiplier()
    {
        int lv = GetLevel(HeroUpgradeBranch.TowerAttackSpeed);
        return 1f + 0.5f * lv;
    }

    /// <summary>伤害倍率</summary>
    public float GetTowerDamageMultiplier()
    {
        int lv = GetLevel(HeroUpgradeBranch.TowerDamage);
        return 1f + 0.5f * lv;
    }

    #endregion

    #region 把加成应用到各系统（关键：在这里集中调用）

    [Header("应用目标 Refs")]
    public GhostController ghostController;          // 玩家移动
    public GhostAura ghostAura;                 // Aura 半径
    public GhostActiveSkill ghostSkill;         // 主动技能（冷却 & 减速）
    public AuraBuffProvider auraBuffProvider;   // Aura 内塔 buff

    /// <summary>
    /// 每次升级后调用：根据当前分支等级刷新所有相关数值
    /// </summary>
    public void ApplyAllUpgrades()
    {
        // 1. 玩家移速
        if (ghostController != null)
        {
            ghostController.ApplyMoveSpeedFromUpgrade(GetMoveSpeedMultiplier());
        }

        // 2. Aura 半径
        if (ghostAura != null)
        {
            ghostAura.ApplyRadiusFromUpgrade(GetAuraRadiusMultiplier());
        }

        // 3. 主动技能（冷却 & 减速）
        if (ghostSkill != null)
        {
            ghostSkill.ApplyUpgradeMultipliers(
                GetSkillCooldownMultiplier(),
                GetSkillSlowEffectMultiplier()
            );
        }

        // 4. Aura 内塔攻速 & 伤害
        if (auraBuffProvider != null)
        {
            auraBuffProvider.ApplyTowerBuffFromUpgrade(
                GetTowerAttackSpeedMultiplier(),
                GetTowerDamageMultiplier()
            );
        }
    }

    #endregion
}
