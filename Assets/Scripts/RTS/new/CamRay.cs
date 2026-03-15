using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class CamRay : MonoBehaviour
{
    
    [SerializeField, Tooltip("射线检测筛选")] private LayerMask layerMask;
    [SerializeField, Tooltip("射线检测距离")] float maxDistance;
    int towerLayer, groundLayer, enemyLayer;




    [SerializeField] RTScontrol rtsControl;



    [SerializeField] Camera mainCamera;



    void Awake()
    {
        towerLayer = LayerMask.NameToLayer("Tower");
        groundLayer = LayerMask.NameToLayer("Ground");
        enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    bool TryGetRaycastHit(out RaycastHit hit)
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit, maxDistance, layerMask);
    }

    
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return; //检查有没有碰到UI层

        bool leftClick = Input.GetMouseButtonDown(0);
        bool rightClick = Input.GetMouseButtonDown(1);

        if(!leftClick && !rightClick) return;
        if(!TryGetRaycastHit(out RaycastHit hit) || hit.collider == null) return;
        //
        //之后这里加悬停UI的话要改一下
        //

        if (rightClick)
        {
            rtsControl.DeselectUnit();
            return;
        }
        //
        //之后要做其他右键功能就在这里
        //

        if (leftClick)
        {
            int hitLayer = hit.collider.gameObject.layer;
            if(hitLayer == towerLayer)
            {
                var unit = hit.collider.GetComponentInChildren<CharacterStat>();
                if(unit != null) rtsControl.SelectUnit(unit);
            }
            else if(hitLayer == groundLayer)
            {
                rtsControl.MoveUnit(hit.point);
            }
            else if(hitLayer == enemyLayer)
            {
                rtsControl.AttackTarget(hit.collider.gameObject);
            }


        }

    



    }

    






}
