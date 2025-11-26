using UnityEngine;

public class ExplosionShockwave : MonoBehaviour
{
    [Header("整体时长")]
    public float duration = 0.15f;

    [Header("外圈闪白占用比例(0~1)")]
    [Range(0.05f, 0.5f)]
    public float outerFlashPortion = 0.25f;

    [Header("内圈颜色（固定不变）")]
    public Color innerColor = new Color(0.5f, 0.5f, 0.6f);

    [Header("渲染引用")]
    public SpriteRenderer outerFlash;   // 白色外圈
    public SpriteRenderer innerWave;    // 固定颜色内圈

    private float baseScale = 1f;       // 半径对应的直径 scale
    private float timer = 0f;

    /// <summary>
    /// 初始化冲击波，使其从一开始就等于 explosionRadius
    /// </summary>
    public void Init(float explosionRadius)
    {
        float radius = Mathf.Max(0.01f, explosionRadius);

        // 半径 r → 直径 = 2r → 对应 localScale
        baseScale = radius * 2f;
        timer = 0f;

        // 直接一开始就画出完整范围
        transform.localScale = new Vector3(baseScale, baseScale, 1f);

        if (outerFlash != null)
        {
            outerFlash.enabled = true;
            outerFlash.color = Color.white;    // 纯白一圈
        }

        if (innerWave != null)
        {
            innerWave.enabled = true;
            innerWave.color = innerColor;      // 固定颜色
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);

        // 1) 外圈白闪：只在前 outerFlashPortion 时间内存在
        if (outerFlash != null)
        {
            float outerT = t / Mathf.Max(0.0001f, outerFlashPortion);
            outerFlash.enabled = outerT < 1f;
        }

        // 3) 结束
        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
