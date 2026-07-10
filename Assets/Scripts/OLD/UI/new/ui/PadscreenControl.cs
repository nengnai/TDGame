using System.Collections;
using UnityEngine;
//加载页面平板边框
public class PadscreenControl : MonoBehaviour
{

    public IEnumerator ScaleOut(RectTransform PadUI, float MoveTime, float ScaleMultiple)
    {
        Vector3 startPos = PadUI.localScale;
        Vector3 endPos = new Vector3 (ScaleMultiple, ScaleMultiple, 1f);
        float time = 0f;
        while(time < MoveTime)
        {
            float t = time / MoveTime;
            float easeInT = Mathf.Pow(t, 2f);
            PadUI.localScale = Vector3.Lerp(startPos, endPos, easeInT);
            time += Time.unscaledDeltaTime;
            yield return null;
        }
    }
    //放大（转入对局）


    public IEnumerator ScaleIn(RectTransform PadUI, float MoveTime, float ScaleMultiple)
    {
        PadUI.localScale =new Vector3 (ScaleMultiple, ScaleMultiple, 1f);
        Vector3 startPos = PadUI.localScale;
        Vector3 endPos = new Vector3 (1f, 1f, 1f);
        float time = 0f;
        while (time < MoveTime)
        {
            float t = time / MoveTime;
            float easeInT = Mathf.Pow(t, 2f);
            PadUI.localScale = Vector3.Lerp(startPos, endPos, easeInT);
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        
    }
    //缩小（从对局转出）

    public IEnumerator FadeIn(CanvasGroup canvasGroup, float FadeTime, float EndAlpha)
    {
        canvasGroup.alpha = 1;
        float startAlpha = 1;
        float time = 0f;
        while(time < FadeTime)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, EndAlpha, time / FadeTime);
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    //淡入

    public IEnumerator FadeOut(CanvasGroup canvasGroup, float FadeTime, float EndAlpha)
    {
        float startAlpha = 0;
        float time = 0f;
        while (time < FadeTime)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, EndAlpha, time / FadeTime);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    //淡出

}
