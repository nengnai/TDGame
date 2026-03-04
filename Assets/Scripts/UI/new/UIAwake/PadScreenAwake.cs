using UnityEngine;

public class PadScreenAwake : MonoBehaviour
{
    public RectTransform PadScreenUI;
    public CanvasGroup canvasGroup;
    void Awake()
    {
        PadScreenUI.localScale = new Vector3(3f, 3f, 1f);
    }
}
