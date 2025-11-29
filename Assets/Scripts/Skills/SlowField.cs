using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class SlowField : MonoBehaviour
{
    [Header("领域参数")]
    public float radius = 1f;

    [Tooltip("速度倍率，例如 0.5 = -50% 速度")]
    public float slowMultiplier = 0.5f;
    public float duration = 3f;

    [Header("Refs")]
    [SerializeField] Transform visual;

    private CircleCollider2D col;
    private readonly HashSet<EnemyMover> affected = new HashSet<EnemyMover>();

    void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;

        ApplyRadius(radius);

        // 持续 duration 秒后自动消失
        Destroy(gameObject, duration);
    }

    /// <summary>
    /// 初始化/更新半径（包括碰撞和视觉）
    /// </summary>
    public void ApplyRadius(float r)
    {
        radius = r;

        if (col != null)
            col.radius = radius;

        if (visual != null)
        {
            visual.localScale = Vector3.one * radius * 2f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var mover = other.GetComponentInParent<EnemyMover>();
        if (mover == null) return;

        if (affected.Add(mover))
        {
            mover.AddSpeedMultiplier(1-slowMultiplier);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var mover = other.GetComponentInParent<EnemyMover>();
        if (mover == null) return;

        if (affected.Remove(mover))
        {
            mover.RemoveSpeedMultiplier(1-slowMultiplier);
        }
    }

    private void OnDestroy()
    {
        // 领域消失时，把还在里面的怪恢复速度
        foreach (var mover in affected)
        {
            if (mover != null)
            {
                mover.RemoveSpeedMultiplier(slowMultiplier);
            }
        }
        affected.Clear();
    }
}
