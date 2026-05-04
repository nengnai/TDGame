using Unity.VisualScripting;
using UnityEngine;

public class MoveState : IState
{
    public void Enter(AI owner, IState LastState)
    {
        owner.animator.CrossFade(UnitAnim.Anim_Moving, 0.1f);
        Debug.Log("进入移动状态");
    }

    public void Update(AI owner)
    {
        if (owner.PathAI.reachedEndOfPath)
        {
            owner.ChangeState(States.Idle);
            
            return;
        }
        
    }

    public void Exit(AI owner)
    {
        
    }
}
