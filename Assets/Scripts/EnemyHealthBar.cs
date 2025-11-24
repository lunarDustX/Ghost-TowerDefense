using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("血条填充部分")]
    public Transform fill;   // 缩放 X 轴

    private float maxScaleX = 1f;
    private EnemyHealth enemy;

    void Awake()
    {
        if (enemy == null)
            enemy = GetComponentInParent<EnemyHealth>();

        if (fill != null)
        {
            maxScaleX = fill.localScale.x;
        }
    }

    void LateUpdate()
    {
        if (enemy == null || fill == null)
            return;

        float t = Mathf.Clamp01(enemy.CurrentHealth / enemy.MaxHealth);
        fill.localScale = new Vector3(maxScaleX * t, fill.localScale.y, fill.localScale.z);
    }

    public void SetEnemy(EnemyHealth target)
    {
        enemy = target;
    }
}
