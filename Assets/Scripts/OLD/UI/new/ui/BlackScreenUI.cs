using UnityEngine;
using System.Collections;

public class BlackScreenUI : MonoBehaviour
{
    public CanvasGroup BlackScreen;

    public IEnumerator FadeIn(float FadeTime, float endAlpha)
    {
        float startAlpha = 0f;
        float time = 0f;
        while(time < FadeTime)
        {
            BlackScreen.alpha = Mathf.Lerp(startAlpha, endAlpha, time / FadeTime);
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        BlackScreen.alpha = endAlpha;
        BlackScreen.interactable = true;
        BlackScreen.blocksRaycasts = true;

    }
    //从无到有

    public IEnumerator FadeOut(float FadeTime, float endAlpha)
    {
        float startAlpha = 1f;
        float time = 0f;
        while(time < FadeTime)
        {
            BlackScreen.alpha = Mathf.Lerp(startAlpha, endAlpha, time / FadeTime);
            time += Time.unscaledDeltaTime;
            yield return null;

        }
        BlackScreen.alpha = endAlpha;
        BlackScreen.interactable = false;
        BlackScreen.blocksRaycasts = false;
    }
    //从有到无

    public void TurnBlack()
    {
        BlackScreen.alpha = 1;
    }
    //瞬间出现

    public void TurnWhite()
    {
        BlackScreen.alpha = 0;
    }
    //瞬间消失

    public void canTouch(bool canTouch)
    {
        if(canTouch == true)
        {
            BlackScreen.interactable = true;
            BlackScreen.blocksRaycasts = true;
        }
        else
        {
            BlackScreen.interactable = false;
            BlackScreen.blocksRaycasts = false;
        }
    }
    //true时会阻挡操作
}
