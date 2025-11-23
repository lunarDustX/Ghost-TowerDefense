using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

public class UpgradeUI : MonoBehaviour
{
    [Serializable]
    public class UpgradeOptionSlot
    {
        public Button button;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
        public Image iconImage;
    }

    [SerializeField] GameObject panel;
    [SerializeField] UpgradeOptionSlot[] slots;

    private Action<UpgradeData> onSelectedCallback;
    private List<UpgradeData> currentOptions;

    private void Awake()
    {
        panel.SetActive(false);
    }

    public void Show(List<UpgradeData> options, Action<UpgradeData> onSelected)
    {
        panel.SetActive(true);

        currentOptions = options;
        onSelectedCallback = onSelected;

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < options.Count)
            {
                var data = options[i];
                var slot = slots[i];

                slot.titleText.text = data.displayName;
                slot.descriptionText.text = data.description;
                if (slot.iconImage != null)
                {
                    slot.iconImage.sprite = data.icon;
                    slot.iconImage.enabled = (data.icon != null);
                }

                int index = i; // 闭包
                slot.button.onClick.RemoveAllListeners();
                slot.button.onClick.AddListener(() => OnClickOption(index));

                slot.button.gameObject.SetActive(true);
            }
            else
            {
                slots[i].button.gameObject.SetActive(false);
            }
        }
    }

    private void OnClickOption(int index)
    {
        if (currentOptions == null || index < 0 || index >= currentOptions.Count)
            return;

        UpgradeData chosen = currentOptions[index];

        // 先关 UI 再回调
        gameObject.SetActive(false);

        onSelectedCallback?.Invoke(chosen);
    }
}
