using UnityEngine;

public class TowerPre1 : MonoBehaviour
{
    public Camera cam;
    public LayerMask 检测层级;
    public float 平滑跟随速度;
    
    private GameObject 当前预览;
    private GameObject 当前选中;


    private enum 当前状态
    {
        待机,
        预览
    }

    private 当前状态 currentState = 当前状态.待机;


    void Update()
    {

        switch (currentState)
        {
            case 当前状态.待机:
            break;

            case 当前状态.预览:
            进行预览();
            break;
        }

        if (Input.GetMouseButtonDown(1))
        {
            取消预览();
        }


    }

    public void 选中塔(GameObject 预制体)
    {
        if(currentState == 当前状态.预览 && 当前选中 == 预制体)
        {
            取消预览();
            return;
        }

        进入预览(预制体);

    }

    void 进入预览(GameObject 预制体)
    {
        取消预览();

        当前选中 = 预制体;
        当前预览 = Instantiate(预制体);

        currentState = 当前状态.预览;
    }


    void 进行预览()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 100f, 检测层级))
        {
            Vector3 targetPos = hit.point;

            当前预览.transform.position = Vector3.Lerp(当前预览.transform.position, targetPos, 平滑跟随速度 * Time.deltaTime);
        }
    }


    void 取消预览()
    {
        if(当前预览 != null)
        {
            Destroy(当前预览);
        }

        当前预览 = null;
        当前选中 = null;
        currentState = 当前状态.待机;
    }



}
