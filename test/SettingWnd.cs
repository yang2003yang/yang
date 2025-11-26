using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

/// <summary>
/// 打开设置界面
/// </summary>
public class SettingWnd : MonoBehaviour
{
    public Toggle toggle0;
    public Toggle toggle1;
    public Toggle toggle2;
    public FirstPersonController firstPersonController;
    public void Open()
    {
        firstPersonController.enabled = false;
        firstPersonController.GetComponent<PlayerCtr>().enabled = false;
        gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Close()
    {
        firstPersonController.GetComponent<PlayerCtr>().enabled = true;
        firstPersonController.enabled = true;
        gameObject.SetActive(false);

    }
    private void Awake()
    {
        toggle0.onValueChanged.AddListener(Togger0);
        toggle1.onValueChanged.AddListener(Togger1);
        toggle2.onValueChanged.AddListener(Togger2);
    }
    // Start is called before the first frame update
    public GameObject[] pageInfos;


    public void Togger0(bool isSelect)
    {
        
        pageInfos[0].gameObject.SetActive(isSelect);
    }
    public void Togger1(bool isSelect)
    {
        pageInfos[1].gameObject.SetActive(isSelect);
    }
    public void Togger2(bool isSelect)
    {
        pageInfos[2].gameObject.SetActive(isSelect);
    }
}
