using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LifeUI : MonoBehaviour
{
    private GameLifeManager lifeManager;

    [Header("Refs")]
    [SerializeField] TextMeshPro lifeText;

    private void Start()
    {
        if (lifeManager == null)
            lifeManager = GameLifeManager.Instance;

        if (lifeManager == null)
        {
            Debug.LogError("[LifeUI] 找不到 GameLifeManager 实例", this);
            enabled = false;
            return;
        }

        // 订阅事件
        lifeManager.OnLifeChanged += OnLifeChanged;

        // 初始化 UI
        OnLifeChanged(lifeManager.currentLife, lifeManager.maxLife);
    }

    private void OnDestroy()
    {
        if (lifeManager != null)
        {
            lifeManager.OnLifeChanged -= OnLifeChanged;
        }
    }

    private void OnLifeChanged(int current, int max)
    {
        if (lifeText != null)
        {
            lifeText.text = $"HP: {current}/{max}";
        }
    }
}
