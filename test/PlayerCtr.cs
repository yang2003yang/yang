using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制玩家选择场景里面的模型
/// </summary>
public class PlayerCtr : MonoBehaviour
{
    private Camera mainCamea;

    public Item item;
    private void Awake()
    {
        mainCamea = Camera.main;
    }
    // Update is called once per frame
    void Update()
    {
        var ray = mainCamea.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out var hit, 200, LayerMask.GetMask("Build")))
        {
            var hitItem= hit.collider.gameObject.GetComponentInParent<Item>();
            if (item != hitItem)
            {
                //if (item != null)
                //{
                //    item.SetOutLine(false);
                //}
                item = hitItem;
                if (item != null)
                {
                    item.SetOutLine(true);
                }
            }
        }
        else
        {
            if (item != null)
            {
                item.SetOutLine(false);
                item = null;
            }
        }

        if (item!=null&&Input.GetMouseButtonDown(0))
        {
            UImanager.Instance.showPage.Open(item);
        }
    }
}
