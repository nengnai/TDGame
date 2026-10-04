using UnityEngine;
using System.Collections.Generic;
using System;


public partial class AbilitySystemComponent : MonoBehaviour
{
    /* Tag */
    public readonly GameTagContainer Tags = new();
    
    /* 循环专用 Buffer */
    private readonly List<GameAbilitySpec> TickAbilityBuffer = new();
    private readonly List<GameEffect> TickEffectBuffer = new();

    private readonly Dictionary<FGameTag, Action> AnimDict = new();

    private void Update()
    {
        TickAbilityBuffer.Clear();
        GetActiveSpecs(TickAbilityBuffer);
        foreach (GameAbilitySpec Spec in TickAbilityBuffer)
        {
            if (!Spec.IsActive || !Spec.Instance) continue;
            Spec.Instance.OnTick();
        }

        TickEffectBuffer.Clear();
        TickEffectBuffer.AddRange(EffectInstances.Values);
        foreach(var Target in TickEffectBuffer)
        {
            Target.OnTick();
        }
    }


    public void RegisterAnimEvent(FGameTag Tag, Action Callback)
    {
        if(!AnimDict.ContainsKey(Tag)) AnimDict[Tag] = null;

        AnimDict[Tag] += Callback;
    }

    public void UnRegisterAnimEvent(FGameTag Tag, Action Callback)
    {
        if(!AnimDict.ContainsKey(Tag)) return;
        AnimDict[Tag] -= Callback;
        if(AnimDict[Tag] == null) AnimDict.Remove(Tag);
    }


    public void UseAnimEvent(FGameTag Tag)
    {
        if(AnimDict.TryGetValue(Tag, out Action Callback)) Callback?.Invoke();
    }
    
}