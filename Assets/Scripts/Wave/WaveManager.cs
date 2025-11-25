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

    // 新增：给 UI 用的开波事件
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

            // 1) 等 preWaveDelay
            if (wave.preWaveDelay > 0)
                yield return new WaitForSeconds(wave.preWaveDelay);

            // 2) 触发 UI：本波开始了！
            OnWaveStart?.Invoke(CurrentWaveNumber);

            // 3) 开始出怪
            yield return StartCoroutine(SpawnWave(wave));

            // 4) 等所有怪清空
            yield return StartCoroutine(WaitUntilNoEnemies());

            // 5) 波间隔
            if (timeBetweenWaves > 0)
                yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private IEnumerator SpawnWave(WaveConfig wave)
    {
        Path path = paths[wave.pathIndex];
        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemy(wave, path);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    void SpawnEnemy(WaveConfig wave, Path path)
    {
        Vector3 pos = path.GetPoint(0);
        EnemyMover e = Instantiate(wave.enemyPrefab, pos, Quaternion.identity);
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
