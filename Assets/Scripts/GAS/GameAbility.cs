using System;
using UnityEngine;



public class GameAbility : ScriptableObject
{
    public enum InstantiationPolicy
    {
        Static,
        OnGranted,
        OnActivate
    }

    public InstantiationPolicy Policy;
    

    public GameTagContainer RequiredTags = new(); // 激活技能需要的 tags
    public GameTagContainer BlockedByTags = new(); // 有这些 tags 技能无法激活
    public GameTagContainer BlockingTags = new(); // 技能激活时拒绝让有这些 tags 的技能激活
    public GameTagContainer CancelTags = new(); // 激活时要打断目标身上有这些 tags 的技能（眩晕）
    public GameTagContainer OwnTags = new(); // 激活时给目标身上挂的 Tags

    /* 运行时上下文 */
    [NonSerialized] public AbilitySystemComponent Owner;
    [NonSerialized] public FAbilityHandle Handle;
    [NonSerialized] public AbilityTimeManager TimeManager;   // 仅 OnGranted / OnActivate 会创建
    //@todo:Static 策略下上面这些字段写在共享 ScriptableObject 资产上，属于"调用期临时值"，调用结束即被清空
    
    public bool IsActive => Owner != null && Owner.IsAbilityActive(Handle);


    
    // 只在 OnGranted / OnActivate 的实例上被调用；Static 不调用
    public virtual void OnGranted()
    {
        TimeManager = new AbilityTimeManager();
    }

    // 是否可以激活技能
    public virtual bool CanActivate()
    {
        if (Owner == null) return false;

        if (!RequiredTags.IsEmpty && !Owner.Tags.MatchAllContainer(RequiredTags)) return false;
        if (!BlockedByTags.IsEmpty && Owner.Tags.MatchAnyContainer(BlockedByTags)) return false;

        return true;
    }

    public virtual void OnTick() {}

    public virtual void Activate() {}

    
    public virtual void Cancel()
    {
        TimeManager?.ClearAll();
    }

    
    public virtual void EndAbility()
    {
        TimeManager?.ClearAll();
        Owner?.ShutdownAbility(this);
        
        if (Policy == InstantiationPolicy.Static)
        { return; }
    }
}


public class GameAbilitySpec
{
    public readonly GameAbility Config; // 配置资产
    public readonly FAbilityHandle Handle; // 句柄

    public GameAbility Instance; // 运行实例

    public bool IsActive; // 激活状态的唯一真源(GA 不另存一份)

    public GameAbilitySpec(GameAbility InConfig, FAbilityHandle InHandle)
    {
        Config = InConfig;
        Handle = InHandle;
    }
}
