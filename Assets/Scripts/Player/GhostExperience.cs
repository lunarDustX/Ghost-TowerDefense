using UnityEngine;
using System;

public class GhostExperience : MonoBehaviour
{
    public static GhostExperience Instance { get; private set; }

    [Header("经验曲线")]
    [SerializeField] int startExpToLevel = 20;
    [SerializeField] float expGrowthFactor = 1.4f;  // 每级经验需求系数

    [Header("当前状态（只读观察用）")]
    public int currentLevel = 1;
    public int currentExp = 0;
    public int expToNextLevel = 20;

    public event Action<int> OnLevelUp;  // 参数是新等级

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        expToNextLevel = startExpToLevel;
    }

    public void AddExp(int amount)
    {
        currentExp += amount;

        // 有可能一次获得很多经验，升多级
        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }
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
}
