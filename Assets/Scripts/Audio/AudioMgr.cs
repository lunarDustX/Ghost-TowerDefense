using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMgr : MonoBehaviour
{
    public static AudioMgr I;

    [Header("Global Volume")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float bgmVolume = 1f;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource bgmSource;

    [Header("SFX Clips")]
    [SerializeField] private List<SFXEntry> sfxEntries = new List<SFXEntry>();
    private Dictionary<SFXType, SFXEntry> sfxDict = new Dictionary<SFXType, SFXEntry>();

    [Header("BGM Clips")]
    [SerializeField] private List<BGMEntry> bgmEntries = new List<BGMEntry>();
    private Dictionary<BGMType, BGMEntry> bgmDict = new Dictionary<BGMType, BGMEntry>();

    // ----------- Structs -----------

    [Serializable]
    public class SFXEntry
    {
        public SFXType type;
        public AudioClip clip;
        [Range(0f, 1f)] public float baseVolume = 1f;
    }

    [Serializable]
    public class BGMEntry
    {
        public BGMType type;
        public AudioClip clip;
        [Range(0f, 1f)] public float baseVolume = 1f;
    }

    // ----------- Init -----------

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;

        // 如果希望跨场景保留，可启用：
        // DontDestroyOnLoad(gameObject);

        InitSFXDict();
        InitBGMdict();

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
        }
    }

    private void Start()
    {
        PlayBGM(BGMType.MainTheme);
    }

    private void InitSFXDict()
    {
        sfxDict.Clear();
        foreach (var entry in sfxEntries)
        {
            if (entry.clip == null) continue;
            if (!sfxDict.ContainsKey(entry.type))
                sfxDict.Add(entry.type, entry);
        }
    }

    private void InitBGMdict()
    {
        bgmDict.Clear();
        foreach (var entry in bgmEntries)
        {
            if (entry.clip == null) continue;
            if (!bgmDict.ContainsKey(entry.type))
                bgmDict.Add(entry.type, entry);
        }
    }

    // ----------- SFX -----------

    public void PlaySFX(SFXType type)
    {
        if (sfxDict.TryGetValue(type, out var entry))
        {
            float finalVolume = sfxVolume * entry.baseVolume;
            sfxSource.PlayOneShot(entry.clip, finalVolume);
        }
        else
        {
            Debug.LogWarning($"未配置音效：{type}");
        }
    }

    // ----------- BGM -----------

    /// <summary>
    /// 直接播放指定BGM
    /// </summary>
    public void PlayBGM(BGMType type, bool loop = true)
    {
        if (!bgmDict.TryGetValue(type, out var entry) || entry.clip == null)
        {
            Debug.LogWarning($"未配置BGM：{type}");
            return;
        }

        bgmSource.clip = entry.clip;
        bgmSource.loop = loop;
        bgmSource.volume = bgmVolume * entry.baseVolume;
        bgmSource.Play();
    }

    /// <summary>
    /// 平滑切换BGM（带淡入淡出）
    /// </summary>
    public void SwitchBGM(BGMType type, float fadeTime = 1f)
    {
        if (!bgmDict.TryGetValue(type, out var entry) || entry.clip == null)
        {
            Debug.LogWarning($"未配置BGM：{type}");
            return;
        }

        if (bgmSource.clip == entry.clip) return;

        StopAllCoroutines();
        StartCoroutine(FadeSwitchBGM(entry, fadeTime));
    }

    private IEnumerator FadeSwitchBGM(BGMEntry newEntry, float fadeTime)
    {
        // 淡出旧曲
        if (bgmSource.isPlaying)
        {
            float t = 0f;
            float startVol = bgmSource.volume;
            while (t < fadeTime)
            {
                t += Time.unscaledDeltaTime;
                bgmSource.volume = Mathf.Lerp(startVol, 0f, t / fadeTime);
                yield return null;
            }
            bgmSource.Stop();
        }

        // 播放新曲
        bgmSource.clip = newEntry.clip;
        bgmSource.volume = 0f;
        bgmSource.Play();

        float t2 = 0f;
        while (t2 < fadeTime)
        {
            t2 += Time.unscaledDeltaTime;
            float targetVol = bgmVolume * newEntry.baseVolume;
            bgmSource.volume = Mathf.Lerp(0f, targetVol, t2 / fadeTime);
            yield return null;
        }
        bgmSource.volume = bgmVolume * newEntry.baseVolume;
    }

    public void StopBGM(float fadeTime = 0.5f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutBGM(fadeTime));
    }

    private IEnumerator FadeOutBGM(float fadeTime)
    {
        float t = 0f;
        float startVol = bgmSource.volume;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(startVol, 0f, t / fadeTime);
            yield return null;
        }
        bgmSource.Stop();
    }
}

// ----------- ENUMS -----------

public enum SFXType
{
    TowerShoot = 100,

    ProjectileHit = 200,

    EnemyDie = 300,
}

public enum BGMType
{
    MainTheme,
}
