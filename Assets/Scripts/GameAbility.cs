using System;
using System.Collections.Generic;
using UnityEngine;

public class GameAbility : ScriptableObject
{
    public FGameTag AbilityTag;
    public enum InstantiationPolicy
    {
        Static,
        OnGranted,
        OnActivate
    }
    public InstantiationPolicy Policy;
    public List<FGameTag> RequiredTags = new List<FGameTag>();
    public List<FGameTag> BlockedTags = new List<FGameTag>();


    [NonSerialized] public AbilitySystemComponent Owner;
    [NonSerialized] public AbilityTimeManager TimeManager;


    public virtual void OnGranted()
    {
        TimeManager = new AbilityTimeManager();
    }


    public virtual void Activate()
    {
        
    }

    public virtual void Cancel()
    {
        TimeManager?.ClearAll();
    }

    public virtual void EndAbility()
    {
        Owner?.OnAbilityEnd(this);
    }


}
