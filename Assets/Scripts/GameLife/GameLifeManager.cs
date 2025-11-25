using System;
using UnityEngine;

public class GameLifeManager : MonoBehaviour
{
    public static GameLifeManager Instance { get; private set; }

    [Header("玩家生命设置")]
    public int maxLife = 20;

    [HideInInspector] public int currentLife;

    // 当生命变化：参数 (当前生命, 最大生命)
    public event Action<int, int> OnLifeChanged;

    // 当生命降到 0（或以下）
    public event Action OnGameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        currentLife = maxLife;
    }

    /// <summary>
    /// 敌人漏过去时调用，扣除生命
    /// </summary>
    public void LoseLife(int amount)
    {
        if (amount <= 0) return;

        currentLife -= amount;
        if (currentLife < 0)
            currentLife = 0;

        OnLifeChanged?.Invoke(currentLife, maxLife);

        if (currentLife <= 0)
        {
            HandleGameOver();
        }
    }

    private void HandleGameOver()
    {
        Debug.Log("[GameLifeManager] Game Over!");

        OnGameOver?.Invoke();

        // TODO：这里可以：
        // - 弹出失败界面
        // - 暂停游戏 Time.timeScale = 0
        // - 切场景等
    }
}
