using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

/// <summary>
/// 显示选中的对应模型，可用鼠标旋转显示，缩放大小
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
        
        // Start model preview to pause BGM
        if (_Item != null)
        {
            _Item.StartModelPreview();
        }
    }

    public void Close()
    {
        // End model preview to resume BGM
        if (_Item != null)
        {
            _Item.EndModelPreview();
        }
        
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

        //通过鼠标右键实现转向和缩放
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
