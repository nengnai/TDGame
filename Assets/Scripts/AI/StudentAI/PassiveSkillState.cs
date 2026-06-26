using UnityEngine;

public class PassiveSkillState : IState
{
    public virtual void Enter(AI owner, IState LastState)
    {
        owner.stat.isPassiveSkilling = true;
    }

    public virtual void Update(AI owner)
    {
        
    }
    
    public virtual void Exit(AI owner)
    {
        owner.stat.isPassiveSkilling = false;
    }
}
