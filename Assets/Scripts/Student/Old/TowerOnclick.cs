using Unity.Mathematics;
using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    public Camera cam;        // 摄像机
    public LayerMask Building;  // 只检测地面层（可选）
    float HalfHeight;


    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, Building))
        {
            transform.position = hit.point;
        }
    }
}