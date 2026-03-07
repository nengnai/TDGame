using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.AI;

public class CameraRay : MonoBehaviour
{
    public float maxDistance = 500f;
    public LayerMask layerMask;
    public RTSControl rtsControl;
    GameObject SavedItem = null;
    //当前选中的物体存储 能被选中并查看信息的只有己方塔、盟友单位和敌人
    GameObject SavedItem_1 = null;
    //当前已经保存的物体存储
    NavMeshHit navHit;
    float radius = 1f;



    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            if(!rtsControl.TryGetRaycastHit(out RaycastHit hit) || hit.collider == null) return;
            //无论左右键先射一发 如果没设中任何东西就无事发生 或者射中的物体没有碰撞箱也无事发生

            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Tower")) //射中的东西是Tower层的话
            {

                SavedItem = hit.collider.gameObject;

                if (SavedItem_1 == SavedItem) //如果选中的塔跟已经被选中的东西是同一个
                {
                    CharacterState towerState = SavedItem_1.GetComponentInChildren<CharacterState>();
                    rtsControl.SwitchtheRing(towerState, false);
                    rtsControl.SwitchControlable(towerState, false);
                    SavedItem_1 = null;
                }
                else //如果选中的塔跟被选中的东西不是同一个
                {

                    if(SavedItem_1 != null) //如果之前已经选中了东西
                    {
                        CharacterState towerState_1 = SavedItem_1.GetComponentInChildren<CharacterState>();
                        rtsControl.SwitchtheRing(towerState_1, false);
                        rtsControl.SwitchControlable(towerState_1, false);
                    }

                SavedItem_1 = SavedItem;
                CharacterState towerState = SavedItem_1.GetComponentInChildren<CharacterState>();
                rtsControl.SwitchtheRing(towerState, true);
                rtsControl.SwitchControlable(towerState, true);
                //无论saveditem_1里有没有存东西 总会把新选中的塔的选中特效打开

                }                   
            }   

            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {

                if(SavedItem_1 == null || SavedItem_1.layer != LayerMask.NameToLayer("Tower"))return; //如果当时啥也没选中 或者当前已选中的东西不是塔 则无事发生
                bool canReach = NavMesh.SamplePosition(hit.point, out navHit, radius, NavMesh.AllAreas);    //检查一下那个地点能不能站
                if(canReach)
                {
                    var SavedItem_2 = SavedItem_1.GetComponentInChildren<CharacterState>();
                    var agent = SavedItem_1.GetComponent<NavMeshAgent>();
                    rtsControl.MoveCommand(agent, SavedItem_2, navHit.position);              //移动到射线射中的坐标点
                }

            }

            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Boss enemy"))
            {
                if(SavedItem_1.layer == LayerMask.NameToLayer("Tower"))
                {
                    
                }
                else
                {
                    return;
                }
            }


        }

        if (Input.GetMouseButtonDown(1))   //如果是右键
        {

            if(SavedItem_1 == null) return; //如果之前没选中任何东西则无事发生
            CharacterState towerState = SavedItem_1.GetComponentInChildren<CharacterState>();
            rtsControl.SwitchtheRing(towerState, false);
            rtsControl.SwitchControlable(towerState, false);
            SavedItem_1 = null;

        }   
    }
}

