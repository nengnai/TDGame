using System;
using UnityEngine;


[Serializable] public abstract class GameEffectModifier
{
    public abstract void Apply(CharacterStats Target, GameEffect Effect);
    public abstract void Remove(CharacterStats Target, GameEffect Effect);
    public abstract void OnStackChanged(CharacterStats Target, GameEffect Effect);
}







[Serializable] public class AddModifier : GameEffectModifier
{
    public FName TargetAttribute;
    public float Value;

    public override void Apply(CharacterStats Target, GameEffect Effect)
    {
        if (Target.HasAdditionalStat(TargetAttribute))
        {
            float OldValue = Target.GetAdditionalStat(TargetAttribute);
            Target.AdditionalStats[TargetAttribute] = OldValue + Value;
        }
        
    }

    public override void Remove(CharacterStats Target, GameEffect Effect)
    {
        
    }

    public override void OnStackChanged(CharacterStats Target, GameEffect Effect)
    {
        
    }
}



[Serializable] public class ModifyModifier : GameEffectModifier
{
    public FName TargetAttribute;
    public float Value;

    [NonSerialized] public float OriginalValue;
    public override void Apply(CharacterStats Target, GameEffect Effect)
    {
        if (Target.HasAdditionalStat(TargetAttribute))
        {
            OriginalValue = Target.AdditionalStats[TargetAttribute];
            Target.AdditionalStats[TargetAttribute] = OriginalValue + Value;
        }
        
    }

    public override void Remove(CharacterStats Target, GameEffect Effect)
    {
        if (Target.HasAdditionalStat(TargetAttribute))
        {
            Target.AdditionalStats[TargetAttribute] = OriginalValue;
        }
        
    }


    public override void OnStackChanged(CharacterStats Target, GameEffect Effect)
    {
        
    }
}