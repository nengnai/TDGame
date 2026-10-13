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
    // @todo:修复层级对修改结果的影响
    public FGameTag TargetAttribute;
    public float Value;


    public override void Apply(CharacterStats Target, GameEffect Effect)
    {
        Target.AddAddModifier(TargetAttribute, Value);
    }

    public override void Remove(CharacterStats Target, GameEffect Effect)
    {
        Target.RemoveAddModifier(TargetAttribute, Value);
    }

    public override void OnStackChanged(CharacterStats Target, GameEffect Effect)
    {
        
    }
}