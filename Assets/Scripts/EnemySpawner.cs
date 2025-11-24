using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("敌人预制体（必须挂有 EnemyMover + EnemyHealth）")]
    public EnemyMover enemyPrefab;

    [Header("敌人行走路径")]
    public Path path;

    [Header("生成节奏")]
    [Tooltip("开局延迟多少秒再开始生成")]
    public float startDelay = 1f;

    [Tooltip("每两个敌人之间的间隔秒数")]
    public float spawnInterval = 1.5f;

    [Tooltip("是否无限生成敌人")]
    public bool endless = true;

    [Tooltip("若不是无限生成，则限定生成总数量")]
    public int totalToSpawn = 20;

    private int spawnedCount = 0;
    private Coroutine spawnRoutine;

    private void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("[EnemySpawner] enemyPrefab 未设置！", this);
            enabled = false;
            return;
        }

        if (path == null)
        {
            Debug.LogError("[EnemySpawner] path 未设置！", this);
            enabled = false;
            return;
        }

        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        // 开局延迟
        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        while (endless || spawnedCount < totalToSpawn)
        {
            SpawnOneEnemy();
            spawnedCount++;

            yield return new WaitForSeconds(spawnInterval);
        }

        spawnRoutine = null;
    }

    private void SpawnOneEnemy()
    {
        // 敌人起点直接放在路径第一个点上
        Vector3 spawnPos = path.GetPoint(0);

        EnemyMover enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        enemy.path = path;
    }

    // 可选：在需要时从外部手动停止刷怪
    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }
}
