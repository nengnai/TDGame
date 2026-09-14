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

    public List<FGameTag> RequiredTags;
    public List<FGameTag> BlockedTags;



    public virtual void Activate(GameAbilityInstance Instance)
    {
        
    }

    public virtual void Cancel(GameAbilityInstance Instance)
    {
        
    }

}





public class GameAbilityInstance
{
    public GameAbility Config;
    public AbilitySystem Owner;
    public AbilityTimeManager TimeManager = new AbilityTimeManager();

    public void Activate()
    {
        Config.Activate(this);
    }

    public void Cancel()
    {
        Config.Cancel(this);
        TimeManager.ClearAll();
    }

    public void EndAbility()
    {
        Owner.AbilityInstanceEnd(this);
    }
}