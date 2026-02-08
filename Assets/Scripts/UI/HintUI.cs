using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class HintUI : MonoBehaviour
{
    [Header("关联面板")]
    public RectTransform panel;

    [Header("参数设置")]
    public float 滑动动画时长;
    public float 隐藏时x坐标;
    bool 是否显示 = false;
    Vector2 显示时位置;
    Vector2 隐藏时位置;
    Coroutine slideCoroutine;

    void OnEnable()
    {
        纠正坐标();
    }

    void OnRectTransformDimensionsChange()
    {
        纠正坐标();
    }

    void 纠正坐标()
    {
        Vector2 yLock = panel.anchoredPosition;
        显示时位置 = new Vector2(0f, yLock.y);
        隐藏时位置 = new Vector2(隐藏时x坐标, yLock.y);

        panel.anchoredPosition = 是否显示 ? 显示时位置 : 隐藏时位置;
    }

    public void 触发()
    {
        是否显示 = !是否显示;
        Vector2 目标位置 = 是否显示 ? 显示时位置 : 隐藏时位置;

        if(slideCoroutine != null)
        StopCoroutine(slideCoroutine);

        slideCoroutine = StartCoroutine(滑向动画(目标位置));
    
    }

    IEnumerator 滑向动画(Vector2 目标位置)
    {
        Vector2 起始位置 = panel.anchoredPosition;
        float elapsed = 0f;

        while(elapsed < 滑动动画时长)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 滑动动画时长;
            float easedT = 1f - Mathf.Pow(1f - t, 3f);
            
            panel.anchoredPosition = Vector2.Lerp(起始位置, 目标位置, easedT);
            yield return null;
        }

        panel.anchoredPosition = 目标位置;
        slideCoroutine = null;

    }
}
