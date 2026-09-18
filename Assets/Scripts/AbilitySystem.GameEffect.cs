using UnityEngine;
using System.Collections.Generic;
using System;



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


public class AbilityTimeManager
{
    private readonly HashSet<FTimerHandle> Handles = new();

    public FTimerHandle AddTimer(float Time, bool IsLoop, bool IsUnscaled, Action Callback)
    {
        FTimerHandle Handle = TimerSubsystem.GetSubsystem().AddTimer(
            Time, 
            IsLoop,
            IsUnscaled, 
            Callback
        );
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

public partial class AbilitySystemComponent : MonoBehaviour
{
    /* 效果数据存放 */
    private readonly Dictionary<FEffectHandle, int> HandleToEffect = new();
    private readonly Dictionary<FGameTag, int> EffectsByTag = new();
    private readonly Dictionary<int, List<FEffectHandle>> EffectToHandle = new();
    private readonly Dictionary<FEffectHandle, FTimerHandle> EffectTimers = new();
    private readonly Dictionary<int, GameEffect> EffectInstances = new();
    private readonly List<GameEffect> TickEffectBuffer = new();
    private uint EffectID;
    
    
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
