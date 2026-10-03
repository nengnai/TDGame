// 基础普攻

// @todo:将获取浮点值改为使用新的 modify 系统
// @todo:实现准备射击技能，当准备射击之后调用最终攻击 GA，或者直接内置一个前摇系统
// @todo:实现伤害、特效、动画函数

using UnityEngine;

interface IWeaponAtackInterface
{
    float GetShootRange();
}

public class GA_BaseAtack : GameAbility, IWeaponAtackInterface
{
    // 攻击距离
    public float ShootRange = 400.0f;
    // 攻击间隔
    public float ShootDelay = 0.1f;
    // 连发间隔
    public float BurstDelay = 0.01f;  
    //一轮最大连发次数
    public int MaxBurstCont = 1;
    // 消耗弹药数
    public int AmmoCostPerShot = 1;

    private FTimerHandle BurstDelayHandle;
    private FTimerHandle ShootDelayHandle;
    private int BurstCount = 0;

    private CharacterStats MyStats;

    /* ---- 重写父项函数 ---- */
    public override void OnGranted()
    {
        base.OnGranted();
        
        // 需要准备之后才能开火
        RequiredTags.AddTag(TDGameTags.FireReady);
        // 禁止移动时开火
        CancelTags.AddTag(TDGameTags.IsMove);
        BlockedByTags.AddTag(TDGameTags.IsMove);
        
        MyStats = Owner.GetComponent<CharacterStats>();
    }

    public sealed override bool CanActivate() 
    {
        if (!base.CanActivate()) return false;
        return CanShoot();
    }
    
    public override void Activate()
    {
        // 定时器有效则证明还在转 CD
        if (ShootDelayHandle.IsValid()) return;
        if (BurstDelayHandle.IsValid()) return;
        
        // 定时器无效则说明可以开火
        TryShoot();
    }

    public override void EndAbility()
    {
        base.EndAbility();
        // 感觉不用了？
        //BurstDelayHandle.RemoveTimer();
    }
    
    /* ---- 实现接口 ---- */
    // 获取射击距离
    public float GetShootRange() => ShootRange;
    
    /* ---- 射击实现 ---- */
    // 用于计算是否可以射击
    protected virtual bool CanShoot()
    {
        if (MyStats.CurrentAmmo <= 0) return false;
        return true;
    }
    
    // 射击时判断
    private void TryShoot()
    {
        if (!IsActive) return;
        
        CharacterStats Target = null; // 通过某种方法获取
        
        //if(!Target)return;

        if (!CanShoot())
        {
            // 应该换弹了，但是不在这里触发，在外部 AI 部分
            return;
        }
        
        MyStats.CurrentAmmo -= AmmoCostPerShot;
        BurstCount++;

        Shoot(Target);

        if (BurstCount >= MaxBurstCont)
        {
            ShootDelayHandle = TimeManager.AddTimer(ShootDelay, false, false, () => 
            {
                BurstCount = 0;
                TryShoot();
            });
        }
        else
        {
            BurstDelayHandle = TimeManager.AddTimer(BurstDelay, false, false, TryShoot);
        }
    }

    // 最终射击代码
    protected virtual void Shoot(CharacterStats Target)
    {
        // 伤害、特效、动画 啥的
        Debug.Log("测试射击！目标：" + Target);
    }
}