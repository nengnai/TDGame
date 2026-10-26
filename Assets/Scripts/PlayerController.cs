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
        选中的单位 = new();
        选中的建筑 = new();
        选中的主动技能 = new();
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
            RightClick();
        }

        
    }


    void LeftClick(bool HasHit)
    {
        if(!HasHit) return;
        if(HitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Student"))
        {   
            if(选中的单位.Count > 0)                              //如果手里有选中的单位 取消选中
            {   
                foreach(RaycastHit Hit in 选中的单位)
                {
                    CharacterStats 现有的单位 = Hit.collider.GetComponentInChildren<CharacterStats>();
                    现有的单位.IsSelected = false;
                    Debug.Log("取消选中当前选中的单位");
                }
                选中的单位.Clear();
            }
            CharacterStats TargetStats = HitInfo.collider.GetComponentInChildren<CharacterStats>();
            AbilitySystemComponent ASC = HitInfo.collider.GetComponentInChildren<AbilitySystemComponent>();
            if(ASC.Tags.HasTagExact(TDGameTags.IsPlayer))          //选中自己的单位或友军的单位 友军单位选中后不可进行指令（不知道这样写行不行 还是说也没必要写
            {
                选中的单位.Add(HitInfo);
                TargetStats.IsSelected = true;
            }
            else if (ASC.Tags.HasTagExact(TDGameTags.IsAlly))
            {
                TargetStats.IsSelected = true;
            }
            Debug.Log("选中了单位");
        }
        else if(HitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Building"))
        {
            if(选中的单位.Count > 0)                              //如果手里有选中的单位 取消选中
            {   
                foreach(RaycastHit Hit in 选中的单位)
                {
                    CharacterStats 现有的单位 = Hit.collider.GetComponentInChildren<CharacterStats>();
                    现有的单位.IsSelected = false;
                    Debug.Log("取消选中当前选中的单位");
                }
                选中的单位.Clear();
            }
            //选中建筑后 如果建筑是功能性建筑（非红警那种平民建筑 建筑不可进驻驻守）则选中并且出对应UI什么的
        }
        else if(HitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if(选中的单位.Count > 0)        //如果有选中的单位 则命令单位前往地点
            {
                foreach(RaycastHit Hit in 选中的单位)
                {
                    AbilitySystemComponent ASC = Hit.collider.GetComponentInChildren<AbilitySystemComponent>();
                    CharacterStats TargetStats = Hit.collider.GetComponentInChildren<CharacterStats>();
                    if (!ASC.Tags.HasTagExact(TDGameTags.IsPlayer))                    //如果被选中的单位是敌军或友军 则取消选中 否则进入寻路
                    {
                        TargetStats.IsSelected = false;
                        continue;
                    }

                    角色控制器 控制器 = Hit.collider.GetComponentInChildren<角色控制器>();
                    控制器.移动目标地点 = HitInfo.point;
                }
            }
            else if(选中的建筑.Count > 0)
            {
                选中的建筑.Clear();
            }
            else if(选中的主动技能.Count > 0)
            {
                //释放技能
            }
        }
    }


    void RightClick()
    {
        if(选中的单位.Count > 0)                              //如果有选中的单位 取消选中
        {
            foreach(RaycastHit Hit in 选中的单位)
            {
                CharacterStats 现有的单位 = Hit.collider.GetComponentInChildren<CharacterStats>();
                现有的单位.IsSelected = false;
            }
            选中的单位.Clear();
            Debug.Log("取消选中当前选中的单位");
        }
        else if(选中的建筑.Count > 0)
        {
            
        }
        else if(选中的主动技能.Count > 0)
        {
            
        }
        
    }



}
