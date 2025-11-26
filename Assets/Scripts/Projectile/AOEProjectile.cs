using UnityEngine;

public class AoeProjectile : MonoBehaviour
{
    [Header("移动参数")]
    public float speed = 8f;
    public float maxLifeTime = 3f;



    [Header("命中特效（可选）")]
    public GameObject hitVfxPrefab;


    private float explosionRadius;
    private LayerMask enemyLayer;
    private Vector3 targetPoint;
    private float damage;
    private bool isInited = false;

    public void Init(Vector3 targetPoint, float damage, float explosionRadius, LayerMask enemyLayer)
    {
        this.targetPoint = targetPoint;
        this.damage = damage;
        this.explosionRadius = explosionRadius;
        this.enemyLayer = enemyLayer;

        isInited = true;

        Destroy(gameObject, maxLifeTime);
    }

    void Update()
    {
        if (!isInited) return;

        // 方向：从当前位置指向目标点
        Vector3 dir = (targetPoint - transform.position).normalized;

        transform.position += dir * speed * Time.deltaTime;

        float dist = Vector3.Distance(transform.position, targetPoint);

        // 接近目标点则触发爆炸
        if (dist < 0.1f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        // 范围伤害
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);
        foreach (var hit in hits)
        {
            var hp = hit.GetComponent<EnemyHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }
        }

        // 特效
        if (hitVfxPrefab != null)
        {
            var vfx = Instantiate(hitVfxPrefab, transform.position, Quaternion.identity);

            // 新增：如果特效上有 ExplosionShockwave，则初始化半径
            var shock = vfx.GetComponentInChildren<ExplosionShockwave>();
            if (shock != null)
            {
                shock.Init(explosionRadius);
            }
        }

        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
#endif
}
