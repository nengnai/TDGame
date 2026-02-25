using UnityEngine;
public class range : MonoBehaviour
{
    [Header("目标尺寸参数")]
    public float targetXZ;
    public float duration;
    private Vector3 startScale;
    private Vector3 endScale;
    private float elapsedTime;
    void Start()
    {
        // 记录初始缩放，X 和 Z 从 0 开始，Y 固定为当前值
        startScale = new Vector3(0f, transform.localScale.y, 0f);
        // 生成目标缩放值（Y 固定）
        endScale = new Vector3(targetXZ, transform.localScale.y, targetXZ);
        // 初始化到起始值
        transform.localScale = startScale;
    }
    void Update()
    {
        if (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            // 过渡比例
            float t = elapsedTime / duration;
            // 平滑插值（可以换成 Mathf.SmoothStep 更柔和）
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
        }
    }
}