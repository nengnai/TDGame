
using System.Collections;
using UnityEngine;

public class IngameUINEW : MonoBehaviour
{

    public AnimationCurve moveCurve1 = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f); 
    public AnimationCurve moveCurve2 = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    //小UI进出用这俩 大UI不需要 就用默认自带的函数即可

    
    [Header("下方UI主控")]
    public RectTransform BottomMain;
    public float mainMovetime;  //下方主面板动画时长

    [Header("左下角UI")]

    public RectTransform SkillSet;
    public RectTransform TowerPlace;
    public RectTransform PlaceHolder;
    public RectTransform SettingButton;
    public float LBMovetime;  //左下角UI动画时长



    [Header("右下角UI")]

    public RectTransform Student_1;
    public RectTransform Student_2;
    public RectTransform Student_3;
    public RectTransform Student_4;
    public RectTransform Student_5;
    public RectTransform Student_6;
    public float RBMovetime; //右下角UI动画时长


    [Header("顶部UI")]

    public RectTransform TopMain;
    public float TopMovetime;


    [Header("右上角UI")]

    public RectTransform Credit;
    public RectTransform Pyrox;
    public RectTransform Eligma;

    public float RTMovetime;



    public void BeginingUIforButton()
    {
        StartCoroutine(BeginningShowup());
    }


    public void hideUI()
    {
        StartCoroutine(UIHide());
    }





    public IEnumerator BeginingUI()
    {
        StartCoroutine(BeginningShowup());
        yield return null;
    }









    //主UI y：-150 → -15
    //子左下UI y：-100 → 67
    //子右下UI y：-100 → 107
    public void ResetAllUI()
    {
        BottomMain.anchoredPosition = new Vector2(BottomMain.anchoredPosition.x, -150);

        TopMain.anchoredPosition = new Vector2(TopMain.anchoredPosition.x, 100);


        SkillSet.anchoredPosition = new Vector2(SkillSet.anchoredPosition.x, -100);
        TowerPlace.anchoredPosition = new Vector2(TowerPlace.anchoredPosition.x, -100);
        PlaceHolder.anchoredPosition = new Vector2(PlaceHolder.anchoredPosition.x, -100);
        SettingButton.anchoredPosition = new Vector2(SettingButton.anchoredPosition.x, -100);


        Student_1.anchoredPosition = new Vector2(Student_1.anchoredPosition.x, -100);
        Student_2.anchoredPosition = new Vector2(Student_2.anchoredPosition.x, -100);
        Student_3.anchoredPosition = new Vector2(Student_3.anchoredPosition.x, -100);
        Student_4.anchoredPosition = new Vector2(Student_4.anchoredPosition.x, -100);
        Student_5.anchoredPosition = new Vector2(Student_5.anchoredPosition.x, -100);
        Student_6.anchoredPosition = new Vector2(Student_6.anchoredPosition.x, -100);


        Credit.anchoredPosition = new Vector2(50, Credit.anchoredPosition.y);
        Pyrox.anchoredPosition = new Vector2(50, Pyrox.anchoredPosition.y);
        Eligma.anchoredPosition = new Vector2(50, Eligma.anchoredPosition.y);

    }



    #region UI行为预制体

    public IEnumerator BeginningShowup()
    {
        ResetAllUI();
        //可加等待时间
        StartCoroutine(BottomMainSwitch(BottomMain, true));
        //
        StartCoroutine(TopMainSwitch(TopMain, true));
        //测试用 可删↑
        yield return new WaitForSecondsRealtime(1);
        yield return StartCoroutine(BottomUIshowup());
    }
    //主UI进入画面

    public IEnumerator UIHide()
    {
        yield return StartCoroutine(BottomUIhide());
        StartCoroutine(TopMainSwitch(TopMain, false));
        yield return StartCoroutine(BottomMainSwitch(BottomMain, false));
    }





    IEnumerator BottomUIshowup()
    {
        StartCoroutine(LBUIshowup());
        StartCoroutine(RTUIshowup());
        yield return StartCoroutine(RBUIshowup());
    }

    //次级UI出现

    IEnumerator BottomUIhide()
    {
        StartCoroutine(LBUIhide());
        StartCoroutine(RTUIhide());
        yield return StartCoroutine(RBUIhide());
    }






    //===============================
    //左下UI
    IEnumerator LBUIshowup()
    {
        StartCoroutine(LBButtonSwitch(SkillSet, true));
        yield return new WaitForSecondsRealtime(0.1f);
        StartCoroutine(LBButtonSwitch(TowerPlace, true));
        yield return new WaitForSecondsRealtime(0.1f);
        StartCoroutine(LBButtonSwitch(PlaceHolder, true));
        yield return new WaitForSecondsRealtime(0.1f);
        StartCoroutine(LBButtonSwitch(SettingButton, true));
    }
    

    IEnumerator LBUIhide()
    {
        StartCoroutine(LBButtonSwitch(SettingButton, false));
        yield return new WaitForSecondsRealtime(0.1f);
        StartCoroutine(LBButtonSwitch(PlaceHolder, false));
        yield return new WaitForSecondsRealtime(0.1f);
        StartCoroutine(LBButtonSwitch(TowerPlace, false));
        yield return new WaitForSecondsRealtime(0.1f);
        StartCoroutine(LBButtonSwitch(SkillSet, false));
    }

    //===============================




    //==============================
    //右下UI


    IEnumerator RBUIshowup()
    {
        StartCoroutine(RBButtonSwitch(Student_6, true));
        yield return new WaitForSecondsRealtime(0.06f);
        StartCoroutine(RBButtonSwitch(Student_5, true));
        yield return new WaitForSecondsRealtime(0.06f);
        StartCoroutine(RBButtonSwitch(Student_4, true));
        yield return new WaitForSecondsRealtime(0.06f);
        StartCoroutine(RBButtonSwitch(Student_3, true));
        yield return new WaitForSecondsRealtime(0.06f);
        StartCoroutine(RBButtonSwitch(Student_2, true));
        yield return new WaitForSecondsRealtime(0.06f);
        StartCoroutine(RBButtonSwitch(Student_1, true));
    }
    

    IEnumerator RBUIhide()
    {
        StartCoroutine(RBButtonSwitch(Student_1, false));
        yield return new WaitForSecondsRealtime(0.06f);
        StartCoroutine(RBButtonSwitch(Student_2, false));
        yield return new WaitForSecondsRealtime(0.06f);
        StartCoroutine(RBButtonSwitch(Student_3, false));
        yield return new WaitForSecondsRealtime(0.06f);
        StartCoroutine(RBButtonSwitch(Student_4, false));
        yield return new WaitForSecondsRealtime(0.06f);
        StartCoroutine(RBButtonSwitch(Student_5, false));
        yield return new WaitForSecondsRealtime(0.06f);
        StartCoroutine(RBButtonSwitch(Student_6, false));
    }


    //================================




    //================================
    //右上角资源UI出现
    IEnumerator RTUIshowup()
    {
        StartCoroutine(ResourceSwitch(Credit, true));
        yield return new WaitForSecondsRealtime(0.15f);
        StartCoroutine(ResourceSwitch(Pyrox, true));
        yield return new WaitForSecondsRealtime(0.15f);
        StartCoroutine(ResourceSwitch(Eligma, true));
    }
    
    IEnumerator RTUIhide()
    {
        StartCoroutine(ResourceSwitch(Eligma, false));
        yield return new WaitForSecondsRealtime(0.15f);
        StartCoroutine(ResourceSwitch(Pyrox, false));
        yield return new WaitForSecondsRealtime(0.15f);
        StartCoroutine(ResourceSwitch(Credit, false));
    }

    //===============================












    #endregion



















    #region 下方UI主控件

    IEnumerator BottomMainSwitch(RectTransform rectTransform, bool isOn)
    {
        Vector2 startPos = rectTransform.anchoredPosition;

        float endY = isOn ? -15 : -150;
        Vector2 endPos = new(rectTransform.anchoredPosition.x, endY);

        float time = 0f;
        while(time < mainMovetime)
        {
            time += Time.unscaledDeltaTime;
            float t = time / mainMovetime;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, smoothT);
            yield return null;
        }

        rectTransform.anchoredPosition = endPos;

    }
    //主控件UI为大型UI 不需要做自定义函数动画

    #endregion








    #region 左下角UI

    IEnumerator LBButtonSwitch(RectTransform rectTransform, bool isOn)
    {
        Vector2 startPos = rectTransform.anchoredPosition;

        float endY = isOn ? 67 : -100;
        Vector2 endPos = new(rectTransform.anchoredPosition.x, endY);

        AnimationCurve currentCurve = isOn ? moveCurve1 : moveCurve2;

        float time = 0f;
        while(time < LBMovetime)
        {
            time += Time.unscaledDeltaTime;
            float t = time / LBMovetime;
            float curveT = currentCurve.Evaluate(t);
            rectTransform.anchoredPosition = Vector2.LerpUnclamped(startPos, endPos, curveT);   //lerpunclamped不会受到函数动画图表内当超过0或1时候的限制
            yield return null;
        }

        rectTransform.anchoredPosition = endPos;   //防止偏差 最后强制位移一下
    }

    #endregion










    #region 右下角UI

    //子右下UI y：-100 → 107

    IEnumerator RBButtonSwitch(RectTransform rectTransform, bool isOn)
    {
        Vector2 startPos = rectTransform.anchoredPosition;

        float endY = isOn ? 107 : -100;
        Vector2 endPos = new(rectTransform.anchoredPosition.x, endY);

        AnimationCurve currentCurve = isOn ? moveCurve1 : moveCurve2;

        float time = 0f;
        while(time < RBMovetime)
        {
            time += Time.unscaledDeltaTime;
            float t = time / RBMovetime;
            float curveT = currentCurve.Evaluate(t);
            rectTransform.anchoredPosition = Vector2.LerpUnclamped(startPos, endPos, curveT);
            yield return null;
        }

        rectTransform.anchoredPosition = endPos;
    }


    #endregion







    // 50 → -150

    #region 右上资源显示栏
    IEnumerator ResourceSwitch(RectTransform rectTransform, bool isOn)
    {
        Vector2 startPos = rectTransform.anchoredPosition;

        float endX = isOn ? -150 : 50;
        Vector2 endPos = new(endX, rectTransform.anchoredPosition.y);

        float time = 0f;
        while(time < RTMovetime)
        {
            time += Time.unscaledDeltaTime;
            float t = time / RTMovetime;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, smoothT);
            yield return null;
        }


        rectTransform.anchoredPosition = endPos;
    }

    #endregion






    //100 → -75


    #region 顶部UI

    IEnumerator TopMainSwitch(RectTransform rectTransform, bool isOn)
    {
        Vector2 startPos = rectTransform.anchoredPosition;

        float endY = isOn ? -75 : 100;
        Vector2 endPos = new(rectTransform.anchoredPosition.x, endY);

        AnimationCurve currentCurve = isOn ? moveCurve1 : moveCurve2;

        float time = 0f;
        while(time < TopMovetime)
        {
            time += Time.unscaledDeltaTime;
            float t = time / TopMovetime;
            float curveT = currentCurve.Evaluate(t);
            rectTransform.anchoredPosition = Vector2.LerpUnclamped(startPos, endPos, curveT);
            yield return null;
        }


        rectTransform.anchoredPosition = endPos;
    }




    #endregion





}
