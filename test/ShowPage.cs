using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

/// <summary>
/// 打开选择的对应模型，可以控制旋转缩放，显示详情
/// </summary>
public class ShowPage : MonoBehaviour
{
    public FirstPersonController firstPersonController;
    public RectTransform rect_Prefab;
    public Text txt_Name;
    public Text txt_Desc;

    private Item _Item;
    public GameObject go;

    public List<string> desc;

    private Vector3 mouse;
    public void Open(Item item)
    {
        txt_Name.text = item.buildType.ToString();
        txt_Desc.text = desc[(int)item.buildType];
       
        firstPersonController.enabled = false;
        firstPersonController.GetComponent<PlayerCtr>().enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        isDown = false; 
        item.SetOutLine(false);
        if (go != null)
        {
            Destroy(go);
        }
        rect_Prefab.transform.localScale = Vector3.one;
        rect_Prefab.localEulerAngles = Vector3.zero;
        _Item = item;
        gameObject.SetActive(true);
        go= Instantiate(item.gameObject, rect_Prefab);
        go.transform.localPosition = _Item.pos;
        go.transform.localScale = _Item.scale*2;
        go.transform.localEulerAngles = _Item.angle;
       var rs= go.GetComponent<Item>().renderers;
        foreach (var r in rs)
        {
            r.gameObject.layer = 5;
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
        firstPersonController.GetComponent<PlayerCtr>().enabled = true;
        firstPersonController.enabled = true;
    }
    bool isDown = false;
    private void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            mouse = Input.mousePosition;
            isDown = true;
        }

        //设置模型的旋转和大小
        if (Input.GetMouseButton(1))
        {
            if (!isDown) return;
            var dir = Input.mousePosition - mouse;
            mouse = Input.mousePosition;
            rect_Prefab.Rotate(new Vector3(dir.y,-dir.x) * Time.deltaTime * 6, Space.World);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        rect_Prefab.transform.localScale += Vector3.one* scroll * 10 * Time.deltaTime;
    }

    public void ClickAudio()
    {
        AudioManager.Instance.PlayAudio(_Item.buildType);
    }
}
