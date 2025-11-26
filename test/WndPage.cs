using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 设置画质和分辨率，是否全屏
/// </summary>
public class WndPage : MonoBehaviour
{
    public Dropdown dropdown_PictureQuality;
    public Dropdown dropdown_resolution;
    public Toggle toggle_FullScreen;

    // 画面质量选项
    private string[] qualityNames;

    // 常用分辨率选项
    private List<Resolution> resolutions;

    void Start()
    {
        if (dropdown_PictureQuality != null)
            dropdown_PictureQuality.onValueChanged.AddListener(OnPictureQualityChanged);

        if (dropdown_resolution != null)
            dropdown_resolution.onValueChanged.AddListener(OnResolutionChanged);

        if (toggle_FullScreen != null)
            toggle_FullScreen.onValueChanged.AddListener(OnFullScreenChanged);

        InitializePictureQuality();
        InitializeResolution();
        InitializeFullScreen();
    }

    /// <summary>
    /// 初始化画面质量设置
    /// </summary>
    private void InitializePictureQuality()
    {
        if (dropdown_PictureQuality == null) return;

        // 获取所有可用的画面质量等级
        qualityNames = QualitySettings.names;

        // 清空现有选项
        dropdown_PictureQuality.ClearOptions();

        // 添加质量选项
        foreach (string qualityName in qualityNames)
        {
            dropdown_PictureQuality.options.Add(new Dropdown.OptionData(qualityName));
        }

        // 设置当前选中的质量等级
        dropdown_PictureQuality.value = QualitySettings.GetQualityLevel();
        dropdown_PictureQuality.RefreshShownValue();

        // 添加值改变监听
        dropdown_PictureQuality.onValueChanged.AddListener(OnPictureQualityChanged);
    }

    /// <summary>
    /// 画面质量切换事件
    /// </summary>
    private void OnPictureQualityChanged(int qualityIndex)
    {
        if (qualityIndex >= 0 && qualityIndex < qualityNames.Length)
        {
            QualitySettings.SetQualityLevel(qualityIndex, true);
            Debug.Log($"画面质量已切换为: {qualityNames[qualityIndex]}");
        }
    }

    /// <summary>
    /// 初始化分辨率设置
    /// </summary>
    private void InitializeResolution()
    {
        if (dropdown_resolution == null) return;
        resolutions = new List<Resolution>();
        // 获取所有可用的分辨率
        var list =new List<Resolution>(Screen.resolutions);
        foreach (var item in list)
        {
            bool isAdd = false;
            foreach (var resolution in resolutions)
            {
                if(item.width== resolution.width&& item.height == resolution.height)
                {
                    isAdd = true;
                    break;
                }
            }
            if(!isAdd)
            resolutions.Add(item);
        }
        
       // 清空现有选项
       dropdown_resolution.ClearOptions();

        // 创建分辨率选项列表
        var resolutionOptions = new System.Collections.Generic.List<Dropdown.OptionData>();

        foreach (Resolution resolution in resolutions)
        {
            string optionText = $"{resolution.width} x {resolution.height}";
            // 可选：添加刷新率信息
            // optionText += $" ({resolution.refreshRate}Hz)";

            resolutionOptions.Add(new Dropdown.OptionData(optionText));
        }

        // 添加选项到下拉菜单
        dropdown_resolution.AddOptions(resolutionOptions);

        // 设置当前选中的分辨率
        int currentResolutionIndex = GetCurrentResolutionIndex();
        dropdown_resolution.value = currentResolutionIndex;
        dropdown_resolution.RefreshShownValue();

        // 添加值改变监听
        dropdown_resolution.onValueChanged.AddListener(OnResolutionChanged);
    }

    /// <summary>
    /// 获取当前分辨率在列表中的索引
    /// </summary>
    private int GetCurrentResolutionIndex()
    {
        Resolution currentResolution = Screen.currentResolution;

        for (int i = 0; i < resolutions.Count; i++)
        {
            if (resolutions[i].width == currentResolution.width &&
                resolutions[i].height == currentResolution.height)
            {
                return i;//3
            }
        }

        return 0;// 0  1  2   3  4  5  1920*1080 3 
    }

    /// <summary>
    /// 分辨率切换事件
    /// </summary>
    private void OnResolutionChanged(int resolutionIndex)
    {
        if (resolutionIndex >= 0 && resolutionIndex < resolutions.Count)
        {
            Resolution selectedResolution = resolutions[resolutionIndex];

            // 设置分辨率
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);

            Debug.Log($"分辨率已切换为: {selectedResolution.width} x {selectedResolution.height}");
        }
    }

    /// <summary>
    /// 初始化全屏设置
    /// </summary>
    private void InitializeFullScreen()
    {
        if (toggle_FullScreen == null) return;

        // 设置Toggle的初始状态
        toggle_FullScreen.isOn = Screen.fullScreen;

        // 添加值改变监听
        toggle_FullScreen.onValueChanged.AddListener(OnFullScreenChanged);
    }

    /// <summary>
    /// 全屏切换事件
    /// </summary>
    private void OnFullScreenChanged(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;

        // 可选：设置全屏模式
        // Screen.fullScreenMode = isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

        Debug.Log($"全屏模式: {(isFullScreen ? "开启" : "关闭")}");
    }

    /// <summary>
    /// 应用所有图形设置（可选方法，可用于确认按钮）
    /// </summary>
    public void ApplyGraphicsSettings()
    {
        // 应用画面质量
        if (dropdown_PictureQuality != null)
        {
            QualitySettings.SetQualityLevel(dropdown_PictureQuality.value, true);
        }

        // 应用分辨率
        if (dropdown_resolution != null && dropdown_resolution.value < resolutions.Count)
        {
            Resolution selectedResolution = resolutions[dropdown_resolution.value];
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, toggle_FullScreen.isOn);
        }

        // 应用全屏设置
        if (toggle_FullScreen != null)
        {
            Screen.fullScreen = toggle_FullScreen.isOn;
        }

        Debug.Log("图形设置已应用");
    }

    /// <summary>
    /// 重置为默认设置（可选方法）
    /// </summary>
    public void ResetToDefault()
    {
        // 重置画面质量为默认（通常为中等）
        int defaultQuality = 2; // 可根据需要调整
        if (dropdown_PictureQuality != null && defaultQuality < qualityNames.Length)
        {
            dropdown_PictureQuality.value = defaultQuality;
        }

        // 重置分辨率为推荐分辨率
        Resolution recommendedResolution = Screen.currentResolution;
        if (dropdown_resolution != null)
        {
            for (int i = 0; i < resolutions.Count; i++)
            {
                if (resolutions[i].width == recommendedResolution.width &&
                    resolutions[i].height == recommendedResolution.height)
                {
                    dropdown_resolution.value = i;
                    break;
                }
            }
        }

        // 重置全屏设置为窗口模式
        if (toggle_FullScreen != null)
        {
            toggle_FullScreen.isOn = false;
        }

        Debug.Log("已重置为默认设置");
    }

    void OnDestroy()
    {
        // 移除事件监听，避免内存泄漏
        if (dropdown_PictureQuality != null)
            dropdown_PictureQuality.onValueChanged.RemoveListener(OnPictureQualityChanged);

        if (dropdown_resolution != null)
            dropdown_resolution.onValueChanged.RemoveListener(OnResolutionChanged);

        if (toggle_FullScreen != null)
            toggle_FullScreen.onValueChanged.RemoveListener(OnFullScreenChanged);
    }
}