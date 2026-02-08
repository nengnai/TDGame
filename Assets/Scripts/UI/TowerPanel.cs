using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TowerPanel : MonoBehaviour
{
    [Header("关联面板")]
    public RectTransform panel;

    [Header("参数设置")]
    public float 滑动动画时长,半隐藏时Y坐标,完全隐藏时Y坐标;
    Vector2 显示时位置, 半隐藏时位置, 完全隐藏时位置,目标位置;
    Coroutine slideCoroutine;

    int 面板状态 = 1;

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
        Vector2 xLock = panel.anchoredPosition;

        显示时位置 = new Vector2(xLock.x,0f);
        半隐藏时位置 = new Vector2(xLock.x,半隐藏时Y坐标);
        完全隐藏时位置 = new Vector2(xLock.x,完全隐藏时Y坐标);

        switch (面板状态)
        {
            case 1 :
            panel.anchoredPosition = 显示时位置;
            break;

            case 2 :
            panel.anchoredPosition = 半隐藏时位置;
            break;

            case 3:
            panel.anchoredPosition = 完全隐藏时位置;
            break;
        }
    }

    public void 半隐藏()
    {

        if(面板状态 == 2 || 面板状态 == 3)
        {
            目标位置 = 显示时位置;
            Debug.Log("回调至显示");
        }
        else
        目标位置 = 半隐藏时位置;

        if(slideCoroutine != null)
        StopCoroutine(slideCoroutine);

        slideCoroutine = StartCoroutine(滑向动画(目标位置));
        
    }

    public void 完全隐藏()
    {

        if(面板状态 == 3)
        目标位置 = 显示时位置;
        else
        目标位置 = 完全隐藏时位置;

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

        if(目标位置 == 半隐藏时位置)
        面板状态 = 2;
        else if (目标位置 == 完全隐藏时位置)
        面板状态 = 3;
        else
        面板状态 = 1;

    }

}
