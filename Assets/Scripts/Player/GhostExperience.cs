using UnityEngine;
using System;

public class GhostExperience : MonoBehaviour
{
    public static GhostExperience Instance { get; private set; }

    [Header("经验曲线")]
    [SerializeField] int startExpToLevel = 20;
    [SerializeField] float expGrowthFactor = 1.4f;  // 每级经验需求系数

    int currentLevel = 1;
    int currentExp = 0;
    int expToNextLevel = 20;

    /// <summary>
    /// 升级事件（参数是新等级）
    /// </summary>
    public event Action<int> OnLevelUp;

    /// <summary>
    /// 经验变化事件：当前经验、当前等级所需经验、当前等级
    /// 用于经验条 UI
    /// </summary>
    public event Action<int, int, int> OnExpChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        expToNextLevel = startExpToLevel;

        // 初始化时通知一次 UI（例如开局 0 / 20, Lv1）
        OnExpChanged?.Invoke(currentExp, expToNextLevel, currentLevel);
    }

    public void AddExp(int amount)
    {
        if (amount <= 0) return;

        currentExp += amount;

        // 有可能一次获得很多经验，升多级
        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }

        // 统一在处理完所有升级之后，再通知一次经验变化
        OnExpChanged?.Invoke(currentExp, expToNextLevel, currentLevel);
    }

    private void LevelUp()
    {
        currentLevel++;

        // 简单指数成长
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * expGrowthFactor);

        Debug.Log($"Ghost Level Up! Level = {currentLevel}");

        OnLevelUp?.Invoke(currentLevel);

        // 通知升级管理器：来一波 3 选 1
        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.OnGhostLevelUp(currentLevel);
        }
    }

    // 给 UI / 其它系统用的只读访问器（可选，但好用）
    public int CurrentLevel => currentLevel;
    public int CurrentExp => currentExp;
    public int ExpToNextLevel => expToNextLevel;
}
