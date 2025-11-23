using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GhostAura : MonoBehaviour
{
    [Header("检测塔的 Layer")]
    public LayerMask towerLayer;

    private void Reset()
    {
        // 自动设置成 Trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsTowerLayer(other.gameObject.layer))
            return;

        Tower tower = other.GetComponent<Tower>();
        if (tower != null)
        {
            tower.SetGhostBuffed(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsTowerLayer(other.gameObject.layer))
            return;

        Tower tower = other.GetComponent<Tower>();
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
