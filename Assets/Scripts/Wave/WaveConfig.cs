using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WaveLaneConfig
{
    [Header("这条路上的敌人预制体")]
    public EnemyMover enemyPrefab;

    [Header("走哪条路径（由 WaveManager 的 paths 数组决定）")]
    [Tooltip("0 = paths[0], 1 = paths[1] ...")]
    public int pathIndex = 0;

    [Header("数量 & 频率")]
    public int enemyCount = 10;
    public float spawnInterval = 1f;

    [Header("本条路线相对于本波开始的额外延迟")]
    public float laneStartDelay = 1f;
}

[CreateAssetMenu(menuName = "Data/WaveConfig")]
public class WaveConfig : ScriptableObject
{
    [Header("开波前额外等待时间（全局）")]
    public float preWaveDelay = 1f;

    [Header("本波在各条路径上的出怪配置")]
    public List<WaveLaneConfig> lanes = new List<WaveLaneConfig>();
}
