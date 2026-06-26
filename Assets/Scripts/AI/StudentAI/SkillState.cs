using UnityEngine;

public class SkillState : IState
{
    public virtual void Enter(AI owner, IState LastState)
    {
        owner.stat.isSkilling = true;
    }

    public virtual void Update(AI owner)
    {
        
    }
    
    public virtual void Exit(AI owner)
    {
        owner.stat.isSkilling = false;
    }
}
