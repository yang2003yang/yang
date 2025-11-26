using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 控制开始按钮等放大和颜色改变
/// </summary>
public class TextButtonAnim : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{

    public Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.transform.DOScale(4, 0.2f);
        text.color = Color.red;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.transform.DOScale(3.33f, 0.2f);
        text.color = Color.black;
    }
}
