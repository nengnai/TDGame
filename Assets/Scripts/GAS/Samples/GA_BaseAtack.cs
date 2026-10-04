// 基础普攻

// @todo:将获取浮点值改为使用新的 modify 系统
// @todo:实现准备射击技能，当准备射击之后调用最终攻击 GA，或者直接内置一个前摇系统
// @todo:实现伤害、特效、动画函数

using UnityEngine;

interface IWeaponAtackInterface
{
    float GetShootRange();
    float GetDetectRange();
}

public class GA_BaseAttack : GameAbility, IWeaponAtackInterface
{
    // 攻击间隔
    //一轮最大连发次数
    public int MaxBurstCount = 1;
    // 消耗弹药数


    private float WindUpDelay;
    private float WindDownDelay;
    private FTimerHandle BurstDelayHandle;
    private FTimerHandle ShootDelayHandle;
    private int BurstCount = 0;

    private CharacterStats MyStats;
    private Animator MyAnim;
    private AnimatorOverrideController AnimController;
    private SearchingTarget SearchingTarget;


    /* ---- 重写父项函数 ---- */
    public override void OnGranted()
    {
        base.OnGranted();
        
        // 禁止移动时开火
        CancelTags.AddTag(TDGameTags.IsMove);
        BlockedByTags.AddTag(TDGameTags.IsMove);
        
        MyStats = Owner.GetComponent<CharacterStats>();
        MyAnim = Owner.GetComponent<Animator>();
        SearchingTarget = Owner.GetComponent<SearchingTarget>();

        AnimController = MyAnim.runtimeAnimatorController as AnimatorOverrideController;
        WindUpDelay = AnimController["AttackStart"] == null ? 0f : AnimController["AttackStart"].length;
    }

    public sealed override bool CanActivate()  
    {
        if (!base.CanActivate()) return false;
        return true;
    }
    
    public override void Activate()
    {
        // 定时器有效则证明还在转 CD
        if (ShootDelayHandle.IsValid()) return;
        if (BurstDelayHandle.IsValid()) return;


        if(WindUpDelay > 0f)
        {
            MyAnim.CrossFade("AttackStart", 0.1f);
            TimeManager.AddTimer(WindUpDelay, false, false, () => TryShoot());
        }
        else
        {
            TryShoot();
        }
    }

    public override void EndAbility()
    {
        base.EndAbility();
        // 感觉不用了？
        //BurstDelayHandle.RemoveTimer();
    }
    
    /* ---- 实现接口 ---- */
    // 获取射击距离
    public float GetShootRange() => MyStats.GetModifiedValue(TDGameTags.ShootRange);
    public float GetDetectRange() => MyStats.GetModifiedValue(TDGameTags.DetectRange);
    
    /* ---- 射击实现 ---- */
    // 用于计算是否可以射击
    /*protected virtual bool CanShoot()
    {
        if (MyStats.GetModifiedValue(TDGameTags.Ammo) <= 0) return false;
        return true;
    }
    */
    // 射击时判断
    private void TryShoot()
    {
        if (!IsActive) return;
        
        SearchingTarget.ScanWithWeapon(this);
        CharacterStats Target = SearchingTarget.AttackTarget; // 通过某种方法获取
        
        if(!Target)return;

        /*if (!CanShoot())
        {
            // 应该换弹了，但是不在这里触发，在外部 AI 部分
            return;
        }*/
        
        BurstCount++;

        Shoot(Target);

        if (BurstCount >= MaxBurstCount)
        {
            ShootDelayHandle = TimeManager.AddTimer(MyStats.GetModifiedValue(TDGameTags.ShootDelay), false, false, () => 
            {
                BurstCount = 0;
                TryShoot();
            });
        }
        else
        {
            BurstDelayHandle = TimeManager.AddTimer(MyStats.GetModifiedValue(TDGameTags.BurstDelay), false, false, TryShoot);
        }
    }

    // 最终射击代码
    protected virtual void Shoot(CharacterStats Target)
    {
        FGameTag UseAttackType;
        switch (Target.ArmorKind)
        {
            case CharacterStats.ArmorType.Light:
            UseAttackType = TDGameTags.ExplosionDmg;
            break;

            case CharacterStats.ArmorType.Medium:
            UseAttackType = TDGameTags.PenetrationDmg;
            break;

            case CharacterStats.ArmorType.Heavy:
            UseAttackType = TDGameTags.EnergyDmg;
            break;

            default:
            UseAttackType = TDGameTags.Damage;
            break;
        }

        Target.DecreaseHealth(MyStats.GetModifiedValue(UseAttackType));
    }
}