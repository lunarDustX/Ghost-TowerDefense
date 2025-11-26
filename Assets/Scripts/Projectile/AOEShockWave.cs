using UnityEngine;

public class AOEShockWave : MonoBehaviour
{
    [Header("动画时长")]
    public float duration = 0.2f;

    [Header("渲染")]
    public SpriteRenderer sr;

    private float finalRadius = 1f;   // 外部传入
    private float timer;

    /// <summary>
    /// 初始化 ShockWave，使其扩散到指定半径
    /// </summary>
    public void Init(float explosionRadius)
    {
        finalRadius = explosionRadius;
        timer = 0f;

        // 初始缩到 0
        transform.localScale = Vector3.zero;

        if (sr != null)
        {
            var c = sr.color;
            c.a = 1f;
            sr.color = c;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);

        // scale = 半径 * 2（因为 sprite 默认直径=1）
        float scale = Mathf.Lerp(0f, finalRadius * 2f, t);
        transform.localScale = new Vector3(scale, scale, 1f);

        // alpha → fade out
        if (sr != null)
        {
            var c = sr.color;
            c.a = 1f - t;
            sr.color = c;
        }

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
