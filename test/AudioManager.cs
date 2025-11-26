using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// 主要负责配置背景音乐和音效，音量大小
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

    // 用PlayerPrefs保存音量
    private const string GLOBAL_VOLUME_KEY = "GlobalVolume1";
    private const string BGM_VOLUME_KEY = "BGMVolume1";
    private const string EFFECT_VOLUME_KEY = "EffectVolume1";

    [Header("Audio Sources")]
    public AudioSource bgmAudioSource;
    public AudioSource sfxAudioSource;

    // BGM暂停/恢复/关闭管理
    private float savedBGMVolume;
    private bool isBGMPaused = false;
    
    // Audio Priority System
    private bool isAudioPriority = false;

    public void PlayAudio(BuildType buildType)
    {
        if (sfxAudioSource == null)
        {
            Debug.LogWarning("SFX AudioSource is not assigned!");
            return;
        }

        AudioClip clip = Resources.Load<AudioClip>("建筑/" + buildType.ToString());
        if (clip == null)
        {
            Debug.LogWarning($"Audio clip not found for BuildType: {buildType}");
            return;
        }

        // Stop current SFX if playing
        if (sfxAudioSource.isPlaying)
        {
            sfxAudioSource.Stop();
        }

        sfxAudioSource.clip = clip;
        sfxAudioSource.Play();
        Debug.Log($"Playing SFX: {buildType}");
    }

    /// <summary>
    /// 播放BGM
    /// </summary>
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (bgmAudioSource == null)
        {
            Debug.LogWarning("BGM AudioSource is not assigned!");
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning("BGM clip is null!");
            return;
        }

        // Stop current BGM if playing
        if (bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
        }

        // Pause SFX when BGM starts
        if (sfxAudioSource != null && sfxAudioSource.isPlaying)
        {
            sfxAudioSource.Pause();
        }

        bgmAudioSource.clip = clip;
        bgmAudioSource.loop = loop;
        bgmAudioSource.Play();
        isAudioPriority = true;
        Debug.Log("Playing BGM");
    }

    /// <summary>
    /// 停止BGM
    /// </summary>
    public void StopBGM()
    {
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
            isAudioPriority = false;
            
            // 恢复SFX播放（如果之前被暂停）
            if (sfxAudioSource != null && !sfxAudioSource.isPlaying)
            {
                sfxAudioSource.UnPause();
            }

            Debug.Log("BGM stopped");
        }
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

        // 初始化音效音量
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

        Debug.Log($"全局音量已设置: {volume:F2}");
    }

    /// <summary>
    /// BGM音量改变事件
    /// </summary>
    private void OnBGMVolumeChanged(float volume)
    {
        // 如果BGM处于暂停状态，则不改变音量
        if (!isBGMPaused)
        {
            SetMixerVolume(bgmVolumeParam, volume);
            savedBGMVolume = volume;
        }
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, volume);
        PlayerPrefs.Save();

        Debug.Log($"BGM音量已设置: {volume:F2}");
    }

    /// <summary>
    /// 音效音量改变事件
    /// </summary>
    private void OnEffectVolumeChanged(float volume)
    {
        SetMixerVolume(effectVolumeParam, volume);
        PlayerPrefs.SetFloat(EFFECT_VOLUME_KEY, volume);
        PlayerPrefs.Save();

        Debug.Log($"音效音量已设置: {volume:F2}");
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
            // 将0-1数值转换为分贝值(-80dB to 0dB)
            float dB = VolumeTodB(volume);
            audioMixer.SetFloat(parameterName, dB);
        }
        else
        {
            // 如果没有AudioMixer，直接改变AudioListener音量处理
            if (parameterName == globalVolumeParam)
            {
                AudioListener.volume = volume;
            }
        }
    }

    /// <summary>
    /// 将线性数值转换为分贝值
    /// </summary>
    private float VolumeTodB(float volume)
    {
        if (volume <= 0.0001f)
            return -80f;

        return Mathf.Log10(volume) * 20f;
    }

    /// <summary>
    /// 将分贝值转换为线性数值
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
    /// 恢复到默认设置
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

        Debug.Log("音频设置已恢复为默认");
    }

    /// <summary>
    /// 获取当前全局值
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
    /// 获取当前音效音量值
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
    /// 临时静音BGM，用于演示预览
    /// </summary>
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
    /// 暂停BGM音量，模型预览时使用
    /// </summary>
    public void PauseBGM()
    {
        if (!isBGMPaused)
        {
            // 保存当前BGM音量
            savedBGMVolume = GetBGMVolume();
            // 将BGM音量设置为0
            SetMixerVolume(bgmVolumeParam, 0f);
            isBGMPaused = true;
            
            // 暂停BGM播放
            if (bgmAudioSource != null && bgmAudioSource.isPlaying)
            {
                bgmAudioSource.Pause();
            }
            
            Debug.Log("BGM已暂停");
        }
    }

    /// <summary>
    /// 恢复BGM音量
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
            
            // 恢复BGM播放
            if (bgmAudioSource != null && !bgmAudioSource.isPlaying)
            {
                bgmAudioSource.UnPause();
            }
            
            Debug.Log("BGM已恢复");
        }
    }

    void OnDestroy()
    {
        // 移除事件监听，防止内存泄漏
        if (slider_Global != null)
            slider_Global.onValueChanged.RemoveListener(OnGlobalVolumeChanged);

        if (slider_BGM != null)
            slider_BGM.onValueChanged.RemoveListener(OnBGMVolumeChanged);

        if (slider_Effect != null)
            slider_Effect.onValueChanged.RemoveListener(OnEffectVolumeChanged);
    }

    /// <summary>
    /// 在编辑器中测试音频设置
    /// </summary>
    [ContextMenu("Test Audio Settings")]
    private void TestAudioSettings()
    {
        Debug.Log($"当前全局音量: {GetGlobalVolume()}");
        Debug.Log($"当前BGM音量: {GetBGMVolume()}");
        Debug.Log($"当前音效音量: {GetEffectVolume()}");
    }
}
