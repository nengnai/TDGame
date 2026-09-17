using UnityEngine;
using System.Collections.Generic;
using System;


public struct FAbilityHandle
{
    private readonly uint HandleID;
    public FAbilityHandle(uint ID)
    {
        HandleID = ID;
    }

    public override int GetHashCode()
    {
        return (int)HandleID;
    }
}

public struct FEffectHandle
{
    // 定义 HandleID = UInt32.MaxValue 为无效句柄
    private readonly uint HandleID;

    public FEffectHandle(uint ID)
    {
        HandleID = ID;
    }
    
    public static FEffectHandle Create(ref uint _effectID)
    {
        uint ID = _effectID++;
        if (ID == UInt32.MaxValue)
        {
            ID = _effectID = 0;
        }
        return new FEffectHandle(ID);
    }
    
    
    /* 工具 */
    public bool IsValid => HandleID != UInt32.MaxValue;

    public override int GetHashCode()
    {
        return (int)HandleID;
    }
    
    public static readonly FEffectHandle Invalid = new FEffectHandle(UInt32.MaxValue);

}


/*public class StackEntry
{
    public FTimerHandle TimerHandle;
    public FEffectHandle EffectHandle;
    public StackEntry(FTimerHandle THandle, FEffectHandle EHandle)
    {
        TimerHandle = THandle;
        EffectHandle = EHandle;
    }
}
*/


public class AbilityTimeManager
{
    private List<FTimerHandle> Handles = new();

    public FTimerHandle AddTimer(float Time, bool IsLoop, bool IsUnscaled, Action Callback)
    {
        FTimerHandle Handle = TimerSubsystem.GetSubsystem().AddTimer(Time, IsLoop, IsUnscaled, Callback);
        Handles.Add(Handle);
        return Handle;
    }

    public void RemoveTimer(FTimerHandle Handle)
    {
        TimerSubsystem.GetSubsystem().RemoveTimer(Handle);
        Handles.Remove(Handle);
    }

    public void ClearAll()
    {
        foreach (FTimerHandle Handle in Handles)
        {
            TimerSubsystem.GetSubsystem().RemoveTimer(Handle);
        }
        Handles.Clear();
    }

}


public class AbilitySystemComponent : MonoBehaviour
{
    /* 技能数据存放 */
    private Dictionary<FAbilityHandle, GameAbility> Abilities = new();
    private Dictionary<FGameTag, List<FAbilityHandle>> AbilitiesByTag = new();
    private List<GameAbility> OnActivateAbilities = new();
    private uint AbilityID;

    /* 效果数据存放 */
    // 句柄对应技能
    private Dictionary<FEffectHandle, GameEffect> EffectHandles = new();
    private Dictionary<FGameTag, GameEffect> EffectsByTag = new();
    private Dictionary<GameEffect, List<FEffectHandle>> EffectInstances = new();
    private Dictionary<FEffectHandle, FTimerHandle> EffectTimers = new();
    private uint EffectID;


    public GameTagContainer Tags = new();



    void Update()
    {
        foreach(var Effect in EffectInstances.Keys)
        {
            Effect.OnTick();
        }
    }


    public FAbilityHandle AddAbility(GameAbility Config)
    {
        FAbilityHandle Handle = new FAbilityHandle(AbilityID++);

        GameAbility Ability = null;

        switch (Config.Policy)
        {
            case GameAbility.InstantiationPolicy.Static:
            {
                Ability = Config;
                break;
            }
            case GameAbility.InstantiationPolicy.OnGranted:
            {
                Ability = Instantiate(Config);
                Ability.OnGranted();
                break;
            }
            case GameAbility.InstantiationPolicy.OnActivate:
            {
                Ability = Config;
                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        Ability.Owner = this;

        Abilities[Handle] = Ability;

        if (Config.AbilityTag.IsValid())
        {
            if(!AbilitiesByTag.TryGetValue(Config.AbilityTag, out List<FAbilityHandle> List))
            {
                List = new List<FAbilityHandle>();
                AbilitiesByTag[Config.AbilityTag] = List;
            }
            List.Add(Handle);
        }

        return Handle;
    }




    public void RemoveAbility(FAbilityHandle Handle)
    {
        if(!Abilities.Remove(Handle, out GameAbility Ability))
        { return; }

        if (Ability.AbilityTag.IsValid())
        {
            if(AbilitiesByTag.TryGetValue(Ability.AbilityTag, out List<FAbilityHandle> List))
            {
                List.Remove(Handle);

                if(List.Count == 0) AbilitiesByTag.Remove(Ability.AbilityTag);

            }
        }

        if(Ability.Policy == GameAbility.InstantiationPolicy.OnActivate) OnActivateAbilities.Remove(Ability);

    }




    public void ActivateAbility(FAbilityHandle Handle)
    {
        if(CanActivateAbility(Handle) == false)
        { return; }
        
        if(!Abilities.TryGetValue(Handle, out GameAbility Ability))
        { return; }

        GameAbility AbilityToActivate;

        if(Ability.Policy == GameAbility.InstantiationPolicy.OnActivate)
        {
            AbilityToActivate = Instantiate(Ability);
            AbilityToActivate.Owner = this;
            AbilityToActivate.OnGranted();
            OnActivateAbilities.Add(AbilityToActivate);
        }
        else
        {
            AbilityToActivate = Ability;
        }

        if(AbilityToActivate.AbilityTag.IsValid()) Tags.AddTag(AbilityToActivate.AbilityTag);

        AbilityToActivate.Activate();

    }





    public void CancelAbility(FAbilityHandle Handle)
    {
        if (!Abilities.TryGetValue(Handle, out GameAbility Ability))
        { return; }

        Ability.Cancel();

        if (Ability.AbilityTag.IsValid()) Tags.RemoveTag(Ability.AbilityTag);

        if (Ability.Policy == GameAbility.InstantiationPolicy.OnActivate)
        {
            OnActivateAbilities.Remove(Ability);
            Destroy(Ability);
        }
    }





    public bool CanActivateAbility(FAbilityHandle Handle)
    {
        if (!Abilities.TryGetValue(Handle, out GameAbility Ability))
        { return false; }

        if (Ability.RequiredTags.Count != 0
            && Tags.HasAllTags(Ability.RequiredTags) == false)
        { return false; }
        
        if (Ability.BlockedTags.Count != 0
            && Tags.HasAnyTag(Ability.BlockedTags))
        { return false; }
        
        return true;
    }





    public void OnAbilityEnd(GameAbility Instance)
    {
        if(!OnActivateAbilities.Contains(Instance))
        { return; }

        OnActivateAbilities.Remove(Instance);

        if(Instance.AbilityTag.IsValid())
        { Tags.RemoveTag(Instance.AbilityTag); }

        Destroy(Instance);
    }



    public FEffectHandle ApplyEffect(GameEffect Config, int StackCount = 1)
    {
        // 初始化Effect
        GameEffect Effect = Config.Policy == GameEffect.InstantiationPolicy.Static ? Config : Instantiate(Config);
        
        // 如果是立即生效的就直接应用并返回
        if(Config.DurPolicy == GameEffect.DurationPolicy.Instant)
        {
            Effect.Owner = this;
            Effect.OnApplied();
            Effect.OnPeriod();
            return FEffectHandle.Invalid;
        }
        
        // 
        if (Config.EffectTag.IsValid()
            && EffectsByTag.TryGetValue(Config.EffectTag, out GameEffect Effect1))
        {
            if(Effect1.CurrentStack > Effect1.MaxStack)
            {
                return FEffectHandle.Invalid;
            }
            FEffectHandle Handle = FEffectHandle.Create(ref EffectID);
            FTimerHandle THandle = TimerSubsystem.GetSubsystem().AddTimer(Effect1.GetDuration(), false, false, () => OnStackExpired(Handle));
        

            EffectHandles[Handle] = Effect1;
            EffectInstances[Effect1].Add(Handle);
            EffectTimers[Handle] = THandle;


            Effect1.CurrentStack++;
            Effect1.OnStackChanged();

            return Handle;
        }
        
        FEffectHandle NewHandle = FEffectHandle.Create(ref EffectID);
        

        Effect.Owner = this;
        Effect.CurrentStack = 1;
        EffectHandles[NewHandle] = Effect;
        EffectInstances[Effect] = new List<FEffectHandle>{ NewHandle };

        if (Config.EffectTag.IsValid())
        {
            EffectsByTag[Effect.EffectTag] = Effect;
        }
        
        FTimerHandle TimerHandle = TimerSubsystem.GetSubsystem().AddTimer(
            Effect.GetDuration(),
            false, 
            false,
            () => OnStackExpired(NewHandle)
        );

        EffectTimers[NewHandle] = TimerHandle;

        Effect.OnApplied();

        if(Effect.Period > 0)
        {
            Effect.PeriodTimerHandle = TimerSubsystem.GetSubsystem().AddTimer(
                Effect.Period, 
                true,
                false,
                () => Effect.OnPeriod()
            );
        }
        return NewHandle;

    }




    public void RemoveEffect(FEffectHandle Handle)
    {
        if(!EffectHandles.TryGetValue(Handle, out GameEffect Effect)) return;
        if(EffectTimers.TryGetValue(Handle, out FTimerHandle THandle))
        {
            TimerSubsystem.GetSubsystem().RemoveTimer(THandle);
            EffectTimers.Remove(Handle);
        }

        if(EffectInstances.TryGetValue(Effect, out List<FEffectHandle> EHandle))
        {
            EHandle.Remove(Handle);
        }

        EffectHandles.Remove(Handle);
        Effect.CurrentStack--;

        if(EffectInstances[Effect].Count <= 0)
        {
            if(Effect.Period > 0) TimerSubsystem.GetSubsystem().RemoveTimer(Effect.PeriodTimerHandle);
            Effect.OnRemoved();
            EffectInstances.Remove(Effect);

            if(Effect.EffectTag.IsValid()) EffectsByTag.Remove(Effect.EffectTag);

            if(Effect.Policy == GameEffect.InstantiationPolicy.OnGranted) Destroy(Effect);
        }
        else
        {
            Effect.OnStackChanged();
        }
        
    }


    private void OnStackExpired(FEffectHandle Handle)
    {
        RemoveEffect(Handle);
    }
    
}