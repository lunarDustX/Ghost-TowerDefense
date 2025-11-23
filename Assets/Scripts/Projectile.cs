using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 8f;
    public float maxLifeTime = 3f;

    private Transform target;
    private float damage;

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
            // 目标死了或消失了
            Destroy(gameObject);
            return;
        }

        // TODO: 当前是软追踪，之后会更改各种不同模式。
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (target == null) return;

        if (other.transform == target)
        {
            EnemyHealth hp = other.GetComponent<EnemyHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}
