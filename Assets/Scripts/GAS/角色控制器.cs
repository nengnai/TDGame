using Pathfinding;
using UnityEngine;

public class 角色控制器 : MonoBehaviour
{
    
    CharacterStats MyStats;
    CharacterStats MyTarget;
    SearchingTarget SearchingTarget;
    AbilitySystemComponent ASC;
    FollowerEntity 寻路系统;


    float 目标距离;
    public Vector3 移动目标地点;
    private Vector3 上次移动地点;


    FAbilityHandle 普攻;



    void Awake()
    {
        MyStats = GetComponent<CharacterStats>();
        SearchingTarget = GetComponent<SearchingTarget>();
        ASC = GetComponent<AbilitySystemComponent>();
        寻路系统 = GetComponentInParent<FollowerEntity>();

        普攻 = ASC.AddAbility(MyStats.NormalAttack);

    }


    void Update()
    {
        if(MyStats.IsDead) return;


        if (ASC.Tags.HasTag(TDGameTags.IsStun))
        {
            寻路系统.SetPath(null);
            停止移动();
            ASC.Tags.RemoveTag(TDGameTags.IsMove);

            ASC.EndAbility(普攻);
            //去掉tag


            return;
        }



        //主动/被动技能检测和释放
        if(MyTarget != null && !寻路系统.hasPath)
        {
            SearchingTarget.ScanWithWeapon(MyStats.NormalAttack);
        }






        if(上次移动地点 != 移动目标地点)
        {
            开始移动();
            寻路系统.SetDestination(移动目标地点);
            上次移动地点 = 移动目标地点;
        }


        if (寻路系统.hasPath)
        {
            ASC.EndAbility(普攻);
            //去掉tag
            if(!ASC.Tags.HasTagExact(TDGameTags.IsMove))  ASC.Tags.AddTag(TDGameTags.IsMove);


            
            if (寻路系统.reachedDestination)
            {
                寻路系统.SetPath(null);
                停止移动();
                ASC.Tags.RemoveTag(TDGameTags.IsMove);
            }
        }






        if(MyTarget != null)
        {
            目标距离 = Vector3.Distance(MyStats.transform.position, MyTarget.transform.position);
            if(目标距离 > MyStats.ShootRange)
            {
                寻路系统.SetDestination(MyTarget.transform.position);
            }
            else                            //记得加减攻击tag
            {
                if (!寻路系统.reachedDestination)
                {                   
                    寻路系统.SetPath(null);
                    停止移动();
                    ASC.ActivateAbility(普攻);
                }
            }
        }


    }





    void 停止移动()
    {
        var policy = 寻路系统.autoRepath;
        policy.mode = AutoRepathPolicy.Mode.Never;
        寻路系统.autoRepath = policy;
    }


    void 开始移动()
    {
        var policy = 寻路系统.autoRepath;
        policy.mode = AutoRepathPolicy.Mode.EveryNSeconds;
        寻路系统.autoRepath = policy;
    }


}
