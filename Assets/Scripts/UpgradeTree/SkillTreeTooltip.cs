using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillTreeTooltip : MonoBehaviour
{
    [Header("根节点（整个 tooltip 面板）")]
    public GameObject root;

    [Header("文本组件")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;

    [Header("屏幕位置偏移")]
    public Vector2 offset = new Vector2(16f, -100f);

    RectTransform Rect => root != null ? root.GetComponent<RectTransform>() : null;

    void Awake()
    {
        if (root == null)
            root = gameObject;
        Hide();
    }

    public void Show(string title, string body, Vector2 screenPos)
    {
        if (root == null) return;

        root.SetActive(true);

        if (titleText != null) titleText.text = title;
        if (bodyText != null) bodyText.text = body;

        var rt = Rect;
        if (rt != null)
            rt.position = screenPos + offset;
    }

    public void Move(Vector2 screenPos)
    {
        if (root == null || !root.activeSelf) return;

        var rt = Rect;
        if (rt != null)
            rt.position = screenPos + offset;
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);
    }
}
