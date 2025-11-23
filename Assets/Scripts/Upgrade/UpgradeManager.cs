using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("全部可用升级池")]
    [SerializeField] List<UpgradeData> allUpgrades = new List<UpgradeData>();

    [Header("每次给玩家的选项数")]
    [SerializeField] int optionsPerLevel = 3;

    [Header("升级 UI")]
    [SerializeField] UpgradeUI upgradeUI; // 我们下面写一个简单版

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void OnGhostLevelUp(int newLevel)
    {
        // 抽几张牌
        var options = GetRandomUpgrades(optionsPerLevel);
        if (options.Count == 0)
        {
            Debug.LogWarning("No upgrades available!");
            return;
        }

        // 暂停游戏（可选）
        Time.timeScale = 0f;

        // 打开 UI
        if (upgradeUI != null)
        {
            upgradeUI.Show(options, OnUpgradeSelected);
        }
    }

    private List<UpgradeData> GetRandomUpgrades(int count)
    {
        List<UpgradeData> pool = new List<UpgradeData>(allUpgrades);
        List<UpgradeData> result = new List<UpgradeData>();

        // 简单版：无权重，随机不重复
        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int index = Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return result;
    }

    private void OnUpgradeSelected(UpgradeData chosen)
    {
        // 生效
        chosen.Apply();

        // 恢复时间
        Time.timeScale = 1f;
    }
}
