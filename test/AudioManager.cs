using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// 主要用于设置背景音效和保存设置音效音量大小
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("UI Controls")]
    public Slider slider_Global;
    public Slider slider_BGM;
    public Slider slider_Effect;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Mixer Parameters")]
    public string globalVolumeParam = "MasterVolume";
    public string bgmVolumeParam = "BGMVolume";
    public string effectVolumeParam = "EffectVolume";

    // 音量键名，用于PlayerPrefs保存
    private const string GLOBAL_VOLUME_KEY = "GlobalVolume1";
    private const string BGM_VOLUME_KEY = "BGMVolume1";
    private const string EFFECT_VOLUME_KEY = "EffectVolume1";

    public AudioSource audioSource;

    // BGM暂停/恢复相关变量
    private float savedBGMVolume; // 保存暂停前的BGM音量
    private bool isBGMPaused = false; // 标记BGM是否被暂停

    public void PlayAudio(BuildType buildType)
    {
        audioSource.clip = Resources.Load<AudioClip>("音乐/" + buildType.ToString());
        audioSource.Play();
    }
    public static AudioManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (slider_Global != null)
            slider_Global.onValueChanged.AddListener(OnGlobalVolumeChanged);

        if (slider_BGM != null)
            slider_BGM.onValueChanged.AddListener(OnBGMVolumeChanged);

        if (slider_Effect != null)
            slider_Effect.onValueChanged.AddListener(OnEffectVolumeChanged);
        InitializeAudioSettings();
    }

    /// <summary>
    /// 初始化音频设置
    /// </summary>
    private void InitializeAudioSettings()
    {
        // 初始化全局音量
        if (slider_Global != null)
        {
            float savedGlobalVolume = PlayerPrefs.GetFloat(GLOBAL_VOLUME_KEY, 0.4f);
            slider_Global.value = savedGlobalVolume;
            SetMixerVolume(globalVolumeParam, savedGlobalVolume);
        }

        // 初始化BGM音量
        if (slider_BGM != null)
        {
            float savedBGMVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 0.4f);
            slider_BGM.value = savedBGMVolume;
            SetMixerVolume(bgmVolumeParam, savedBGMVolume);
        }

        // 初始化特效音量
        if (slider_Effect != null)
        {
            float savedEffectVolume = PlayerPrefs.GetFloat(EFFECT_VOLUME_KEY, 0.4f);
            slider_Effect.value = savedEffectVolume;
            SetMixerVolume(effectVolumeParam, savedEffectVolume);
        }
    }

    /// <summary>
    /// 全局音量改变事件
    /// </summary>
    private void OnGlobalVolumeChanged(float volume)
    {
        SetMixerVolume(globalVolumeParam, volume);
        PlayerPrefs.SetFloat(GLOBAL_VOLUME_KEY, volume);
        PlayerPrefs.Save();

        Debug.Log($"全局音量已调整: {volume:F2}");
    }

    /// <summary>
    /// BGM音量改变事件
    /// </summary>
    private void OnBGMVolumeChanged(float volume)
    {
        // 如果BGM不是被暂停状态，才更新音量
        if (!isBGMPaused)
        {
            SetMixerVolume(bgmVolumeParam, volume);
            savedBGMVolume = volume; // 更新保存的音量值
        }
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, volume);
        PlayerPrefs.Save();

        Debug.Log($"BGM音量已调整: {volume:F2}");
    }

    /// <summary>
    /// 特效音量改变事件
    /// </summary>
    private void OnEffectVolumeChanged(float volume)
    {
        SetMixerVolume(effectVolumeParam, volume);
        PlayerPrefs.SetFloat(EFFECT_VOLUME_KEY, volume);
        PlayerPrefs.Save();

        Debug.Log($"特效音量已调整: {volume:F2}");
    }

    /// <summary>
    /// 设置AudioMixer音量
    /// </summary>
    /// <param name="parameterName">混音器参数名</param>
    /// <param name="volume">音量值 (0-1)</param>
    private void SetMixerVolume(string parameterName, float volume)
    {
        if (audioMixer != null)
        {
            // 将0-1的线性值转换为分贝值(-80dB to 0dB)
            float dB = VolumeTodB(volume);
            audioMixer.SetFloat(parameterName, dB);
        }
        else
        {
            // 如果没有AudioMixer，直接设置AudioListener（简化方案）
            if (parameterName == globalVolumeParam)
            {
                AudioListener.volume = volume;
            }
        }
    }

    /// <summary>
    /// 将线性音量值转换为分贝值
    /// </summary>
    private float VolumeTodB(float volume)
    {
        if (volume <= 0.0001f) // 防止log(0)
            return -80f;

        return Mathf.Log10(volume) * 20f;
    }

    /// <summary>
    /// 将分贝值转换为线性音量值
    /// </summary>
    private float dBToVolume(float dB)
    {
        return Mathf.Pow(10f, dB / 20f);
    }

    /// <summary>
    /// 静音所有音频
    /// </summary>
    public void MuteAll()
    {
        if (slider_Global != null)
        {
            slider_Global.value = 0f;
        }

        Debug.Log("所有音频已静音");
    }

    /// <summary>
    /// 恢复默认音量设置
    /// </summary>
    public void ResetToDefault()
    {
        float defaultVolume = 0.8f;

        if (slider_Global != null)
        {
            slider_Global.value = defaultVolume;
        }

        if (slider_BGM != null)
        {
            slider_BGM.value = defaultVolume;
        }

        if (slider_Effect != null)
        {
            slider_Effect.value = defaultVolume;
        }

        Debug.Log("音频设置已重置为默认");
    }

    /// <summary>
    /// 获取当前音量值
    /// </summary>
    public float GetGlobalVolume()
    {
        return slider_Global != null ? slider_Global.value : 1f;
    }

    /// <summary>
    /// 获取当前BGM音量值
    /// </summary>
    public float GetBGMVolume()
    {
        return slider_BGM != null ? slider_BGM.value : 1f;
    }

    /// <summary>
    /// 获取当前特效音量值
    /// </summary>
    public float GetEffectVolume()
    {
        return slider_Effect != null ? slider_Effect.value : 1f;
    }

    /// <summary>
    /// 设置全局音量（外部调用）
    /// </summary>
    public void SetGlobalVolume(float volume)
    {
        if (slider_Global != null)
        {
            slider_Global.value = Mathf.Clamp01(volume);
        }
    }

    /// <summary>
    /// 设置BGM音量（外部调用）
    /// </summary>
    //public void SetBGMVolume(float volume)
    //{
    //    if (slider_BGM != null)
    //    {
    //        slider_BGM.value = Mathf.Clamp01(volume);
    //    }
    //}

    ///// <summary>
    ///// 设置特效音量（外部调用）
    ///// </summary>
    //public void SetEffectVolume(float volume)
    //{
    //    if (slider_Effect != null)
    //    {
    //        slider_Effect.value = Mathf.Clamp01(volume);
    //    }
    //}

    ///// <summary>
    ///// 临时静音BGM（用于剧情等场景）
    ///// </summary>
    public void TemporaryMuteBGM(bool mute)
    {
        if (mute)
        {
            SetMixerVolume(bgmVolumeParam, 0f);
        }
        else
        {
           // 恢复之前的BGM音量
            float bgmVolume = GetBGMVolume();
            SetMixerVolume(bgmVolumeParam, bgmVolume);
        }
    }

    /// <summary>
    /// 暂停BGM（用于模型预览等场景）
    /// </summary>
    public void PauseBGM()
    {
        if (!isBGMPaused)
        {
            // 保存当前BGM音量
            savedBGMVolume = GetBGMVolume();
            // 将BGM音量设为0
            SetMixerVolume(bgmVolumeParam, 0f);
            isBGMPaused = true;
            Debug.Log("BGM已暂停");
        }
    }

    /// <summary>
    /// 恢复BGM播放
    /// </summary>
    public void ResumeBGM()
    {
        if (isBGMPaused)
        {
            // 恢复之前保存的BGM音量
            SetMixerVolume(bgmVolumeParam, savedBGMVolume);
            // 更新滑块显示
            if (slider_BGM != null)
            {
                slider_BGM.value = savedBGMVolume;
            }
            isBGMPaused = false;
            Debug.Log("BGM已恢复");
        }
    }

    void OnDestroy()
    {
        // 移除事件监听，避免内存泄漏
        if (slider_Global != null)
            slider_Global.onValueChanged.RemoveListener(OnGlobalVolumeChanged);

        if (slider_BGM != null)
            slider_BGM.onValueChanged.RemoveListener(OnBGMVolumeChanged);

        if (slider_Effect != null)
            slider_Effect.onValueChanged.RemoveListener(OnEffectVolumeChanged);
    }

    /// <summary>
    /// 在编辑器中测试音量设置
    /// </summary>
    [ContextMenu("Test Audio Settings")]
    private void TestAudioSettings()
    {
        Debug.Log($"当前全局音量: {GetGlobalVolume()}");
        Debug.Log($"当前BGM音量: {GetBGMVolume()}");
      Debug.Log($"当前特效音量: {GetEffectVolume()}");
   }
}