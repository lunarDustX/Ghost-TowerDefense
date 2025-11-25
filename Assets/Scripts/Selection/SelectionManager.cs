using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [Header("UI 引用")]
    public SelectionUI selectionUI;

    [Header("可选中的 Layer（塔 + 敌人）")]
    public LayerMask selectableLayer;

    [Header("射线检测最大距离")]
    public float maxRayDistance = 100f;

    private Tower currentTower;
    private EnemyHealth currentEnemy;

    void Update()
    {
        HandleClick();
    }

    void HandleClick()
    {
        // 左键点击
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 origin = mouseWorldPos;

            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero, maxRayDistance, selectableLayer);

            if (hit.collider != null)
            {
                TrySelect(hit.collider.gameObject);
            }
            else
            {
                ClearSelection();
            }
        }

        // 右键取消
        if (Input.GetMouseButtonDown(1))
        {
            ClearSelection();
        }
    }

    void TrySelect(GameObject go)
    {
        // 优先判塔
        Tower tower = go.GetComponentInParent<Tower>();
        EnemyHealth enemy = null;

        if (tower != null)
        {
            SelectTower(tower);
            return;
        }

        // 再判敌人
        enemy = go.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            SelectEnemy(enemy);
            return;
        }

        // 什么都不是，就清空
        ClearSelection();
    }

    void SelectTower(Tower tower)
    {
        currentTower = tower;
        currentEnemy = null;

        if (selectionUI != null)
        {
            selectionUI.ShowTower(tower);
        }
    }

    void SelectEnemy(EnemyHealth enemy)
    {
        currentTower = null;
        currentEnemy = enemy;

        if (selectionUI != null)
        {
            selectionUI.ShowEnemy(enemy);
        }
    }

    void ClearSelection()
    {
        currentTower = null;
        currentEnemy = null;

        if (selectionUI != null)
        {
            selectionUI.Hide();
        }
    }
}
