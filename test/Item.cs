using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 在场景里面打开模型详情页面
/// </summary>
public class Item : MonoBehaviour
{
    public Renderer[] renderers;
    public BuildType buildType;
    public Vector3 pos;
    public Vector3 angle;
    public Vector3 scale;

    public Material material;
    public bool isM;

    // 预览相关字段
    [Header("预览设置")]
    public bool isPreviewing = false; // 标记是否正在预览

    private List<GameObject> meshs = new();

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    // 开始模型预览
    public void StartModelPreview()
    {
        if (!isPreviewing)
        {
            isPreviewing = true;

            // 开始浏览模型时暂停BGM
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PauseBGM();
            }

            // 预览时可以显示轮廓高亮
            SetOutLine(true);
        }
    }

    // 结束模型预览
    public void EndModelPreview()
    {
        if (isPreviewing)
        {
            isPreviewing = false;

            // 浏览完毕后恢复BGM
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ResumeBGM();
            }

            // 结束预览时取消轮廓高亮
            SetOutLine(false);
        }
    }

    public bool isOutLine = false;
    public void SetOutLine(bool value)
    {
        if (isOutLine == value) return;
        if (value)
        {
            isOutLine = true;
            foreach (var item in renderers)
            {
                if (isM)
                {
                    var go = Instantiate(item.gameObject, item.transform.parent);
                    var r = go.GetComponent<MeshRenderer>();
                    r.GetComponent<MeshCollider>().enabled = false;
                    r.material = material;
                    meshs.Add(go.gameObject);
                    continue;
                }

                var list = new List<Material>(item.materials) { };
                list.Add(material);
                item.materials = list.ToArray();
            }
        }
        else
        {
            isOutLine = false;
            if (isM)
            {
                foreach (var item in meshs)
                {
                    DestroyImmediate(item);
                }
                meshs.Clear();
                return;
            }
            foreach (var item in renderers)
            {
                var list = new List<Material>(item.materials) { };
                list.RemoveAt(list.Count - 1);
                item.materials = list.ToArray();
            }
        }
    }

    // 如果模型被销毁时还在预览状态，恢复BGM
    private void OnDestroy()
    {
        if (isPreviewing && AudioManager.Instance != null)
        {
            AudioManager.Instance.ResumeBGM();
        }
    }
}