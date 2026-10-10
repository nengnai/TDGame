// 基础普攻

// @todo:将获取浮点值改为使用新的 modify 系统
// @todo:实现准备射击技能，当准备射击之后调用最终攻击 GA，或者直接内置一个前摇系统
// @todo:实现伤害、特效、动画函数
// @todo:动画方面应该调用事件总线上的触发函数，动画播放器监听总线上的时间来触发动画，而不是直接在这里调用

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

    
    private FTimerHandle BurstDelayHandle;
    private FTimerHandle ShootDelayHandle;
    private int BurstCount = 0;

    private CharacterStats MyStats;
    private SearchingTarget SearchingTarget;


    /* ---- 重写父项函数 ---- */
    public override void OnGranted()
    {
        base.OnGranted();
        
        // 禁止移动时开火
        CancelTags.AddTag(TDGameTags.IsMove);
        BlockedByTags.AddTag(TDGameTags.IsMove);
        
        MyStats = Owner.GetComponent<CharacterStats>();
        SearchingTarget = Owner.GetComponent<SearchingTarget>();
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
    
    
    /* ---- 实现接口 ---- */
    // 获取射击距离
    public float GetShootRange() => MyStats.GetModifiedValue(TDGameTags.ShootRange);
    public float GetDetectRange() => MyStats.GetModifiedValue(TDGameTags.DetectRange);
    
    /* ---- 射击实现 ---- */
    // 用于计算是否可以射击
    protected virtual bool CanShoot()
    {
        return true;
    }
    
    // 射击时判断
    private void TryShoot()
    {
        if (!IsActive) return;
        
        CharacterStats Target = SearchingTarget.ScanWithWeapon(this);
        
        if(!Target)return;

        if (!CanShoot())
        {
            // 应该换弹了，但是不在这里触发，在外部 AI 部分
            return;
        }
        
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
        // 伤害、特效、动画 啥的
        
        // @todo: 调用 CharacterStats 的一个伤害函数，传入攻击类型与伤害
    }
}