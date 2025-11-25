using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    [HideInInspector] public Path path;

    public float speed = 2f;
    public float reachThreshold = 0.1f;

    [Tooltip("到终点时对玩家造成的伤害")]
    public int leakDamage = 1;

    int currentIndex = 0;

    // 新增：0~1 的路径进度
    public float PathProgress01 { get; private set; }

    void Start()
    {
        if (path == null)
        {
            Debug.LogError("Enemy has no path assigned!", this);
            enabled = false;
            return;
        }

        transform.position = path.GetPoint(0);
        currentIndex = 1;
        UpdatePathProgress();
    }

    void Update()
    {
        if (currentIndex >= path.Count)
            return;

        Vector3 target = path.GetPoint(currentIndex);
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        //if (direction != Vector3.zero)
        //    transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

        if (Vector3.Distance(transform.position, target) < reachThreshold)
        {
            currentIndex++;

            if (currentIndex >= path.Count)
            {
                OnReachGoal();
                PathProgress01 = 1f;
                return;
            }
        }

        UpdatePathProgress();
    }

    void UpdatePathProgress()
    {
        // 简单估算：按照路点进度来算，够用且单调递增
        // 0 在起点，1 在终点
        int lastIndex = path.Count - 1;
        if (lastIndex <= 0)
        {
            PathProgress01 = 0f;
            return;
        }

        // 当前位置介于 (currentIndex-1) 和 currentIndex 之间
        int prevIndex = Mathf.Clamp(currentIndex - 1, 0, lastIndex - 1);
        Vector3 prevPoint = path.GetPoint(prevIndex);
        Vector3 nextPoint = path.GetPoint(Mathf.Clamp(currentIndex, 0, lastIndex));

        float segmentLength = Vector3.Distance(prevPoint, nextPoint);
        float t = 0f;
        if (segmentLength > 0.001f)
        {
            float distToPrev = Vector3.Distance(transform.position, prevPoint);
            t = Mathf.Clamp01(distToPrev / segmentLength);
        }

        float segmentIndex = prevIndex + t;
        PathProgress01 = Mathf.Clamp01(segmentIndex / lastIndex);
    }

    private void OnReachGoal()
    {
        Debug.Log("Enemy reached the goal!");

        // 敌人成功到终点 → 扣玩家生命
        if (GameLifeManager.Instance != null)
        {
            GameLifeManager.Instance.LoseLife(leakDamage);
        }

        Destroy(gameObject);
    }
}
