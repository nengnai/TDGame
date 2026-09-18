using UnityEngine;
using System.Collections.Generic;


public partial class AbilitySystemComponent : MonoBehaviour
{
    /* Tag */
    public readonly GameTagContainer Tags = new();
    
    /* 循环专用 Buffer */
    private readonly List<GameAbilitySpec> TickAbilityBuffer = new();
    private readonly List<GameEffect> TickEffectBuffer = new();

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
}