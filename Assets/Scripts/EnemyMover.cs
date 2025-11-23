using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    [Header("Movement")]
    public Path path;
    public float speed = 2f;
    public float reachThreshold = 0.1f;

    private int currentIndex = 0;

    void Start()
    {
        if (path == null)
        {
            Debug.LogError("Enemy has no path assigned!", this);
            enabled = false;
            return;
        }

        // 开局定位到起点
        transform.position = path.GetPoint(0);
        currentIndex = 1;
    }

    void Update()
    {
        if (currentIndex >= path.Count)
            return;

        Vector3 target = path.GetPoint(currentIndex);
        Vector3 direction = (target - transform.position).normalized;

        transform.position += direction * speed * Time.deltaTime;

        // 选装：看向运动方向
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

        // 抵达路点
        if (Vector3.Distance(transform.position, target) < reachThreshold)
        {
            currentIndex++;

            if (currentIndex >= path.Count)
            {
                OnReachGoal();
            }
        }
    }

    private void OnReachGoal()
    {
        // 通知系统：敌人冲终点了
        Debug.Log("Enemy reached the goal!");

        // 游戏管理器（之后实现）扣 HP 或标记漏怪
        Destroy(gameObject);
    }
}
