using UnityEngine;

public class AronaButtonAwake : MonoBehaviour
{
    public RectTransform Button;


    void Awake()
    {
        Button.transform.localScale = new Vector3(0f, 1f, 1f);
    }



}
