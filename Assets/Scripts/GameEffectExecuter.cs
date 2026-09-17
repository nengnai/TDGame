using System;
using UnityEngine;

[Serializable] public abstract class GameEffectExecutor
{
    public virtual void OnApplied(GameEffect Effect)
    {
        
    }

    public virtual void OnRemoved(GameEffect Effect)
    {
        
    }

    public virtual void OnTick(GameEffect Effect)
    {
        
    }

    public virtual void OnPeriod(GameEffect Effect)
    {
        
    }

    public virtual void OnStackChanged(GameEffect Effect)
    {
        
    }
}



[Serializable] public class DebugExecutor : GameEffectExecutor
{
    public string Message;
    public override void OnApplied(GameEffect Effect)
    {
        Debug.Log($"[应用] {Message}");
    }

    public override void OnPeriod(GameEffect Effect)
    {
        Debug.Log($"[周期] {Message}");
    }
}