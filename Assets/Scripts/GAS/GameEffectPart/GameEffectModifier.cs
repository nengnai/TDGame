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
    public FGameTag TargetAttribute;
    public float Value;

    public override void Apply(CharacterStats Target, GameEffect Effect)
    {
        float Current = Target.GetBaseValue(TargetAttribute);
        Target.SetBaseStat(TargetAttribute, Current + Value);
        
    }

    public override void Remove(CharacterStats Target, GameEffect Effect)
    {
        
    }

    public override void OnStackChanged(CharacterStats Target, GameEffect Effect)
    {
        Apply(Target, Effect);
    }
}



[Serializable] public class ModifyModifier : GameEffectModifier
{
    public FGameTag TargetAttribute;
    public float Value;


    public override void Apply(CharacterStats Target, GameEffect Effect)
    {
        ValueChange VC = new ValueChange{Add = Value};
        Target.AddModifier(TargetAttribute, VC);
    }

    public override void Remove(CharacterStats Target, GameEffect Effect)
    {
        ValueChange VC = new ValueChange {Add = Value};
        Target.RemoveModifier(TargetAttribute, VC);
        
    }

    public override void OnStackChanged(CharacterStats Target, GameEffect Effect)
    {
        
    }
}