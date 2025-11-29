using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class SkillTreeUI : MonoBehaviour
{
    [SerializeField] HeroUpgradeTree upgradeTree;

    [Header("Tooltip")]
    [SerializeField] SkillTreeTooltip tooltip;

    [Header("隐藏 / 显示用根节点（整个面板）")]
    public GameObject rootPanel;

    [Header("6 个分支 UI 配置")]
    public SkillBranchUI[] branchUIs;

    private void OnEnable()
    {
        if (upgradeTree != null)
        {
            upgradeTree.OnBranchLevelChanged += OnBranchLevelChanged;
            upgradeTree.OnPointsChanged += OnPointsChanged;
        }

        RefreshAll();
    }

    private void OnDisable()
    {
        if (upgradeTree != null)
        {
            upgradeTree.OnBranchLevelChanged -= OnBranchLevelChanged;
            upgradeTree.OnPointsChanged -= OnPointsChanged;
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePanel();
        }
#endif
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (rootPanel.activeSelf)
                Close();
        }
    }

    public void Open()
    {
        rootPanel.SetActive(true);
        RefreshAll();
        Time.timeScale = 0;
    }

    public void Close()
    {
        rootPanel.SetActive(false);
        Time.timeScale = 1;
        HideTooltip();
    }

    public void TogglePanel()
    {
        if (rootPanel == null) return;
        rootPanel.SetActive(!rootPanel.activeSelf);

        if (rootPanel.activeSelf)
        {
            Time.timeScale = 0;
            RefreshAll();
        }
        else
        {
            Time.timeScale = 1;
            HideTooltip();
        }
    }

    private void OnBranchLevelChanged(HeroUpgradeBranch branch)
    {
        RefreshBranch(branch);
    }

    private void OnPointsChanged(int points)
    {
        RefreshAllInteractable();
    }

    public void RefreshAll()
    {
        if (upgradeTree == null) return;

        if (branchUIs != null)
        {
            foreach (var ui in branchUIs)
            {
                if (ui != null)
                    RefreshBranch(ui.branch);
            }
        }

        RefreshAllInteractable();
    }


    void RefreshBranch(HeroUpgradeBranch branch)
    {
        if (branchUIs == null || upgradeTree == null) return;

        foreach (var ui in branchUIs)
        {
            if (ui != null && ui.branch == branch)
            {
                int lv = upgradeTree.GetLevel(branch);
                ui.SetLevel(lv);
                ui.SetInteractable(upgradeTree.CanUpgrade(branch));
                break;
            }
        }
    }

    void RefreshAllInteractable()
    {
        if (branchUIs == null || upgradeTree == null) return;

        foreach (var ui in branchUIs)
        {
            if (ui != null)
                ui.SetInteractable(upgradeTree.CanUpgrade(ui.branch));
        }
    }

    // 给 Button 用的回调 ↓

    public void OnClickBranch_MoveSpeed() => OnClickBranch(HeroUpgradeBranch.MoveSpeed);
    public void OnClickBranch_AuraRadius() => OnClickBranch(HeroUpgradeBranch.AuraRadius);
    public void OnClickBranch_SkillCooldown() => OnClickBranch(HeroUpgradeBranch.SkillCooldown);
    public void OnClickBranch_SkillSlowEffect() => OnClickBranch(HeroUpgradeBranch.SkillSlowEffect);
    public void OnClickBranch_TowerAttackSpeed() => OnClickBranch(HeroUpgradeBranch.TowerAttackSpeed);
    public void OnClickBranch_TowerDamage() => OnClickBranch(HeroUpgradeBranch.TowerDamage);

    void OnClickBranch(HeroUpgradeBranch branch)
    {
        if (upgradeTree == null) return;

        if (upgradeTree.TryUpgrade(branch))
        {
            // TryUpgrade 里已经会触发事件 -> UI 自动刷新
        }
        else
        {
            // 不满足条件：技能点不足 / 已满级，想的话可以做个提示
            Debug.Log($"[SkillTreeUI] 无法升级：{branch}");
        }
    }

    #region Hover
    public void ShowTooltip(HeroUpgradeBranch branch, Vector2 screenPos)
    {
        if (tooltip == null || upgradeTree == null) return;

        string title = GetBranchTitle(branch);
        string body = BuildNextLevelDescription(branch);

        tooltip.Show(title, body, screenPos);
    }

    public void MoveTooltip(Vector2 screenPos)
    {
        if (tooltip == null) return;
        tooltip.Move(screenPos);
    }

    public void HideTooltip()
    {
        if (tooltip == null) return;
        tooltip.Hide();
    }
    #endregion

    string GetBranchTitle(HeroUpgradeBranch branch)
    {
        switch (branch)
        {
            case HeroUpgradeBranch.MoveSpeed: return "移动速度";
            case HeroUpgradeBranch.AuraRadius: return "Aura Radius";
            case HeroUpgradeBranch.SkillCooldown: return "主动技能冷却";
            case HeroUpgradeBranch.SkillSlowEffect: return "Skill";
            case HeroUpgradeBranch.TowerAttackSpeed: return "Tower Atk Spd";
            case HeroUpgradeBranch.TowerDamage: return "塔伤害（Aura 内）";
            default: return branch.ToString();
        }
    }

    string BuildNextLevelDescription(HeroUpgradeBranch branch)
    {
        if (upgradeTree == null) return "";

        int curLv = upgradeTree.GetLevel(branch);
        int maxLv = upgradeTree.GetMaxLevel(branch);

        //if (curLv >= maxLv)
        //{
        //    return $"当前等级：Lv {curLv}\n已达到最高等级。";
        //}

        int nextLv = curLv + 1;
        nextLv = Mathf.Min(nextLv, maxLv);

        switch (branch)
        {
            case HeroUpgradeBranch.MoveSpeed:
                {
                    float curMul = upgradeTree.GetMoveSpeedMultiplierByLevel(curLv);
                    float nextMul = upgradeTree.GetMoveSpeedMultiplierByLevel(nextLv);

                    float curPct = (curMul - 1f) * 100f;
                    float nextPct = (nextMul - 1f) * 100f;

                    return
                        $"move speed +{nextPct:0}%";
                }

            case HeroUpgradeBranch.AuraRadius:
                {
                    float curMul = upgradeTree.GetAuraRadiusMultiplierByLevel(curLv);
                    float nextMul = upgradeTree.GetAuraRadiusMultiplierByLevel(nextLv);

                    float curPct = (curMul - 1f) * 100f;
                    float nextPct = (nextMul - 1f) * 100f;

                    return
                        $"aura radius +{nextPct:0}%";
                }

            case HeroUpgradeBranch.SkillCooldown:
                {
                    float curMul = upgradeTree.GetSkillCooldownMultiplierByLevel(curLv);
                    float nextMul = upgradeTree.GetSkillCooldownMultiplierByLevel(nextLv);

                    float curReduce = (1f - curMul) * 100f;
                    float nextReduce = (1f - nextMul) * 100f;

                    return
                        $"当前等级：Lv {curLv}（冷却时间 -{curReduce:0}%";
                }

            case HeroUpgradeBranch.SkillSlowEffect:
                {
                    float curMul = upgradeTree.GetSkillSlowEffectMultiplierByLevel(curLv);
                    float nextMul = upgradeTree.GetSkillSlowEffectMultiplierByLevel(nextLv);

                    //float curPct = (curMul - 1f) * 100f;
                    //float nextPct = (nextMul - 1f) * 100f;

                    return
                        $"skill slow enemy speed {(nextMul + 0.4f)*100f :0}%";
                }

            case HeroUpgradeBranch.TowerAttackSpeed:
                {
                    float curMul = upgradeTree.GetTowerAttackSpeedMultiplierByLevel(curLv);
                    float nextMul = upgradeTree.GetTowerAttackSpeedMultiplierByLevel(nextLv);

                    float curPct = (curMul - 1f) * 100f;
                    float nextPct = (nextMul - 1f) * 100f;

                    return
                        $"tower attack speed +{nextPct + 50 :0}%";
                }

            case HeroUpgradeBranch.TowerDamage:
                {
                    float curMul = upgradeTree.GetTowerDamageMultiplierByLevel(curLv);
                    float nextMul = upgradeTree.GetTowerDamageMultiplierByLevel(nextLv);

                    float curPct = (curMul - 1f) * 100f;
                    float nextPct = (nextMul - 1f) * 100f;

                    return
                        $"当前等级：Lv {curLv}（Aura 内塔伤害 +{curPct:0}%）\n" +
                        $"下一级：Lv {nextLv}（Aura 内塔伤害 +{nextPct:0}%）";
                }
        }

        return "";
    }
}

[Serializable]
public class SkillBranchUI
{
    [Header("对应哪一个分支")]
    public HeroUpgradeBranch branch;

    [Header("升级按钮（点击整列）")]
    [SerializeField] Button upgradeButton;

    [Header("3 个等级格子（从下到上）")]
    [SerializeField] Image[] levelSlots;

    Color activeColor = Color.yellow;
    Color inactiveColor = Color.gray;

    public void SetLevel(int level)
    {
        if (levelSlots == null) return;

        for (int i = 0; i < levelSlots.Length; i++)
        {
            if (levelSlots[i] == null) continue;

            bool on = i < level;   // 比如 lv=2 -> index 0,1 亮
            levelSlots[i].color = on ? activeColor : inactiveColor;
        }
    }

    public void SetInteractable(bool canUpgrade)
    {
        if (upgradeButton != null)
            upgradeButton.interactable = canUpgrade;
    }
}
