using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class WaveUI : MonoBehaviour
{
    [SerializeField] WaveManager waveManager;

    [Header("Refs")]
    [SerializeField] TextMeshProUGUI currentWaveText;
    [SerializeField] GameObject waveBanner;
    [SerializeField] TextMeshProUGUI wavePopupText;

    [Header("Anim")]
    [SerializeField] float popupDuration = 1.2f;  // 显示多久
    [SerializeField] float popupFadeTime = 0.4f;  // 淡出时间

    void Start()
    {
        if (waveManager != null)
            waveManager.OnWaveStart += HandleWaveStart;

        waveBanner.SetActive(false);
    }

    void Update()
    {
        if (waveManager == null)
            return;

        int cur = waveManager.CurrentWaveNumber;
        int total = waveManager.TotalWaves;

        if (cur <= 0)
            currentWaveText.text = $"Wave: 0 / {total}";
        else
            currentWaveText.text = $"Wave: {cur} / {total}";
    }

    private void HandleWaveStart(int waveNumber)
    {
        ShowWavePopup(waveNumber);
    }

    private void ShowWavePopup(int waveNumber)
    {
        StopAllCoroutines();
        StartCoroutine(PopupCoroutine(waveNumber));
    }

    private IEnumerator PopupCoroutine(int waveNumber)
    {
        waveBanner.SetActive(true);
        wavePopupText.text = $"Wave {waveNumber}";

        yield return new WaitForSeconds(popupDuration);

        waveBanner.SetActive(false);
    }
}
