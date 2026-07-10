using UnityEngine;

public class PadInsideAwake : MonoBehaviour
{
    public RectTransform PadInsideUI;
    public CanvasGroup canvasGroup;
    public float moveTime;
    public float moveSpeed;
    Vector2 originalPos;
    void Awake()
    {
        canvasGroup.alpha = 0f;
        originalPos = PadInsideUI.anchoredPosition;
    }

    void Update()
    {
        float time = 0;
        time += Time.unscaledDeltaTime;
        PadInsideUI.anchoredPosition += new Vector2(moveSpeed * Time.unscaledDeltaTime, 0);
        if(time >= moveTime)
        {
            time = 0f;
            PadInsideUI.anchoredPosition = originalPos;
        }

    }
}
