using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingOut : MonoBehaviour
{
    [Header("调用UI")]
    GameObject PadScreen;
    GameObject PadBackground;
    GameObject Pad;
    GameObject ButtonUI;

    public GameObject BlackScreen;
    public CanvasGroup canvasGroup;
    //游戏开始时的黑屏淡出
    [Header("调用脚本")]
    public AronaButton aronaButton;
    public BlackScreenUI blackScreenUI;
    public PadbackgroundControl padbackgroundControl;
    public PadscreenControl padscreenControl;
    public PadInsideMove padInsideMove;
    public ingameUI ingameUI;

    public void SetPadScreenUI(GameObject UI)
    {
        PadScreen = UI;
    }

    public void SetPadBackgroundUI(GameObject UI)
    {
        PadBackground = UI;
    }

    public void SetPadUI(GameObject UI)
    {
        Pad = UI;
    }

    public void SetButtonUI(GameObject UI)
    {
        ButtonUI = UI;
        Button ButtonClick = ButtonUI.GetComponentInChildren<Button>();
        //抓住物体里需要的子物体
        ButtonClick.onClick.AddListener(LoadoutScreen);
        //加入监听
    }
    //上面四个是从LoadingIn传递来的UI Object

    public void LoadoutScreen()
    {

        blackScreenUI.TurnBlack();
        blackScreenUI.canTouch(true);
        //瞬间让黑屏出现在后面防止穿帮，同时黑屏遮挡视野和操作
        StartCoroutine(StartLoadout());
        //开始退出加载页面
    }

    IEnumerator StartLoadout()
    {
        yield return ButtonOut();
        //按钮消失
        ingameUI.ResetBattleUI();
        //重置对局内游戏UI
        yield return StartCoroutine(LoadFadeOut());
        //UI三大件消失
        yield return new WaitForSecondsRealtime(0.1f);
        Destroy(Pad);
        Destroy(PadScreen);
        Destroy(PadBackground);
        Destroy(ButtonUI);
        //摧毁平板三UI和按钮UI
        yield return StartCoroutine(blackScreenUI.FadeOut(1f, 0f)); //开启黑屏
        //加过场动画 如果有的话
        yield return new WaitForSecondsRealtime(1f);
        yield return StartCoroutine(ingameUI.uiHiddenShow(1f));
        yield return StartCoroutine(ingameUI.towerPanelUIShow(0.5f));
        //游戏内主UI初始化后出现
    }

    IEnumerator ButtonOut()
    {
        CanvasGroup ButtonUI_1 = ButtonUI.GetComponent<CanvasGroup>();
        RectTransform ButtonUI_2 = ButtonUI.GetComponent<RectTransform>();

        StartCoroutine(aronaButton.ButtonOut(ButtonUI_2, ButtonUI_1, 0.2f));
        yield return null;
    }
    //按钮做消失动画

    IEnumerator LoadFadeOut()
    {
        CanvasGroup PadScreen2 = PadScreen.GetComponent<CanvasGroup>();
        RectTransform PadBackground2 = PadBackground.GetComponent<RectTransform>();
        RectTransform Pad2 = Pad.GetComponent<RectTransform>();

        IEnumerator A = padscreenControl.ScaleOut(Pad2, 3f, 3f);
        IEnumerator B = padbackgroundControl.ScaleOut(PadBackground2, 3f, 3f);
        IEnumerator C = padInsideMove.FadeOut(PadScreen2, 0.5f, 0f);

        yield return StartCoroutine(WaitAllCoroutines(A, B, C));
    }
    //平板UI三要素做消失动画




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
