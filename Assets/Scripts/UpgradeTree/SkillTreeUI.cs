using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class SkillTreeUI : MonoBehaviour
{
    [SerializeField] HeroUpgradeTree upgradeTree;

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
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePanel();
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
