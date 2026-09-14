
using UnityEngine;

public class GameEffect : ScriptableObject
{
    public FGameTag EffectTag;
    public enum InstantiationPolicy
    {
        Static,
        OnGranted,
    }
    public InstantiationPolicy Policy;

    public enum DurationPolicy
    {
        Instant,
        Duration,
        Inf
    }

    public DurationPolicy DurPolicy;
    public float DurTime;
    public float Period;


    public int MaxStacks = 1;
}




public class GameEffectInstance
{
    public GameEffect Config;
    public AbilitySystem Owner;
    public int CurrentStack = 1;
    private float RemainingTime;
    private float PeriodTimer;
    public AbilityTimeManager TimeManager = new AbilityTimeManager();

    private FTimerHandle DurTimerHandle;
    private FTimerHandle PeriodTimerHandle;


    public void InitializeTimers(FEffectHandle Handle)
    {
        if(Config.DurPolicy == GameEffect.DurationPolicy.Instant)
        {
            return;
        }
        else if(Config.DurPolicy == GameEffect.DurationPolicy.Duration)
        {
            DurTimerHandle = TimeManager.AddTimer(Config.DurTime, false, false, () => {Owner.RemoveEffect(Handle);});
            PeriodTimerHandle = TimeManager.AddTimer(Config.Period, true, false, () => {});
        }
        else
        {
            PeriodTimerHandle = TimeManager.AddTimer(Config.Period, true, false, () => {});
        }
    }

    public void RefreshDuration(FEffectHandle Handle)
    {
        TimeManager.RemoveTimer(DurTimerHandle);
        DurTimerHandle = TimeManager.AddTimer(Config.DurTime, false, false, () => {Owner.RemoveEffect(Handle);});
    }





}