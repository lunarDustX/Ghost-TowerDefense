using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GhostAura : MonoBehaviour
{
    [Tooltip("半径")]
    public float radius = 1.5f;

    [Header("检测塔的 Layer")]
    public LayerMask towerLayer;

    [Header("Refs")]
    [SerializeField] CircleCollider2D col;
    [SerializeField] Transform visual;

    private void Start()
    {
        InitColliderAndVisual();
    }

    private void Reset()
    {
        InitColliderAndVisual();
    }

    private void InitColliderAndVisual()
    {
        if (col == null)
            col = GetComponent<CircleCollider2D>();

        col.isTrigger = true;
        RefreshRadius();
    }

    private void OnValidate()
    {
        if (col == null)
            col = GetComponent<CircleCollider2D>();

        if (col != null)
            RefreshRadius();
    }

    private void UpdateVisual(float r)
    {
        if (visual)
            visual.localScale = Vector2.one * r * 2f;
    }

    public void RefreshRadius()
    {
        if (col != null)
            col.radius = radius;

        UpdateVisual(radius);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsTowerLayer(other.gameObject.layer))
            return;

        // 这里改成找接口，而不是具体 Tower
        var buffable = other.GetComponentInParent<IGhostBuffable>();
        if (buffable != null)
        {
            buffable.SetGhostBuffed(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsTowerLayer(other.gameObject.layer))
            return;

        var buffable = other.GetComponentInParent<IGhostBuffable>();
        if (buffable != null)
        {
            buffable.SetGhostBuffed(false);
        }
    }

    private bool IsTowerLayer(int layer)
    {
        return (towerLayer.value & (1 << layer)) != 0;
    }
}
