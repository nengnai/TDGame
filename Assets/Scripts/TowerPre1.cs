using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class TowerPre1 : MonoBehaviour
{
    public Camera cam;
    public LayerMask 检测层级;
    public float 平滑跟随速度;

    private enum 当前状态
    {
        待机,
        预览
    }

    private GameObject 当前预览;
    private Toggle 当前激活的Toggle;
    private 当前状态 currentState = 当前状态.待机;


    void Update()
    {

        switch (currentState)
        {
            case 当前状态.待机:
            break;

            case 当前状态.预览:
            进行预览();
                if (Input.GetMouseButtonDown(1))
                {
                    进入待机模式(true);
                }
            break;
        }
    }

    public void 选中塔(Toggle toggle, GameObject 预制体)
    {
        if (!toggle.isOn)
        {
            if(当前激活的Toggle == toggle)
            {
                进入待机模式(false);
            }
            return;
        }

        清理当前预览();

        if(当前激活的Toggle != null && 当前激活的Toggle != toggle)
        {
            当前激活的Toggle.isOn = false;
        }

        当前激活的Toggle = toggle;
        进入预览(预制体);
        currentState = 当前状态.预览;

    }

    private void 进入预览(GameObject 预制体)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, 检测层级))
        {
            当前预览 = Instantiate(预制体, hit.point, Quaternion.identity);
        }
        else
        {
            当前预览 = Instantiate(预制体, new Vector3(0,-999,0), Quaternion.identity);
        }
    }

    private void 进行预览()
    {
        if(当前预览 == null) return;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100f, 检测层级))
        {
            Vector3 targetPos = hit.point;
            当前预览.transform.position = Vector3.Lerp(当前预览.transform.position, targetPos, 平滑跟随速度 * Time.deltaTime);
            //此处可添加放置逻辑
        }
    }


    public void 进入待机模式(bool 强制关闭UI)
    {
        清理当前预览();

        if(强制关闭UI && 当前激活的Toggle != null)
        {
            当前激活的Toggle.isOn = false;
        }

        当前激活的Toggle = null;
        currentState = 当前状态.待机;
    }

    private void 清理当前预览()
    {
        if(当前预览 != null)
        {
            Destroy(当前预览);
            当前预览 = null;
        }
    }

}
