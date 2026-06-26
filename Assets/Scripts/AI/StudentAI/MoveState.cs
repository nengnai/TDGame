
using UnityEngine;

public class MoveState : IState
{
    public virtual void Enter(AI owner, IState LastState)
    {
        owner.animator.CrossFade(UnitAnim.Anim_Moving, 0.1f);
        Debug.Log("进入移动状态");
    }

    public virtual void Update(AI owner)
    {
        if (owner.PathAI.reachedEndOfPath)
        {
            owner.ChangeState(owner.idleState);
            
            return;
        }
        
    }

    public virtual void Exit(AI owner)
    {
        
    }
}
