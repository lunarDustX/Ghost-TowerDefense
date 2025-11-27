using UnityEngine;

[ExecuteAlways]
public class RangeIndicator : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] TowerBase tower;             // 绑定所属塔
    [SerializeField] SpriteRenderer sr;           // 画圆圈的 SpriteRenderer

    // 其实没用
    float radiusScale = 1f;

    private void Reset()
    {
        if (sr == null)
            sr = GetComponentInChildren<SpriteRenderer>();

        if (tower == null)
            tower = GetComponentInParent<TowerBase>();

        // 默认隐藏
        sr.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (tower == null)
            return;

        // 取当前攻击范围（包含幽灵 buff）
        float r = Application.isPlaying
            ? (tower.CurrentRange > 0 ? tower.CurrentRange : tower.BaseRange)
            : tower.BaseRange;

        float finalRadius = r * radiusScale;

        // sprite 一般是直径为 1 的圆，所以 scale = 半径 * 2
        float s = finalRadius * 2f;
        sr.transform.localScale = new Vector3(s, s, 1f);
    }

    public void SetVisible(bool visible)
    {
        sr.gameObject.SetActive(visible);
    }
}
