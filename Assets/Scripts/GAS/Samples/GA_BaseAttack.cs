// 基础普攻

// @todo:实现伤害、特效函数

using UnityEngine;
interface IWeaponAtackInterface
{
    float GetShootRange();
    float GetDetectRange();
}

[CreateAssetMenu(fileName = "普通射击", menuName = "Scriptable GAS/NewScriptableObjectScript")]
public class GA_BaseAttack : GameAbility, IWeaponAtackInterface
{
    // 攻击间隔
    public float ShootDelay = 0.1f;
    public float BurstDelay = 0.01f;
    
    //一轮最大连发次数
    public int MaxBurstCount = 1;
    
    // 攻击距离
    public float DetectRange = 100.0f;
    public float ShootRange = 100.0f;
    
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
    
    // 获取射击时间
    protected float GetFireDelay(bool IsBurst = false) => IsBurst ? 
            MyStats.GetModifiedValue(TDGameTags.BurstDelay, BurstDelay) :
            MyStats.GetModifiedValue(TDGameTags.ShootDelay, ShootDelay);
    
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

        Shoot(Target, BurstCount < MaxBurstCount);

        if (BurstCount >= MaxBurstCount)
        {
            ShootDelayHandle = TimeManager.AddTimer(GetFireDelay(), false, false, () => 
            {
                BurstCount = 0;
                TryShoot();
            });
        }
        else
        {
            BurstDelayHandle = TimeManager.AddTimer(GetFireDelay(true), false, false, TryShoot);
        }
    }

    // 最终射击代码
    protected virtual void Shoot(CharacterStats Target, bool IsBurst)
    {
        // 伤害、特效、动画 啥的
        AnimatorEventData AnimatorData;
        AnimatorData.AnimatorTime = GetFireDelay(IsBurst);
        
        MyStats.AnimatorEvent?.TriggerAction(TDGameTags.AnimatorFireEvent, AnimatorData);
        
        
        Target.DecreaseHealth(MyStats.Damage);
        // @todo: 调用 CharacterStats 的一个伤害函数，传入攻击类型与伤害
    }
    
    
}