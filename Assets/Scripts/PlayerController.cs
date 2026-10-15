using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{


    public Camera MainCam;




    private Ray MouseRay;
    private RaycastHit HitInfo;

    public LayerMask LayerMask;




    private List<RaycastHit> 选中的单位;
    private List<RaycastHit> 选中的主动技能;
    private List<RaycastHit> 选中的建筑;





    void Awake()
    {
        MainCam = Camera.main;
    }


    void Update()
    {
        MouseRay = MainCam.ScreenPointToRay(Input.mousePosition);

        bool HasHit = Physics.Raycast(MouseRay, out HitInfo, Mathf.Infinity, LayerMask);

        if (Input.GetMouseButtonDown(0))
        {
            LeftClick(HasHit);
        }

        if (Input.GetMouseButtonDown(1))
        {
            RightClick(HasHit);
        }
    }


    void LeftClick(bool HasHit)
    {
        if(!HasHit) return;
        if(HitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Student"))
        {   
            if(选中的单位.Count > 0)
            {   
                foreach(RaycastHit Hit in 选中的单位)
                {
                    CharacterStats 现有的单位 = Hit.collider.GetComponentInChildren<CharacterStats>();
                    现有的单位.IsSelected = false;
                }
                选中的单位.Clear();
            }
            CharacterStats TargetStats = HitInfo.collider.GetComponentInChildren<CharacterStats>();
            if(TargetStats.是否属于玩家)
            {
                选中的单位.Add(HitInfo);
            }
            TargetStats.IsSelected = true;

        }
        else if(HitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Building"))
        {
            
        }
        else if(HitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            
        }
    }


    void RightClick(bool HasHit)
    {
        if(选中的单位.Count > 0)
        {
            foreach(RaycastHit Hit in 选中的单位)
            {
                CharacterStats 现有的单位 = Hit.collider.GetComponentInChildren<CharacterStats>();
                现有的单位.IsSelected = false;
            }
            选中的单位.Clear();
        }
        else if(选中的建筑.Count > 0)
        {
            
        }
        else if(选中的主动技能.Count > 0)
        {
            
        }
        
    }



}
