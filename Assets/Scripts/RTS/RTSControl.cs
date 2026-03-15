using UnityEngine;
using UnityEngine.AI;

public class RTSControl : MonoBehaviour
{
    public CameraRay cameraRay;
    float maxDistance;
    LayerMask layerMask;


    void Awake()
    {
        maxDistance = cameraRay.maxDistance;
        layerMask = cameraRay.layerMask;
    }
    public bool TryGetRaycastHit(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit, maxDistance, layerMask);
    }
    //返回会把射中的物体各项数据传回参数里 其他地方调用这个的时候会直接获得所有命中物体的参数
    

    public void SwitchtheRing(CharacterState targetCharacter, bool switcher)
    {
        if(targetCharacter == null) return;
        if(targetCharacter.Ring != null)
        {
            targetCharacter.Ring.SetActive(switcher);
        }
        else
        {
            Debug.Log("缺少选中光圈特效");
        }
    }
    //开关选中光圈

    public void SwitchControlable(CharacterState targetCharacter, bool switcher)
    {
        if(targetCharacter == null) return;
        targetCharacter.isSelected = switcher;
    }
    //开关是否被选中

    public void MoveCommand(NavMeshAgent agent, CharacterState targetCharacter, Vector3 Location)
    {
        if(targetCharacter == null || !targetCharacter.isSelected) return;
        agent.isStopped = false;
        agent.SetDestination(Location);               //选择目的地，让navmesh的AI去做移动
    }
    //移动指令











}
