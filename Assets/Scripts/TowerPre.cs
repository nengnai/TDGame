using UnityEngine;
using UnityEngine.Rendering;



public class TowerPre : MonoBehaviour
{
    public GameObject 当前预览;
    public Camera cam;
    public LayerMask 检测层级;

    RaycastHit hit;
    private GameObject 预览待生成;
    private GameObject 当前选中;
    public float 平滑跟随速度;

    public void 选中塔(GameObject 虚拟投影)
    {
        if(当前选中 == 虚拟投影)
        {
            if(当前预览 != null)
            {
                Destroy(当前预览);
                当前预览 = null;
                预览待生成 = null;
            }
            当前选中 = null;
            return;
        }

        当前选中 = 虚拟投影;
        预览待生成 = 虚拟投影;

        if(当前预览 != null)
        {
            Destroy(当前预览);
        }

    }

    void Update()
    {

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 100f, 检测层级))
        {
            Vector3 mousePos = hit.point;
            if(预览待生成 != null)
            {
                当前预览 = Instantiate(预览待生成, mousePos, Quaternion.identity);
                预览待生成 = null;
            }

            if(当前预览 != null)
            {
                当前预览.transform.position = Vector3.Lerp(当前预览.transform.position, mousePos, 平滑跟随速度 * Time.deltaTime);
            }
        }

        if (Input.GetMouseButtonDown(1) && 当前预览 != null)
        {
            Destroy(当前预览);
            当前预览 = null;
            预览待生成 = null;
            当前选中 = null;
        }
    }
}
