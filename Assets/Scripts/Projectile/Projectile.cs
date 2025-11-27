using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 8f;
    public float maxLifeTime = 3f;

    public GameObject hitVfxPrefab;   

    private Transform target;
    private float damage;
    private bool hasHit;

    public void Init(Transform target, float damage)
    {
        this.target = target;
        this.damage = damage;
        Destroy(gameObject, maxLifeTime);
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit || target == null) return;
        if (other.transform.IsChildOf(target) == false) return;

        hasHit = true;

        var hp = other.GetComponentInParent<EnemyHealth>();
        if (hp)
        {
            hp.TakeDamage(damage);
            // 命中特效
            if (hitVfxPrefab != null)
            {
                Instantiate(hitVfxPrefab, target.position, Quaternion.identity);
                //Instantiate(hitVfxPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }

    }
}
