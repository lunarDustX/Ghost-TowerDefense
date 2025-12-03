using UnityEngine;
using DG.Tweening;

public class TheGoal : MonoBehaviour
{
    public static TheGoal Instance { get; private set; }

    [SerializeField] Transform target;

    [Header("缩放参数")]
    public float punchScale = 1.2f;    // 膨胀到 120%
    public float duration = 0.25f;     // 动画时长
    public Ease ease = Ease.OutBack;

    private Vector3 originalScale;
    private Tweener currentTween;

    private void Awake()
    {
        Instance = this;
        originalScale = target.localScale;
    }

    public void PlayHitFX()
    {
        // 中断正在进行的动画
        if (currentTween != null && currentTween.IsActive())
            currentTween.Kill();

        // 从原始缩放 → 放大 → 回到原始
        currentTween = target
            .DOScale(originalScale * punchScale, duration)
            .SetEase(ease)
            .OnComplete(() =>
            {
                target.localScale = originalScale;
            });
    }
}
