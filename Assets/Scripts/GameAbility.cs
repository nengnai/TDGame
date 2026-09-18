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
    
    //@todo:将 List 换成 GameTagContainer
    public List<FGameTag> RequiredTags = new List<FGameTag>();         //激活技能需要的tags
    public List<FGameTag> BlockedByTags = new List<FGameTag>();        //有这些tags 技能无法激活
    public List<FGameTag> BlockingTags = new List<FGameTag>();         //技能激活时拒绝让有这些tags的技能激活
    public List<FGameTag> CancelTags = new List<FGameTag>();           //激活时要打断目标身上有这些tags的技能（眩晕）
    public List<FGameTag> OwnTags = new List<FGameTag>();              //激活时给目标身上挂的Tags


    [NonSerialized] public AbilitySystemComponent Owner;
    [NonSerialized] public AbilityTimeManager TimeManager;
    [NonSerialized] public bool IsActive;
    [NonSerialized] public FAbilityHandle Handle;


    public virtual void OnGranted()
    {
        TimeManager = new AbilityTimeManager();
    }


    public virtual bool CanActivate()
    {
        if(Owner == null) return false;
        if (RequiredTags.Count != 0
            && Owner.Tags.HasAllTags(RequiredTags) == false)
        { return false; }
        
        if (BlockedByTags.Count != 0
            && Owner.Tags.HasAnyTag(BlockedByTags))
        { return false; }


        return true;
    }


    public virtual void OnTick()
    {
        
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
        if(IsActive == false) return;
        IsActive = false;
        Owner?.ShutdownAbility(this);
    }


}


/*public bool CanActivateAbility(FAbilityHandle Handle)
    {
        if (!Abilities.TryGetValue(Handle, out GameAbility Ability))
        { return false; }

        if (Ability.RequiredTags.Count != 0
            && Tags.HasAllTags(Ability.RequiredTags) == false)
        { return false; }
        
        if (Ability.BlockedByTags.Count != 0
            && Tags.HasAnyTag(Ability.BlockedByTags))
        { return false; }
        
        return true;
    }
    */