using Pathfinding;
using UnityEngine;

public class 角色控制器 : MonoBehaviour
{
    
    CharacterStats MyStats;
    CharacterStats MyTarget;
    SearchingTarget SearchingTarget;
    AbilitySystemComponent ASC;
    FollowerEntity 寻路系统;
    GA_BaseAttack 普攻实例;


    float 目标距离;
    public Vector3 移动目标地点;
    private Vector3 上次移动地点;


    FAbilityHandle 普攻;



    public float ScanDur;
    float LastScan;



    void Awake()
    {
        MyStats = GetComponent<CharacterStats>();
        SearchingTarget = GetComponent<SearchingTarget>();
        ASC = GetComponent<AbilitySystemComponent>();
        寻路系统 = GetComponentInParent<FollowerEntity>();

        普攻 = ASC.AddAbility(MyStats.NormalAttack);
        普攻实例 = ASC.GetLiveAbility(普攻) as GA_BaseAttack;
    }


    void Update()
    {
        if(MyStats.IsDead) return;


        if (ASC.Tags.HasTag(TDGameTags.IsStun))
        {
            寻路系统.SetPath(null);
            停止移动();
            停止攻击();
            return;
        }


        //主动/被动技能检测和释放
        


        if(上次移动地点 != 移动目标地点)
        {
            MyTarget = null;
            开始移动();
            寻路系统.SetDestination(移动目标地点);
            上次移动地点 = 移动目标地点;
        }

        if(MyTarget == null && !寻路系统.hasPath)
        {
            if(Time.time - LastScan >= ScanDur)
            {
                LastScan = Time.time;
                MyTarget = SearchingTarget.ScanWithWeapon(普攻实例);
            }
            
        }


        if (寻路系统.hasPath)
        {
            停止攻击();
            if(!ASC.Tags.HasTagExact(TDGameTags.IsMove))  ASC.Tags.AddTag(TDGameTags.IsMove);

            if (寻路系统.reachedDestination && 寻路系统.hasPath && 上次移动地点 == 移动目标地点)
            {
                寻路系统.SetPath(null);
                停止移动();
            }
        }






        if(MyTarget != null && !ASC.Tags.HasTagExact(TDGameTags.IsMove))
        {
            目标距离 = Vector3.Distance(MyStats.transform.position, MyTarget.transform.position);
            if(目标距离 > MyStats.ShootRange)
            {
                开始移动();
                寻路系统.SetDestination(MyTarget.transform.position);
            }
            else                            //记得加减攻击tag
            {
                if (!寻路系统.reachedDestination)
                {                   
                    寻路系统.SetPath(null);
                    停止移动();
                    开始攻击();
                }
            }
        }


    }





    void 停止移动()
    {
        var policy = 寻路系统.autoRepath;
        policy.mode = AutoRepathPolicy.Mode.Never;
        寻路系统.autoRepath = policy;
        ASC.Tags.RemoveTag(TDGameTags.IsMove);
    }


    void 开始移动()
    {
        var policy = 寻路系统.autoRepath;
        policy.mode = AutoRepathPolicy.Mode.EveryNSeconds;
        寻路系统.autoRepath = policy;
        ASC.Tags.AddTag(TDGameTags.IsMove);
    }


    void 停止攻击()
    {
        ASC.EndAbility(普攻);
        ASC.Tags.RemoveTag(TDGameTags.IsAttack);
    }

    void 开始攻击()
    {
        ASC.ActivateAbility(普攻);
        ASC.Tags.AddTag(TDGameTags.IsAttack);
    }

}
