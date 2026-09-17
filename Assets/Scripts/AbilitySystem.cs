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


public class StackEntry
{
    public FTimerHandle TimerHandle;
    public int StackCount;
    public StackEntry(int Count)
    {
        StackCount = Count;
    }
}



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
    private Dictionary<FEffectHandle, GameEffect> Effects = new();
    private Dictionary<FGameTag, List<FEffectHandle>> EffectsByTag = new();
    private Dictionary<FEffectHandle, List<StackEntry>> EffectsStacks = new();
    private uint EffectID;


    public GameTagContainer Tags = new();



    void Update()
    {
        foreach(var pair in Effects)
        {
            pair.Value.OnTick();
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
            && EffectsByTag.TryGetValue(Config.EffectTag, out List<FEffectHandle> Handle) && Handle.Count > 0)
        {
            FEffectHandle ExistHandle = Handle[0];
            GameEffect ExistEffect = Effects[ExistHandle];
            
            if(ExistEffect.CurrentStack >= ExistEffect.MaxStack)
            { return ExistHandle; }
            
            StackEntry NewStack = new(StackCount);
            FTimerHandle TimerHandle1 = TimerSubsystem.GetSubsystem().AddTimer(
                ExistEffect.GetDuration(),
                false, 
                false,
                () => OnStackExpired(ExistHandle, NewStack)
            );
            NewStack.TimerHandle = TimerHandle1;

            EffectsStacks[ExistHandle].Add(NewStack);
            ExistEffect.CurrentStack += StackCount;
            ExistEffect.OnStackChanged();

            return ExistHandle;
        }
        
        FEffectHandle NewHandle = FEffectHandle.Create(ref AbilityID);
        

        Effect.Owner = this;
        Effect.CurrentStack = StackCount;

        Effects[NewHandle] = Effect;

        if (Config.EffectTag.IsValid())
        {
            if(!EffectsByTag.TryGetValue(Effect.EffectTag, out List<FEffectHandle> List))
            {
                List = new List<FEffectHandle>();
                EffectsByTag[Effect.EffectTag] = List;
            }

            List.Add(NewHandle);
        }
        


        List<StackEntry> StackList = new();
        StackEntry NewStackEntry = new(StackCount);
        FTimerHandle TimerHandle = TimerSubsystem.GetSubsystem().AddTimer(
            Effect.GetDuration(),
            false, 
            false,
            () => OnStackExpired(NewHandle, NewStackEntry)
        );
        NewStackEntry.TimerHandle = TimerHandle;
        StackList.Add(NewStackEntry);
        EffectsStacks[NewHandle] = StackList;

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
        if(!Effects.TryGetValue(Handle, out GameEffect Effect)) return;
        if(EffectsStacks.TryGetValue(Handle, out List<StackEntry> List))
        {
            foreach (StackEntry Entry in List)
            {
                TimerSubsystem.GetSubsystem().RemoveTimer(Entry.TimerHandle);
            }
            EffectsStacks.Remove(Handle);
        }
        if(Effect.Period > 0) TimerSubsystem.GetSubsystem().RemoveTimer(Effect.PeriodTimerHandle);  
        Effect.OnRemoved();
        Effects.Remove(Handle);
        if (Effect.EffectTag.IsValid())
        {
            if(EffectsByTag.TryGetValue(Effect.EffectTag, out List<FEffectHandle> List2))
            {
                List2.Remove(Handle);
                if(List2.Count == 0) EffectsByTag.Remove(Effect.EffectTag);
            }
            
        }
    }


    private void OnStackExpired(FEffectHandle Handle, StackEntry ExpiredStack)
    {
        if(!Effects.TryGetValue(Handle, out GameEffect Effect)) return;
        if(!EffectsStacks.TryGetValue(Handle, out List<StackEntry> List)) return;
        List.Remove(ExpiredStack);
        Effect.CurrentStack -= ExpiredStack.StackCount;
        Effect.OnStackChanged();

        if(Effect.CurrentStack <= 0 || List.Count == 0) RemoveEffect(Handle);
    }
    
}