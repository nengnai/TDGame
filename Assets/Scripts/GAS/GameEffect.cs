using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEffect : ScriptableObject
{
    public FGameTag EffectTag;
    public enum InstantiationPolicy
    {
        Static,
        OnGranted
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
    public int MaxStack;



    [SerializeReference] public List<GameEffectModifier> Modifiers = new List<GameEffectModifier>();
    [SerializeReference] public List<GameEffectExecutor> Executors = new List<GameEffectExecutor>();

    [NonSerialized] public AbilitySystemComponent Owner;
    [NonSerialized] public int CurrentStack;
    [NonSerialized] public FTimerHandle PeriodTimerHandle;
    [NonSerialized] public GameEffect OriginalConfig;

    public virtual float GetDuration()
    {
        return DurTime;
    }

    public virtual void OnApplied()
    {
        foreach (GameEffectExecutor Executor in Executors)
        {
            Executor.OnApplied(this);
        }
    }

    public virtual void OnRemoved()
    {
        foreach (GameEffectModifier Modifier in Modifiers)
        {
            if(Modifier is ModifyModifier Modify)
            {
                Modify.Remove(Owner.GetComponent<CharacterStats>(), this);
            }
        }

        foreach (GameEffectExecutor Executor in Executors)
        {
            Executor.OnRemoved(this);
        }
    }


    
    public virtual void OnTick()
    {
        foreach (GameEffectExecutor Executor in Executors)
        {
            Executor.OnTick(this);
        }
    }

    public virtual void OnPeriod()
    {
        foreach (GameEffectModifier Modifier in Modifiers)
        {
            if(Modifier is AddModifier Add)
            {
                Add.Apply(Owner.GetComponent<CharacterStats>(), this);
            }
        }

        foreach (GameEffectExecutor Executor in Executors)
        {
            Executor.OnPeriod(this);
        }
    }


    public virtual void OnStackChanged()
    {
        foreach (var Modifier in Modifiers)
        {
            Modifier.OnStackChanged(Owner.GetComponent<CharacterStats>(), this);
        }
        foreach (var Executor in Executors)
        {
            Executor.OnStackChanged(this);
        }
    }

}
