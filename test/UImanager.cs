using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildType
{
    元朝观星台,
    长城,
    总统府,
    该阶段时期特色,
    大雁塔,
    宋斗拱,
    唐榫卯,
    永宁寺,
    汉朝斗拱,
    石阙,
    秦阙,
    草屋,
    采桑猎纺拓本宫室图,
    石窟,
    清明上河图,
    景墙
}
/// <summary>
/// INstance中控，控制页面的生命周期
/// </summary>
public class UImanager : MonoBehaviour
{
    public static UImanager Instance;

    public ShowPage showPage;
    public SettingWnd settingWnd;

    // Start is called before the first frame update
    //单例 设计模式 
    void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (settingWnd.gameObject.activeSelf)
            {
                settingWnd.Close();
            }
            else
            {
                settingWnd.Open();
            }
        }
    }
}
