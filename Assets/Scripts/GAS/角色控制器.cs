using Pathfinding;
using UnityEngine;

public class 角色控制器 : MonoBehaviour
{
    
    CharacterStats MyStats;
    public CharacterStats MyTarget;
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





    public float 最大转向速度;
    public float 最小转向速度;



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
        if (MyStats.IsDead)             //单位死亡 清空索敌目标
        {
            if(MyTarget != null)   MyTarget = null;
            if(!ASC.Tags.HasTagExact(TDGameTags.IsDead)) ASC.Tags.AddTag(TDGameTags.IsDead);
            停止移动();
            停止攻击();
            return;
        }

        if (MyTarget != null && MyTarget.IsDead)         //目标死亡 清空索敌目标
        {
            MyTarget = SearchingTarget.ScanWithWeapon(普攻实例);   //目标死亡后立刻进行一次搜索 如果有敌人就继续打 没有就停火归null
        }    


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






        if(MyTarget != null)
        {
            目标距离 = Vector3.Distance(MyStats.transform.position, MyTarget.transform.position);
            if(目标距离 > MyStats.ShootRange)
            {
                if(!ASC.Tags.HasTagExact(TDGameTags.IsMove)) 开始移动();
                寻路系统.SetDestination(MyTarget.transform.position);
            }
            else                            
            {
                if (ASC.Tags.HasTagExact(TDGameTags.IsMove))
                {                   
                    寻路系统.SetPath(null);
                    停止移动();   
                }

                LookAtTarget(MyTarget);
                开始攻击();
            }
        }


    }





    void 停止移动()
    {
        寻路系统.updateRotation = false;

        var policy = 寻路系统.autoRepath;
        policy.mode = AutoRepathPolicy.Mode.Never;
        寻路系统.autoRepath = policy;
        ASC.Tags.RemoveTag(TDGameTags.IsMove);
    }


    void 开始移动()
    {
        寻路系统.updateRotation = true;

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


    void LookAtTarget(CharacterStats Target)
    {
        Vector3 Direction = Target.transform.position - transform.position;
        Direction.y = 0f;

        if(Direction.sqrMagnitude < 0.01f) return;

        Quaternion TargetRotation = Quaternion.LookRotation(Direction);

        float Angle = Quaternion.Angle(transform.parent.rotation, TargetRotation);

        float 转向速度 = Mathf.Lerp(最小转向速度, 最大转向速度, Angle / 180f);

        transform.parent.rotation = Quaternion.RotateTowards(transform.parent.rotation, TargetRotation, 转向速度 * Time.deltaTime);
    }


}
