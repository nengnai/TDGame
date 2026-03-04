using System.Collections;
using UnityEngine;
//战局内UI
public class ingameUI : MonoBehaviour
{
    [Header("右下角主UI和塔Panel主UI")]
    public RectTransform UI;
    public RectTransform towerPanelUI;
    public RectTransform towerPanelButton;
    public float UIMoveTime;
    public float towerPanelUIMoveTime;
    bool isUIMoved = true;
    //true时为UI进入视角内
    bool isUIMoving = false;
    bool istowerPanelUIMoved = true;
    //同上
    bool istowerPanelUIMoving = false;
    Vector2 targetPos1;
    Vector2 targetPos2;
    Vector3 targetAngle;
    Vector2 UItargetPos;
    Vector2 towerPanelUItargetPos;
    Vector3 towerPanelButtonStartAngle;
    Vector2 ResetPos;
    

    //开局确定所有UI的初始点和最终移动点

    void Start()
    {
        UItargetPos = UI.anchoredPosition;
        targetPos1 = UItargetPos + new Vector2(0, -376);
        towerPanelUItargetPos = towerPanelUI.anchoredPosition;
        targetPos2 = towerPanelUItargetPos + new Vector2(718, 0);
        towerPanelButtonStartAngle = towerPanelButton.localEulerAngles;
        targetAngle = new Vector3(0f, 0f, 270f);
    }
    public void ResetBattleUI()
    {
        if (isUIMoved)
        {
            UI.anchoredPosition = UItargetPos + new Vector2(0, -476);
        }
        if (istowerPanelUIMoved)
        {
            towerPanelUI.anchoredPosition = towerPanelUItargetPos + new Vector2(718, 0);
            towerPanelButton.localEulerAngles = towerPanelButtonStartAngle;
        }
    }
    //将右下角UI和防御塔主UI【瞬间】隐藏

    public void UIAnimation()
    {
        if(isUIMoving) return;
        if (isUIMoved)
        {
            StartCoroutine(UISwitch(targetPos1));
            isUIMoved = false;
        }
        else
        {
            StartCoroutine(UISwitch(UItargetPos));
            isUIMoved = true;
        }
    }
    //右下角UI动画主控

    public void towerPanelAnimation()
    {
        if(istowerPanelUIMoving) return;
        if (istowerPanelUIMoved)
        {
            StartCoroutine(towerUISwitch(targetPos2));
            StartCoroutine(towerPanelButtonSwitch(targetAngle));
            istowerPanelUIMoved = false;
        }
        else
        {
            StartCoroutine(towerUISwitch(towerPanelUItargetPos));
            StartCoroutine(towerPanelButtonSwitch(towerPanelButtonStartAngle));
            istowerPanelUIMoved = true;
        }
    }
    //防御塔主UI动画主控

    IEnumerator UISwitch(Vector2 targetPos)
    {
        isUIMoving = true;
        Vector2 startPos = UI.anchoredPosition;
        float time = 0f;
        while (time < UIMoveTime)
        {
            time += Time.unscaledDeltaTime;
            float t = time / UIMoveTime;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            UI.anchoredPosition = Vector2.Lerp(startPos, targetPos, smoothT);
            yield return null;
        }
        UI.anchoredPosition = targetPos;
        isUIMoving = false;
    }
    //右下角UI动画进出

    IEnumerator towerUISwitch(Vector2 targetPos)
    {
        istowerPanelUIMoving = true;
        Vector2 startPos = towerPanelUI.anchoredPosition;
        float time = 0f;
        while(time < towerPanelUIMoveTime)
        {
            time += Time.unscaledDeltaTime;
            float t = time / towerPanelUIMoveTime;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            towerPanelUI.anchoredPosition = Vector2.Lerp(startPos, targetPos, smoothT);
            yield return null;
        }
        towerPanelUI.anchoredPosition = targetPos;
        istowerPanelUIMoving = false;
    }
    //防御塔主UI动画进出



    public IEnumerator uiHiddenShow(float moveTime)
    {
        Vector2 startPos = UI.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, 476);

        float time = 0f;
        while(time < moveTime)
        {
            time += Time.unscaledDeltaTime;
            float t = time / moveTime;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            UI.anchoredPosition = Vector2.Lerp(startPos, endPos, smoothT);
            yield return null;
        }
    }
    //动画打开右下角UI


    public IEnumerator uiHiddenHide(float moveTime)
    {
        Vector2 startPos = UI.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, -476);

        float time = 0f;
        while(time < moveTime)
        {
            time += Time.unscaledDeltaTime;
            float t = time / moveTime;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            UI.anchoredPosition = Vector2.Lerp(startPos, endPos, smoothT);
            yield return null;
        }
    }
    //动画关闭右下角UI


    public IEnumerator towerPanelUIShow(float moveTime)
    {
        Vector2 startPos = towerPanelUI.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(-718, 0);

        float time = 0f;
        while(time < moveTime)
        {
            time += Time.unscaledDeltaTime;
            float t = time / moveTime;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            towerPanelUI.anchoredPosition = Vector2.Lerp(startPos, endPos, smoothT);
            yield return null;
        }
    }
    //动画开启防御塔主UI


    public IEnumerator towerPanelUIHide(float moveTime)
    {
        Vector2 startPos = towerPanelUI.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(718, 0);

        float time = 0f;
        while(time < moveTime)
        {
            time += Time.unscaledDeltaTime;
            float t = time / moveTime;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            towerPanelUI.anchoredPosition = Vector2.Lerp(startPos, endPos, smoothT);
            yield return null;
        }
    }
    //动画关闭防御塔主UI


    IEnumerator towerPanelButtonSwitch(Vector3 targetAngle)
    {
        Vector3 startAngle = towerPanelButton.localEulerAngles;
        float time = 0f;
        while(time < towerPanelUIMoveTime)
        {
            time += Time.unscaledDeltaTime;
            float t = time / towerPanelUIMoveTime;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            towerPanelButton.localEulerAngles = Vector3.Lerp(startAngle, targetAngle, smoothT);
            yield return null;
        }
        towerPanelButton.localEulerAngles = targetAngle;
        istowerPanelUIMoving = false;
    }
    //塔主UI按钮旋转

}
