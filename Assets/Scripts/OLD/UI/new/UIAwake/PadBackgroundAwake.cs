using UnityEngine;

public class PadBackgroundAwake : MonoBehaviour
{
    public RectTransform PadBackground;
    public CanvasGroup canvasGroup;
    void Awake()
    {
        PadBackground.localScale = new Vector3(3f, 3f, 1f);
    }
}
