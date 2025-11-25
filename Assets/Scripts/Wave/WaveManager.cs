using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("波次列表（按顺序执行）")]
    public List<WaveConfig> waves = new List<WaveConfig>();

    [Header("场景里的路径列表，按顺序配置")]
    public Path[] paths;

    [Header("波与波之间的间隔（所有敌人清空后）")]
    public float timeBetweenWaves = 3f;

    private int currentWaveIndex = -1;
    private int enemiesAlive = 0;
    private bool running = false;

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
        if (waves.Count == 0)
        {
            Debug.LogWarning("[WaveManager] 没有配置任何 WaveConfig", this);
            return;
        }

        if (paths == null || paths.Length == 0)
        {
            Debug.LogWarning("[WaveManager] paths 为空，请在 Inspector 中配置路径数组", this);
            return;
        }

        StartCoroutine(RunWaves());
    }

    private IEnumerator RunWaves()
    {
        running = true;

        for (int i = 0; i < waves.Count; i++)
        {
            currentWaveIndex = i;
            WaveConfig wave = waves[i];

            // 开波前的额外等待
            if (wave.preWaveDelay > 0f)
                yield return new WaitForSeconds(wave.preWaveDelay);

            Debug.Log($"[WaveManager] 开始第 {i + 1} 波，数量：{wave.enemyCount}, 路径索引：{wave.pathIndex}");

            // 生成本波敌人
            yield return StartCoroutine(SpawnWave(wave));

            // 等到场上没有敌人，才进下一波
            yield return StartCoroutine(WaitUntilNoEnemies());

            // 波与波之间的休息时间
            if (timeBetweenWaves > 0f)
                yield return new WaitForSeconds(timeBetweenWaves);
        }

        Debug.Log("[WaveManager] 所有波次完成");
        running = false;
    }

    private IEnumerator SpawnWave(WaveConfig wave)
    {
        if (wave.enemyPrefab == null)
        {
            Debug.LogError("[WaveManager] WaveConfig 缺少 enemyPrefab", wave);
            yield break;
        }

        Path path = GetPathByIndex(wave.pathIndex);
        if (path == null)
        {
            Debug.LogError($"[WaveManager] WaveConfig 指定的 pathIndex={wave.pathIndex} 无效或 paths 未配置", wave);
            yield break;
        }

        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnOneEnemy(wave, path);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    private Path GetPathByIndex(int index)
    {
        if (paths == null || paths.Length == 0)
            return null;

        if (index < 0 || index >= paths.Length)
        {
            Debug.LogError($"[WaveManager] 请求的路径索引 {index} 越界（paths.Length={paths.Length}）");
            return null;
        }

        return paths[index];
    }

    private void SpawnOneEnemy(WaveConfig wave, Path path)
    {
        Vector3 spawnPos = path.GetPoint(0);

        EnemyMover enemy = Instantiate(wave.enemyPrefab, spawnPos, Quaternion.identity);
        enemy.path = path;

        enemiesAlive++;
    }

    private IEnumerator WaitUntilNoEnemies()
    {
        while (enemiesAlive > 0)
        {
            yield return null;
        }
    }

    private void OnEnemyDestroyed(EnemyHealth enemy)
    {
        enemiesAlive = Mathf.Max(0, enemiesAlive - 1);
        // Debug.Log($"[WaveManager] 敌人销毁，当前剩余：{enemiesAlive}");
    }
}
