using UnityEngine;

[CreateAssetMenu(menuName = "Data/WaveConfig")]
public class WaveConfig : ScriptableObject
{
    public EnemyMover enemyPrefab;

    [Header("走哪条路径（由 WaveManager 的 paths 数组决定）")]
    [Tooltip("0 = paths[0], 1 = paths[1] ...")]
    public int pathIndex = 0;

    [Header("数量 & 频率")]
    public int enemyCount = 10;
    public float spawnInterval = 1.0f;

    [Header("开波前额外等待时间")]
    public float preWaveDelay = 1.0f;
}
