using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionUI : MonoBehaviour
{
    [Header("根节点（整体显示/隐藏用）")]
    [SerializeField] GameObject root;

    [Header("通用字段")]
    [SerializeField] TextMeshProUGUI titleText;

    [Header("详细行")]
    [SerializeField] TextMeshProUGUI line1Text;


    void Awake()
    {
        if (root == null)
            root = gameObject;

        Hide();
    }

    public void ShowTower(TowerBase tower)
    {
        root.SetActive(true);

        titleText.text = tower.gameObject.name;

        // 示例展示：基础 vs 当前
        line1Text.text = $"Dmg: {tower.CurrentDamage:0.0}  (Base Dmg: {tower.BaseDamage:0.0})";
        //line2Text.text = $"攻速：{(1f / tower.CurrentAttackCooldown):0.0}/s  (基础 {(1f / tower.BaseAttackCooldown):0.0}/s)";
        //line3Text.text = $"范围：{tower.CurrentRange:0.0}  {(tower.IsBuffedByGhost ? "（幽灵加成中）" : "")}";
    }

    public void ShowEnemy(EnemyHealth enemy)
    {
        root.SetActive(true);

        titleText.text = enemy.gameObject.name;

        float hp = enemy.CurrentHealth;
        float maxHp = enemy.MaxHealth;
        float hpPercent = maxHp > 0 ? hp / maxHp * 100f : 0f;

        line1Text.text = $"HP: {hp:0}/{maxHp:0} ({hpPercent:0}% )";
        //line2Text.text = "";   // 以后可拓展：抗性、移速、特殊能力等
        //line3Text.text = "";
    }

    public void Hide()
    {
        root.SetActive(false);
    }
}
