using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaveManager : MonoBehaviour
{
    [Header("波次列表（按顺序执行）")]
    public List<WaveConfig> waves = new List<WaveConfig>();

    [Header("场景里的路径列表，按顺序配置")]
    public Path[] paths;

    [Header("波与波之间的间隔（所有敌人清空后）")]
    public float timeBetweenWaves = 3f;

    public int CurrentWaveNumber => currentWaveIndex + 1;
    public int TotalWaves => waves.Count;

    // 给 UI 用的开波事件
    public event Action<int> OnWaveStart;

    private int currentWaveIndex = -1;
    private int enemiesAlive = 0;

    private void OnEnable()
    {
        EnemyHealth.OnAnyEnemyDestroyed += OnEnemyDestroyed;
    }

    private void OnDisable()
    {
        EnemyHealth.OnAnyEnemyDestroyed -= OnEnemyDestroyed;
    }

    void Start()
    {
        StartCoroutine(RunWaves());
    }

    private IEnumerator RunWaves()
    {
        for (int i = 0; i < waves.Count; i++)
        {
            currentWaveIndex = i;
            WaveConfig wave = waves[i];

            // 1) 全局 preWaveDelay
            if (wave.preWaveDelay > 0)
                yield return new WaitForSeconds(wave.preWaveDelay);

            // 2) UI：本波开始
            OnWaveStart?.Invoke(CurrentWaveNumber);

            // 3) 同时在多条路出怪（等待所有线路都刷完）
            yield return StartCoroutine(SpawnWave(wave));

            // 4) 等怪全死光
            yield return StartCoroutine(WaitUntilNoEnemies());

            // 5) 波间隔
            if (timeBetweenWaves > 0)
                yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    /// <summary>
    /// 一波：并行刷多条线路，直到所有线路都刷完才结束
    /// </summary>
    private IEnumerator SpawnWave(WaveConfig wave)
    {
        if (wave.lanes == null || wave.lanes.Count == 0)
            yield break;

        int lanesRemaining = wave.lanes.Count;

        foreach (var lane in wave.lanes)
        {
            // 为每条路线开一个子协程
            StartCoroutine(SpawnLane(lane, () =>
            {
                lanesRemaining--;
            }));
        }

        // 等所有线路都刷完（不管怪死没死）
        while (lanesRemaining > 0)
        {
            yield return null;
        }
    }

    /// <summary>
    /// 单条路线按自己的间隔/延迟刷怪
    /// </summary>
    private IEnumerator SpawnLane(WaveLaneConfig lane, Action onDone)
    {
        // 路线自己的延迟
        if (lane.laneStartDelay > 0)
            yield return new WaitForSeconds(lane.laneStartDelay);

        if (lane.enemyPrefab == null)
        {
            onDone?.Invoke();
            yield break;
        }

        // 找到对应路径
        if (lane.pathIndex < 0 || lane.pathIndex >= paths.Length)
        {
            Debug.LogError($"[WaveManager] 路线 pathIndex={lane.pathIndex} 超出 paths 数组范围", this);
            onDone?.Invoke();
            yield break;
        }

        Path path = paths[lane.pathIndex];

        for (int i = 0; i < lane.enemyCount; i++)
        {
            SpawnEnemy(lane.enemyPrefab, path);

            if (lane.spawnInterval > 0)
                yield return new WaitForSeconds(lane.spawnInterval);
            else
                yield return null; // 下一帧
        }

        onDone?.Invoke();
    }

    void SpawnEnemy(EnemyMover prefab, Path path)
    {
        Vector3 pos = path.GetPoint(0);
        EnemyMover e = Instantiate(prefab, pos, Quaternion.identity);
        e.path = path;
        enemiesAlive++;
    }

    private IEnumerator WaitUntilNoEnemies()
    {
        while (enemiesAlive > 0)
            yield return null;
    }

    private void OnEnemyDestroyed(EnemyHealth e)
    {
        enemiesAlive = Mathf.Max(0, enemiesAlive - 1);
    }
}
