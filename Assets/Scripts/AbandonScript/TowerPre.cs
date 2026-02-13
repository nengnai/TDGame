using UnityEngine;
using UnityEngine.Rendering;



public class TowerPre : MonoBehaviour
{
    public GameObject 当前预览;
    //当前跟随鼠标的预览投影
    //the current preview projection following the mouse cursor
    public Camera cam;
    public LayerMask 检测层级;
    //投影可以在哪些层级上面
    //the Layer that projection can be performed

    private GameObject 预览待生成;
    //临时存储目标塔
    //temporary storage the tower that gonna be selected

    private GameObject 当前选中;
    //记录选择的UI按钮
    //the UI button
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

        Ray ray = cam.ScreenPointToRay(Input.mousePosition); //从摄像机发射一道射线穿过鼠标位置
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 100f, 检测层级)) // (射线, 如果射线接触到物体, 射线长度, 检测层级)
        {
            Vector3 mousePos = hit.point;     //hit.point == 射线打中的世界坐标位置
            if(预览待生成 != null)
            {
                当前预览 = Instantiate(预览待生成, mousePos, Quaternion.identity); 
                //(要生成的预制件, 生成的位置, 角度旋转(这里的意思是不做旋转))
                //Instantiate用于只复制一次目标预制件
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
