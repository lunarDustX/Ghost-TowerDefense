using UnityEngine;

public class Path : MonoBehaviour
{
    [Header("Waypoints 按顺序排列")]
    public Transform[] waypoints;

    public int Count => waypoints.Length;

    public Vector3 GetPoint(int index)
    {
        return waypoints[index].position;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                Gizmos.DrawSphere(waypoints[i].position, 0.15f);
            }
        }

        // 最后一个点
        Gizmos.DrawSphere(waypoints[waypoints.Length - 1].position, 0.15f);
    }
#endif
}
