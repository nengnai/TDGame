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
    public static FAbilityHandle Create(ref uint _abilityID)
    {
        if (_abilityID == UInt32.MaxValue)
        {
            _abilityID = 0;
        }
        return new FAbilityHandle(_abilityID++);
    }

    public bool IsValid => HandleID != UInt32.MaxValue;
    public override int GetHashCode()
    {
        return (int)HandleID;
    }
    public static readonly FAbilityHandle Invalid = new FAbilityHandle(UInt32.MaxValue);
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
        if (_effectID == UInt32.MaxValue)
        {
            _effectID = 0;
        }
        return new FEffectHandle(_effectID++);
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
    private List<GameAbility> ActivateAbilities = new();
    private GameTagContainer BlockedAbilityTags = new();
    private List<GameAbility> TickAbilityBuffer = new();
    private uint AbilityID;

    /* 效果数据存放 */
    // 句柄对应技能
    private Dictionary<FEffectHandle, int> HandleToEffect = new();
    private Dictionary<FGameTag, int> EffectsByTag = new();
    private Dictionary<int, List<FEffectHandle>> EffectToHandle = new();
    private Dictionary<FEffectHandle, FTimerHandle> EffectTimers = new();
    private Dictionary<int, GameEffect> EffectInstances = new();
    private List<GameEffect> TickEffectBuffer = new();


    private uint EffectID;


    public GameTagContainer Tags = new();




    void Update()
    {
        TickAbilityBuffer.Clear();
        TickAbilityBuffer.AddRange(ActivateAbilities);
        foreach(var Target in TickAbilityBuffer)
        {
            if(Target.IsActive) Target.OnTick();
        }

        TickEffectBuffer.Clear();
        TickEffectBuffer.AddRange(EffectInstances.Values);
        foreach(var Target in TickEffectBuffer)
        {
            Target.OnTick();
        }
    }


    public FAbilityHandle AddAbility(GameAbility Config)
    {
        FAbilityHandle Handle = FAbilityHandle.Create(ref AbilityID);

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
        Ability.Handle = Handle;
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
        if(!Abilities.TryGetValue(Handle, out GameAbility Ability))
        { return; }

        CancelAbility(Handle);
        Abilities.Remove(Handle);

        if (Ability.AbilityTag.IsValid())
        {
            if(AbilitiesByTag.TryGetValue(Ability.AbilityTag, out List<FAbilityHandle> List))
            {
                List.Remove(Handle);

                if(List.Count == 0) AbilitiesByTag.Remove(Ability.AbilityTag);

            }
        }

        if(Ability.Policy == GameAbility.InstantiationPolicy.OnGranted) Destroy(Ability);

    }




    public void ActivateAbility(FAbilityHandle Handle)
    {
        if(!Abilities.TryGetValue(Handle, out GameAbility Ability)) return;
        
        if(Ability.AbilityTag.IsValid() && BlockedAbilityTags.HasTag(Ability.AbilityTag)) return;

        if(!Ability.CanActivate()) return;
        if(Ability.Policy != GameAbility.InstantiationPolicy.OnActivate && Ability.IsActive == true) return;

        if(Ability.CancelTags.Count > 0)
        {
            List<GameAbility> CheckList = new();
            foreach (GameAbility Active in ActivateAbilities)
            {
                foreach(FGameTag Tag in Active.OwnTags)
                {
                    bool Matched = false;
                    foreach(FGameTag CancelTag in Ability.CancelTags)
                    {
                        if(Tag == CancelTag) {Matched = true; break;}
                    }
                    if(Matched) {CheckList.Add(Active); break;}
                }
            }

            foreach (GameAbility Target in CheckList) CancelAbility(Target.Handle);
        }

        GameAbility AbilityToActivate;
        if(Ability.Policy == GameAbility.InstantiationPolicy.OnActivate)
        {
            AbilityToActivate = Instantiate(Ability);
            AbilityToActivate.Owner = this;
            AbilityToActivate.Handle = Handle;
            AbilityToActivate.OnGranted();
        }
        else
        {
            AbilityToActivate = Ability;
        }

        ActivateAbilities.Add(AbilityToActivate);
        AbilityToActivate.IsActive = true;
        foreach(FGameTag Tag in AbilityToActivate.OwnTags) Tags.AddTag(Tag);
        foreach(FGameTag Tag in AbilityToActivate.BlockingTags) BlockedAbilityTags.AddTag(Tag);


        AbilityToActivate.Activate();
    }





    public void CancelAbility(FAbilityHandle Handle)
    {
        List<GameAbility> CheckList = new();
        foreach(GameAbility Active in ActivateAbilities)
        {
            if(Active.Handle.Equals(Handle)) CheckList.Add(Active);
        }

        foreach(GameAbility Target in CheckList)
        {
            Target.IsActive = false;
            Target.Cancel();
            ShutdownAbilityInstance(Target);
        }
    }


    private void ShutdownAbilityInstance(GameAbility Instance)
    {
        Instance.IsActive = false;
        foreach(FGameTag Tag in Instance.OwnTags)
        {
            Tags.RemoveTag(Tag);
        }
        foreach(FGameTag Tag in Instance.BlockingTags)
        {
            BlockedAbilityTags.RemoveTag(Tag);
        }
        ActivateAbilities.Remove(Instance);

        if(Instance.Policy == GameAbility.InstantiationPolicy.OnActivate) Destroy(Instance);
    }







    public void OnAbilityEnd(GameAbility Instance)
    {
        if(!ActivateAbilities.Contains(Instance))
        { return; }

        ShutdownAbilityInstance(Instance);
    }



    public FEffectHandle ApplyEffect(GameEffect Config)
    {
        if(Config.DurPolicy == GameEffect.DurationPolicy.Instant)
        {
            GameEffect Effect = Config.Policy == GameEffect.InstantiationPolicy.Static ? Config : Instantiate(Config);
            Effect.Owner = this;
            Effect.OnApplied();
            Effect.OnPeriod();
            return FEffectHandle.Invalid;
        }
        
        FEffectHandle NewHandle = FEffectHandle.Create(ref EffectID);

        int ConfigID = Config.GetInstanceID();
        
        if(EffectInstances.TryGetValue(ConfigID, out GameEffect InstancesEffect))
        {
            // 如果已经有效果实例
            if(InstancesEffect.CurrentStack >= InstancesEffect.MaxStack) return FEffectHandle.Invalid;
            
            

            HandleToEffect.Add(NewHandle, ConfigID);
            EffectToHandle[ConfigID].Add(NewHandle);
            if (Config.DurPolicy == GameEffect.DurationPolicy.Duration)
            {
                FTimerHandle THandle = TimerSubsystem.GetSubsystem().AddTimer(InstancesEffect.GetDuration(), false, false, () => OnStackExpired(NewHandle));
                EffectTimers.Add(NewHandle, THandle);
            }
            

            InstancesEffect.CurrentStack++;
            InstancesEffect.OnStackChanged();

            return NewHandle;
        }

        // 无实例时获取默认实例
        GameEffect TargetEffect = Config.Policy == GameEffect.InstantiationPolicy.Static ? Config : Instantiate(Config);
        TargetEffect.Owner = this;
        TargetEffect.CurrentStack = 1;
        
        // 注册
        EffectInstances.Add(ConfigID, TargetEffect);
        HandleToEffect.Add(NewHandle, ConfigID);
        EffectToHandle.Add(ConfigID, new List<FEffectHandle> {NewHandle});
        if (Config.EffectTag.IsValid()) { EffectsByTag[Config.EffectTag] = ConfigID; }
        
        // 注册 GE 时长
        if(TargetEffect.DurPolicy == GameEffect.DurationPolicy.Duration)
        {
            FTimerHandle THandle = TimerSubsystem.GetSubsystem().AddTimer(
                TargetEffect.GetDuration(),
                false,
                false,
                () => OnStackExpired(NewHandle)
            );
            EffectTimers.Add(NewHandle, THandle);
        }

        TargetEffect.OnApplied();
        
        // 周期注册
        if(TargetEffect.Period > 0)
        {
            TargetEffect.PeriodTimerHandle = TimerSubsystem.GetSubsystem().AddTimer(
                TargetEffect.Period,
                true,
                false,
                () => TargetEffect.OnPeriod()
            );
        }

        return NewHandle;
        
        
        



    }




    public void RemoveEffect(FEffectHandle Handle)
    {
        if(!HandleToEffect.TryGetValue(Handle, out int ConfigID)) return;

        if (EffectTimers.TryGetValue(Handle, out FTimerHandle THandle))
        {
            TimerSubsystem.GetSubsystem().RemoveTimer(THandle);
            EffectTimers.Remove(Handle);
        }


        HandleToEffect.Remove(Handle);
        EffectToHandle.TryGetValue(ConfigID, out List<FEffectHandle> EHandle);
        EHandle?.Remove(Handle);
        if(!EffectInstances.TryGetValue(ConfigID, out GameEffect Effect)) return;
        Effect.CurrentStack--;
        

        if(EHandle != null && EHandle.Count > 0)
        {
            Effect.OnStackChanged();
            return;
        }
        
        if(Effect.Period > 0)
        {
            TimerSubsystem.GetSubsystem().RemoveTimer(Effect.PeriodTimerHandle);
        }
        Effect.OnRemoved();
        EffectInstances.Remove(ConfigID);
        EffectToHandle.Remove(ConfigID);

        if (Effect.EffectTag.IsValid())
        {
            if(EffectsByTag.TryGetValue(Effect.EffectTag, out int TagID) && TagID == ConfigID)
            {
                EffectsByTag.Remove(Effect.EffectTag);
            }
        }

        if(Effect.Policy == GameEffect.InstantiationPolicy.OnGranted)
        {
            Destroy(Effect);
        }



    }


    private void OnStackExpired(FEffectHandle Handle)
    {
        RemoveEffect(Handle);
    }
    
}