using System.Collections;
using UnityEngine;
//加载页面平板内图片淡入淡出
public class PadInsideMove : MonoBehaviour
{

    public IEnumerator FadeIn(CanvasGroup UI, float FadeTime, float endAlpha)
    {
        float startAlpha = 0f;
        float time = 0f;
        while(time < FadeTime)
        {
            UI.alpha = Mathf.Lerp(startAlpha, endAlpha, time / FadeTime);
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        UI.alpha = endAlpha;
        UI.interactable = true;
        UI.blocksRaycasts = true;

    }
    //淡入

    public IEnumerator FadeOut(CanvasGroup UI, float FadeTime, float endAlpha)
    {
        float startAlpha = 1f;
        float time = 0f;
        while(time < FadeTime)
        {
            UI.alpha = Mathf.Lerp(startAlpha, endAlpha, time / FadeTime);
            time += Time.unscaledDeltaTime;
            yield return null;

        }
        UI.alpha = endAlpha;
        UI.interactable = false;
        UI.blocksRaycasts = false;
    }
    //淡出

}
