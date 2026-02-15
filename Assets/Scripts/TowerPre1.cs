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
            预览中();
            //投影持续跟随鼠标
                if (Input.GetMouseButtonDown(1))
                {
                    进入待机模式(true);
                }
                //右键取消投影
            break;
        }
    }

    public void 选中塔(Toggle toggle, GameObject 预制体)
    {
        if (!toggle.isOn)
        //点击“之后”按钮的状态
        {
            if(当前激活的Toggle == toggle)
            //点击的按钮是上一次按的同一个按钮
            {
                进入待机模式(false);
            }
            return;
        }
        //当点击之后按钮是关闭状态执行并直接结束整段函数
        //只有再次点击同一个按钮才能在检测的时候检测到关闭状态 所以理论上只要进入这个if 那么进入待机模式必定触发

        清理当前预览();
        //清掉上一个投影

        if(当前激活的Toggle != null && 当前激活的Toggle != toggle)
        //如果当前有一个按钮被选中 并且点击的按钮不是上一个被选中的按钮
        {
            当前激活的Toggle.isOn = false;
            //关闭上一个被选中的按钮
        }

        当前激活的Toggle = toggle;
        进入预览(预制体);
        currentState = 当前状态.预览;
        //把当前被选中的按钮更新成这次点击的按钮
        //生成投影
        //更新当前case状态

    }

    private void 进入预览(GameObject 预制体)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //射线检测 从摄像头射向鼠标位置

        if (Physics.Raycast(ray, out hit, 100f, 检测层级))
        //Physics.Raycast(射线, 返回碰撞信息, 射线长度, 筛选层级)
        {
            当前预览 = Instantiate(预制体, hit.point, Quaternion.identity);
            //Instantiate(生成的东西, 生成的位置, 方向)
            //如果射线碰撞到对应层级 生成投影在鼠标位置
        }
        else
        {
            当前预览 = Instantiate(预制体, new Vector3(0,-999,0), Quaternion.identity);
            //没碰到任何层级 生成在视野外某个地方
        }
    }

    private void 预览中()
    {
        if(当前预览 == null) return;
        //如果没有需要生成投影 结束函数
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
        //如果当前有按钮被选中
        {
            当前激活的Toggle.isOn = false;
            //关了按钮
        }

        当前激活的Toggle = null;
        //清空状态
        currentState = 当前状态.待机;
        //更新当前case状态
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
