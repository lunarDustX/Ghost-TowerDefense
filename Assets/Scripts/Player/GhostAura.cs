using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GhostAura : MonoBehaviour
{
    [Tooltip("半径")]
    public float radius = 1.5f;

    [Header("检测塔的 Layer")]
    public LayerMask towerLayer;

    [Header("Refs")]
    public CircleCollider2D col;
    public Transform visual;

    private void Start()
    {
        Reset();
    }

    private void Reset()
    {
        if (col == null)
            col = GetComponent<CircleCollider2D>();

        col.radius = radius;
        col.isTrigger = true;

        UpdateVisual(radius);
    }

    private void OnValidate()
    {
        RefreshRadius();
    }

    private void UpdateVisual(float r)
    {
        if (visual)
            visual.localScale = Vector2.one * r * 2f;
    }

    public void RefreshRadius()
    {
        col.radius = radius;
        UpdateVisual(radius);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsTowerLayer(other.gameObject.layer))
            return;

        Tower tower = other.GetComponentInParent<Tower>();
        if (tower != null)
        {
            tower.SetGhostBuffed(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsTowerLayer(other.gameObject.layer))
            return;

        Tower tower = other.GetComponentInParent<Tower>();
        if (tower != null)
        {
            tower.SetGhostBuffed(false);
        }
    }

    private bool IsTowerLayer(int layer)
    {
        return (towerLayer.value & (1 << layer)) != 0;
    }
}
