using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AronaButton : MonoBehaviour
{

    public IEnumerator ButtonIn(RectTransform buttonUI, CanvasGroup canvasGroup, float MoveTime)
    {
        Vector3 startScale = buttonUI.transform.localScale;
        Vector3 endScale = new Vector3(1f, 1f, 1f);
        float time = 0f;
        while(time < MoveTime)
        {
            float t = time / MoveTime;
            float easeInT = Mathf.Pow(t, 2f);
            buttonUI.transform.localScale = Vector3.Lerp(startScale, endScale, easeInT);
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    //按钮动画出现


    public IEnumerator ButtonOut(RectTransform buttonUI, CanvasGroup canvasGroup,float MoveTime)
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;


        Vector3 startScale = buttonUI.transform.localScale;
        Vector3 endScale = new Vector3(0f, 1f, 1f);
        float time = 0f;
        while(time < MoveTime)
        {
            float t = time / MoveTime;
            float easrInT = Mathf.Pow(t, 2f);
            buttonUI.transform.localScale = Vector3.Lerp(startScale, endScale, easrInT);
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        buttonUI.transform.localScale = new Vector3(0f, 1f, 1f);
    }
    //按钮动画消失



}
