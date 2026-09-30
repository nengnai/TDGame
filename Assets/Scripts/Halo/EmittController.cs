
using UnityEngine;
using Utils;

/// <summary>
/// 光环的发光变化控制器
/// </summary>
public class EmittController : MonoBehaviour
{
    [Range(0, 1)]
    public float emittScale=0.5f;
    [Range(0, 2)]
    public float speedScale=1;

    private float scale = 1;
    private MpbController mpb;

    void Start()
    {
        mpb = new(transform);
    }

    void Update()
    {
        float normalizedSin = (Mathf.Sin(Time.time * speedScale) + 1f) * 0.5f;
        scale = 1f + (emittScale * normalizedSin);
        mpb.Set("_EmissionScale", scale).Apply();
    }

}
