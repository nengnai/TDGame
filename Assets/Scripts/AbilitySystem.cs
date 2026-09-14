using System;
using System.Collections.Generic;
using UnityEngine;






public struct FAbilityHandle
{
    private readonly uint AbilityHandle;
    public FAbilityHandle(uint InAbilityHandle)
    {
        AbilityHandle = InAbilityHandle;
    }

    public override int GetHashCode()
    {
        return (int)AbilityHandle;
    }
}

public struct FEffectHandle
{
    private readonly uint EffectHandle;
    public FEffectHandle(uint InEffectHandle)
    {
        EffectHandle = InEffectHandle;
    }

    public override int GetHashCode()
    {
        return (int)EffectHandle;
    }
} 



public class AbilitySystem : MonoBehaviour
{
    private Dictionary<FAbilityHandle, GameAbilityInstance> Abilities = new Dictionary<FAbilityHandle, GameAbilityInstance>();
    private Dictionary<FGameTag, List<FAbilityHandle>> AbilitiesByTag = new Dictionary<FGameTag, List<FAbilityHandle>>();
    private List<GameAbilityInstance> OnActivateInstances = new List<GameAbilityInstance>();
    

    private Dictionary<FEffectHandle, GameEffectInstance> Effects = new Dictionary<FEffectHandle, GameEffectInstance>();
    private Dictionary<FGameTag, List<FEffectHandle>> EffectsByTag = new Dictionary<FGameTag, List<FEffectHandle>>();


    public GameTagContainer Tags;

    private uint AbilityID = 0;
    private uint EffectID = 0;

    void Awake()
    {
        Tags = new GameTagContainer();
    }


    #region 技能
    public FAbilityHandle AddAbility(GameAbility Ability)
    {
        FAbilityHandle handle = new FAbilityHandle(AbilityID++);
        
        GameAbilityInstance Instance = new GameAbilityInstance{Config = Ability, Owner = this};

        Abilities[handle] = Instance;

        if (Ability.AbilityTag.IsValid())
        {
            if(AbilitiesByTag.TryGetValue(Ability.AbilityTag, out List<FAbilityHandle> List))
            {
                List.Add(handle);
            }
            else
            {
                List<FAbilityHandle> NewList = new List<FAbilityHandle>();
                AbilitiesByTag.Add(Ability.AbilityTag, NewList);
                NewList.Add(handle);
            }
        }

        return handle;
    }


    public void RemoveAbility(FAbilityHandle Handle)
    {
        if (Abilities.TryGetValue(Handle, out GameAbilityInstance GameAbility))
        {
            if(GameAbility.Config.AbilityTag.IsValid()) AbilitiesByTag[GameAbility.Config.AbilityTag].Remove(Handle);
        }
        else
        {
            return;
        }

        Abilities.Remove(Handle);
    }




    public void ActivateAbility(FAbilityHandle Handle)
    {
        if(!CanActivateAbility(Handle)) return;

        if(Abilities.TryGetValue(Handle, out GameAbilityInstance Ability))
        {
            if(Ability.Config.Policy == GameAbility.InstantiationPolicy.OnActivate)
            {
                GameAbilityInstance TempInstance = new GameAbilityInstance{Config = Ability.Config, Owner = this};
                OnActivateInstances.Add(TempInstance);
                TempInstance.Activate();
                if(Ability.Config.AbilityTag.IsValid()) Tags.AddTag(Ability.Config.AbilityTag);
            }
            else
            {
                Ability.Activate();
                if(Ability.Config.AbilityTag.IsValid()) Tags.AddTag(Ability.Config.AbilityTag);
            }
        }
        else
        {
            return;
        }
    }


    public void CancelAbility(FAbilityHandle Handle)
    {
        if(Abilities.TryGetValue(Handle, out GameAbilityInstance Ability))
        {
            if(Ability.Config.Policy == GameAbility.InstantiationPolicy.OnActivate)
            {
                for(int i = OnActivateInstances.Count - 1; i >= 0; i--)
                {
                    if (OnActivateInstances[i].Config == Ability.Config)
                    {
                        OnActivateInstances[i].Cancel();
                        OnActivateInstances.RemoveAt(i);
                    }
                }
            }
            else
            {
                Ability.Cancel();
            }

            if(Ability.Config.AbilityTag.IsValid()) Tags.RemoveTag(Ability.Config.AbilityTag);
        }
        else
        {
            return;
        }
    }





    public bool CanActivateAbility(FAbilityHandle Handle)
    {
        if(Abilities.TryGetValue(Handle, out GameAbilityInstance Ability))
        {
            if(Tags.HasAllTags(Ability.Config.RequiredTags) == true && Tags.HasAnyTag(Ability.Config.BlockedTags) == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }


    public void AbilityInstanceEnd(GameAbilityInstance Instance)
    {
        OnActivateInstances.Remove(Instance);
        Instance.Cancel();
    }

    #endregion



    
    #region 效果
    public FEffectHandle AddEffect(GameEffect Effect)
    {
        FEffectHandle handle = new FEffectHandle(EffectID++);

        GameEffectInstance Instance = new GameEffectInstance{Config = Effect, Owner = this};

        Effects[handle] = Instance;

        if (Effect.EffectTag.IsValid())
        {
            if(EffectsByTag.TryGetValue(Effect.EffectTag, out List<FEffectHandle> List))
            {
                List.Add(handle);
            }
            else
            {
                List<FEffectHandle> NewList = new List<FEffectHandle>();
                EffectsByTag.Add(Effect.EffectTag, NewList);
                NewList.Add(handle);
            }
        }

        return handle;
    }




    public void RemoveEffect(FEffectHandle Handle)
    {
        if(Effects.TryGetValue(Handle, out GameEffectInstance GameEffect))
        {
            if(GameEffect.Config.EffectTag.IsValid()) EffectsByTag[GameEffect.Config.EffectTag].Remove(Handle);
        }
        else
        {
            return;
        }

        Effects.Remove(Handle);
    }


    public FEffectHandle ApplyEffect(GameEffect Config)
    {

        GameEffectInstance ExistInstance = null;
        FEffectHandle ExistingHandle = new FEffectHandle(0);


        if (Config.EffectTag.IsValid())
        {
            if(EffectsByTag.TryGetValue(Config.EffectTag, out List<FEffectHandle> handles))
            {
                foreach(FEffectHandle Handle in handles)
                {
                    if(Effects.TryGetValue(Handle, out GameEffectInstance InstanceHandle))
                    {
                        if(InstanceHandle.Config == Config)
                        {
                            ExistInstance = InstanceHandle;
                            ExistingHandle = Handle;
                            break;
                        }   
                    }
                }
            }
        }

        if(ExistInstance != null)
        {
            if(ExistInstance.CurrentStack < Config.MaxStacks)
            {
                ExistInstance.CurrentStack++;
            }

            if(Config.DurPolicy == GameEffect.DurationPolicy.Duration)
            {
                ExistInstance.RefreshDuration(ExistingHandle);
            }

            return ExistingHandle;
        }
        else
        {
            FEffectHandle NewHandle = AddEffect(Config);

            if(Effects.TryGetValue(NewHandle, out GameEffectInstance NewInstance))
            {
                if(Config.DurPolicy == GameEffect.DurationPolicy.Instant)
                {  
                    RemoveEffect(NewHandle);
                }
                else
                {
                    NewInstance.InitializeTimers(NewHandle);
                }
            }

            return NewHandle;
        }
    }



    #endregion

}



public class AbilityTimeManager
{
    private List<FTimerHandle> handles = new List<FTimerHandle>();

    public FTimerHandle AddTimer(float CostTime, bool IsLoop, bool IsUnscaledTime, Action CallBack)
    {
        FTimerHandle handle = TimerSubsystem.GetSubsystem().AddTimer(CostTime, IsLoop, IsUnscaledTime, CallBack);
        handles.Add(handle);
        return handle;
    }


    public void RemoveTimer(FTimerHandle Handle)
    {
        TimerSubsystem.GetSubsystem().RemoveTimer(Handle);
        handles.Remove(Handle);
    }

    public void ClearAll()
    {
        foreach(FTimerHandle Handle in handles)
        {
            TimerSubsystem.GetSubsystem().RemoveTimer(Handle);
        }
        handles.Clear();
    }

    

}




public class EffectTimeManager
{
    
}
