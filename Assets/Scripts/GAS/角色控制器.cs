// 用与角色控制与指令传达

using Pathfinding;
using UnityEngine;

public class 角色控制器 : MonoBehaviour
{
    // 其他组件
    CharacterStats MyStats;
    public CharacterStats MyTarget;
    SearchingTarget SearchingTarget;
    AbilitySystemComponent ASC;
    FollowerEntity 寻路系统;
    
    // 移动
    private Vector3 移动目标地点;
    bool 手动移动指令;
    
    // 转向
    public float 最大转向速度;
    public float 最小转向速度;
    Vector3? 面向位置;

    // 普攻
    FAbilityHandle 普攻;
    GA_BaseAttack 普攻实例;



    void Awake()
    {
        MyStats = GetComponent<CharacterStats>();
        SearchingTarget = GetComponent<SearchingTarget>();
        ASC = GetComponent<AbilitySystemComponent>();
        寻路系统 = GetComponentInParent<FollowerEntity>();

        普攻 = ASC.AddAbility(MyStats.NormalAttack);
        普攻实例 = ASC.GetLiveAbility(普攻) as GA_BaseAttack;
    }

    void Start()
    {
        停止攻击();
        停止移动();
    }


    void Update()
    {
        if (MyStats.IsDead)             //单位死亡 清空索敌目标
        {
            if(MyTarget)   MyTarget = null;
            if(!ASC.Tags.HasTagExact(TDGameTags.IsDead)) ASC.Tags.AddTag(TDGameTags.IsDead);
            停止移动();
            停止攻击();
            return;
        }
        
        if (ASC.Tags.HasTag(TDGameTags.IsStun))
        {
            停止移动();
            停止攻击();
            return;
        }

        if (MyTarget && MyTarget.IsDead)         //目标死亡 清空索敌目标
        {
            MyTarget = SearchingTarget.ScanWithWeapon(普攻实例);   //目标死亡后立刻进行一次搜索 如果有敌人就继续打 没有就停火归null
        }
        

        if(!MyStats.IsDead && 面向位置 != null)
        {
            LookAtTarget(面向位置.Value);
        }


        /*  ---- 主动/被动技能检测和释放 ---- */
        if(!MyTarget && !寻路系统.hasPath && !手动移动指令)
        {
            // 使用 SearchingTarget 的缓存，不用外边写
            if (普攻实例 != null)
            { MyTarget = SearchingTarget.ScanWithWeapon(普攻实例); }
        }


        if (寻路系统.hasPath)
        {
            停止攻击();
            if(!ASC.Tags.HasTagExact(TDGameTags.IsMove))  ASC.Tags.AddTag(TDGameTags.IsMove);
            
            if (寻路系统.reachedDestination && 寻路系统.hasPath)
            {
                寻路系统.SetPath(null);
                停止移动();
                手动移动指令 = false;
            }
        }






        // 追击
        //@todo:正常应改为人机自动追击、玩家的单位无法追击？
        if (MyTarget && !手动移动指令)
        {
            if(Vector3.Distance(MyStats.transform.position, MyTarget.transform.position) > MyStats.ShootRange)
            {
                if(!ASC.Tags.HasTagExact(TDGameTags.IsMove)) { 开始移动(); }
                寻路系统.SetDestination(MyTarget.transform.position);
            }
            else
            {
                if (ASC.Tags.HasTagExact(TDGameTags.IsMove))
                {
                    停止移动();
                }
                面向位置 = MyTarget.transform.position;
                开始攻击();
            }
        }
    }





    void 停止移动()
    {
        if (!ASC.Tags.HasTagExact(TDGameTags.IsMove)) { return; }
        
        寻路系统.updateRotation = false;

        var Policy = 寻路系统.autoRepath;
        Policy.mode = AutoRepathPolicy.Mode.Never;
        寻路系统.autoRepath = Policy;
        ASC.Tags.RemoveTag(TDGameTags.IsMove);


        var setting = 寻路系统.rvoSettings;
        setting.locked = true;
        setting.priority = 1f;
        寻路系统.rvoSettings = setting;
        

        寻路系统.SetPath(null);
    }


    void 开始移动()
    {
        停止攻击();
        
        面向位置 = null;
        寻路系统.updateRotation = true;

        var Policy = 寻路系统.autoRepath;
        Policy.mode = AutoRepathPolicy.Mode.EveryNSeconds;
        寻路系统.autoRepath = Policy;
        ASC.Tags.AddTag(TDGameTags.IsMove);


        var setting = 寻路系统.rvoSettings;
        setting.locked = false;
        setting.priority = 0.5f;
        寻路系统.rvoSettings = setting;
    }


    void 停止攻击()
    {
        if (!ASC.IsAbilityActive(普攻)) { return; }
        
        ASC.EndAbility(普攻);
        ASC.Tags.RemoveTag(TDGameTags.IsAttack);
    }

    void 开始攻击()
    {
        ASC.ActivateAbility(普攻);
        ASC.Tags.AddTag(TDGameTags.IsAttack);
    }


    void LookAtTarget(Vector3 Target)
    {
        Vector3 Direction = Target - transform.position;
        Direction.y = 0f;

        if(Direction.sqrMagnitude < 0.01f) return;

        Quaternion TargetRotation = Quaternion.LookRotation(Direction);

        float Angle = Quaternion.Angle(transform.parent.rotation, TargetRotation);

        float 转向速度 = Mathf.Lerp(最小转向速度, 最大转向速度, Angle / 180f);

        transform.parent.rotation = Quaternion.RotateTowards(transform.parent.rotation, TargetRotation, 转向速度 * Time.deltaTime);
    }


    public void SetMoveTargetLocation(Vector3 TargetLocation, bool IsManual = true)
    {
        if (TargetLocation == 移动目标地点)
        {
            寻路系统.SetPath(null);
            停止移动();
            手动移动指令 = false;
            return;
        }
        移动目标地点 = TargetLocation;
        手动移动指令 = IsManual;
        MyTarget = null;
        开始移动();
        寻路系统.SetDestination(移动目标地点);
    }
}
