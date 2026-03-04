using System.Collections;
using UnityEngine;

public class LoadingIn : MonoBehaviour
{
    [Header("调用UI")]
    public GameObject Pad;
    //平板边框
    public CanvasGroup canvasGroup1;
    public GameObject PadScreen;
    //平板内背景
    public CanvasGroup canvasGroup2;
    public GameObject PadBackground;
    //平板后背景
    public CanvasGroup canvasGroup3;
    public GameObject canvas;
    public GameObject ButtonUI;
    //中间的确认按钮
    public CanvasGroup canvasGroup4;
    GameObject Pad1;
    GameObject PadScreen1;
    GameObject PadBackground1;
    GameObject ButtonUI1;
    
    [Header("调用脚本")]
    public AronaButton AronaButton;
    public PadbackgroundControl padbackgroundControl;
    public PadInsideMove padInsideMove;
    public PadscreenControl padscreenControl;
    public LoadingOut LoadingOut;
    //之后要加入进度条什么的要在这里加一个调用它的位置

    public void LoadScreen()
    {
        StartCoroutine(StartLoading());
    }
    //进入加载页面环节

    IEnumerator StartLoading()
    {
        Begin1();
        //生成平板三UI
        yield return StartCoroutine(LoadFadeIn());
        //UI缩小
        yield return new WaitForSecondsRealtime(1);
        //目前是等待1秒 之后可以加入进度条什么的
        //.
        Begin2();
        //加载完后生成按钮并传递给LoadingOut脚本
        LoadingOut.enabled = true;
        //忘了这个干啥的
        StartCoroutine(ButtonIn());
        //按钮出现
    }

    void Begin1()
    {
        PadScreen1 = Instantiate(PadScreen, canvas.transform);
        PadBackground1 = Instantiate(PadBackground, canvas.transform);
        Pad1 = Instantiate(Pad, canvas.transform);
        //生成UI

        LoadingOut.SetPadScreenUI(PadScreen1);
        LoadingOut.SetPadBackgroundUI(PadBackground1);
        LoadingOut.SetPadUI(Pad1);
        //传递生成的UI给LoadingOut脚本
    }
    //生成UI
    //注意顺序 最后一个生成的将会是在最前面一层

    void Begin2()
    {
        ButtonUI1 = Instantiate(ButtonUI, canvas.transform);
        LoadingOut.SetButtonUI(ButtonUI1);
    }
    //加载完毕后生成按钮UI但还没开始做动画，同时传给LoadingOut脚本按钮UI的数据来让它能够调用正确的UI

    IEnumerator LoadFadeIn()
    {
        CanvasGroup PadScreen2 = PadScreen1.GetComponent<CanvasGroup>();
        RectTransform PadBackground2 = PadBackground1.GetComponent<RectTransform>();
        RectTransform Pad2 = Pad1.GetComponent<RectTransform>();
        //由于生成的都是预制体GameObject 所以还得用这个方法获取指定生成的预制件中对应的东西


        IEnumerator A = padscreenControl.ScaleIn(Pad2, 1.5f, 3f);
        IEnumerator B = padbackgroundControl.ScaleIn(PadBackground2, 1.5f, 3f);
        IEnumerator C = padInsideMove.FadeIn(PadScreen2, 1.5f, 1f);
        //调用之前写好的UI动画

        yield return StartCoroutine(WaitAllCoroutines(A, B, C));
        //等上面这三个协程全部播放完之后才会继续执行下面的

    }



    IEnumerator ButtonIn()
    {
        CanvasGroup ButtonUI2_1 = ButtonUI1.GetComponent<CanvasGroup>();
        RectTransform ButtonUI2_2 = ButtonUI1.GetComponent<RectTransform>();

        StartCoroutine(AronaButton.ButtonIn(ButtonUI2_2, ButtonUI2_1, 0.2f));
        yield return null;
    }
    //生成按钮后做动画




    //下面的是协程等待方法 作用是等指定几个协程全都执行完毕后才会进行下面的步骤
    //抄的。
    private IEnumerator WaitAllCoroutines(params IEnumerator[] enumerators)
    {
        bool[] finished = new bool[enumerators.Length];
        for (int i = 0; i < enumerators.Length; i++)
        {
            int index = i;
            StartCoroutine(RunAndFlag(enumerators[i], () => finished[index] = true));
        }
        yield return new WaitUntil(() => AllFinished(finished));
    }
    private IEnumerator RunAndFlag(IEnumerator routine, System.Action onDone)
    {
        yield return StartCoroutine(routine);
        onDone?.Invoke();
    }
    private bool AllFinished(bool[] flags)
    {
        foreach (bool b in flags)
        {
            if (!b) return false;
        }
        return true;
}




}
