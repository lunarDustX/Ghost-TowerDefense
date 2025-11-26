using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExperienceUI : MonoBehaviour
{
    [Header("UI 元素")]
    [SerializeField] Image expFillImage;   // 用 fillAmount 表示进度
    [SerializeField] TextMeshProUGUI expText;         // 显示 X / Y

    GhostExperience expSystem;

    private void Start()
    {
        if (expSystem == null)
            expSystem = GhostExperience.Instance;

        if (expSystem == null)
        {
            Debug.LogError("[ExperienceUI] 找不到 GhostExperience 实例", this);
            enabled = false;
            return;
        }

        // 订阅经验变化事件
        expSystem.OnExpChanged += OnExpChanged;

        // 初始更新 UI
        OnExpChanged(expSystem.CurrentExp, expSystem.ExpToNextLevel, expSystem.CurrentLevel);
    }

    private void OnDestroy()
    {
        if (expSystem != null)
        {
            expSystem.OnExpChanged -= OnExpChanged;
        }
    }

    private void OnExpChanged(int curExp, int expToNext, int level)
    {
        float ratio = expToNext > 0 ? (float)curExp / expToNext : 0f;
        ratio = Mathf.Clamp01(ratio);

        if (expFillImage != null)
            expFillImage.fillAmount = ratio;

        if (expText != null)
            expText.text = $"Lv {level} ({curExp} / {expToNext})";
    }
}
