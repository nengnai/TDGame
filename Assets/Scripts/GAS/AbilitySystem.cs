using UnityEngine;
using System.Collections.Generic;


public partial class AbilitySystemComponent : MonoBehaviour
{
    /* Tag */
    public readonly GameTagContainer Tags = new();
    
    /* 循环专用 Buffer */
    private readonly List<GameAbility> TickAbilityBuffer = new();
    private readonly List<GameEffect> TickEffectBuffer = new();

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
}