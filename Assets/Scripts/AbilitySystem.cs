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


public partial class AbilitySystemComponent : MonoBehaviour
{
    /* Tag */
    public readonly GameTagContainer Tags = new();
    
    
    /* 技能数据存放 */
    private readonly Dictionary<FAbilityHandle, GameAbility> HandleToAbilitie = new();
    private readonly Dictionary<FGameTag, List<FAbilityHandle>> AbilitiesByTag = new();
    private readonly List<GameAbility> ActivateAbilities = new();
    private readonly List<GameAbility> TickAbilityBuffer = new();
    private uint AbilityID;
    


    private void Update()
    {
        TickAbilityBuffer.Clear();
        TickAbilityBuffer.AddRange(ActivateAbilities);
        foreach(var Target in TickAbilityBuffer)
        {
            // 此处无需判定是否为激活,因为在 Buffer 中的值均为已激活
            Target.OnTick();
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

        GameAbility Ability;

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
        HandleToAbilitie[Handle] = Ability;

        if (Config.AbilityTag.IsValid())
        {
            if(!AbilitiesByTag.TryGetValue(Config.AbilityTag, out var List))
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
        if(!HandleToAbilitie.TryGetValue(Handle, out GameAbility Ability))
        { return; }

        EndAbility(Ability);
        HandleToAbilitie.Remove(Handle);

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

    
    // 尝试激活技能时调用
    public void ActivateAbility(FAbilityHandle Handle)
    {
        if(!HandleToAbilitie.TryGetValue(Handle, out GameAbility Ability)) return;
        
        if(Ability.AbilityTag.IsValid() && Tags.HasTag(Ability.AbilityTag)) return;

        if(!Ability.CanActivate()) return;
        
        if(Ability.Policy != GameAbility.InstantiationPolicy.OnActivate && Ability.IsActive == true) return;

        //@todo:优化判定逻辑,以下为临时逻辑
        if(Ability.CancelTags.Count > 0)
        {
            List<GameAbility> CheckList = new();
            foreach (GameAbility Active in ActivateAbilities)
            {
                foreach(FGameTag Tag in Active.OwnTags)
                {
                    foreach(FGameTag CancelTag in Ability.CancelTags)
                    {
                        if (Tag == CancelTag)
                        {
                            CheckList.Add(Active);
                            goto FinishTagMatch;
                        }
                    }
                }
                FinishTagMatch: ;
            }

            foreach (GameAbility Target in CheckList)
            {
                EndAbility(Target);
            }
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
        foreach(FGameTag Tag in AbilityToActivate.OwnTags)
        {
            Tags.AddTag(Tag);
        }
        
        
        AbilityToActivate.Activate();
    }
    
    
    
    
    
    // 当用户尝试取消技能时调用(不代表技能结束,仅做为请求)
    public void CancelAbility(FAbilityHandle Handle)
    {
        if (!HandleToAbilitie.TryGetValue(Handle, out GameAbility Value))
        { return; }
        
        Value.Cancel();
    }
    
    // 技能结束时调用
    public void EndAbility(GameAbility Instance)
    {
        if(!ActivateAbilities.Contains(Instance))
        { return; }
        Instance.EndAbility();
    }
    
    
    // GA 结束之后自动调用的回调
    public void ShutdownAbility(GameAbility Instance)
    {
        Instance.IsActive = false;
        
        foreach(FGameTag Tag in Instance.OwnTags)
        {
            Tags.RemoveTag(Tag);
        }
        ActivateAbilities.Remove(Instance);

        if(Instance.Policy == GameAbility.InstantiationPolicy.OnActivate) Destroy(Instance);
    }
    
}