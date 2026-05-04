using UnityEngine;

public class AttackEndState : IState
{

    public void Enter(AI owner, IState LastState)
    {
        owner.animator.CrossFade(UnitAnim.Anim_AttackEnd, 0.1f);
        owner.attackEndTimer = owner.attackEndTime;
        
    }

    public void Update(AI owner)
    {
        if(owner.attackEndTimer > 0f)
        {
            owner.attackEndTimer -= Time.deltaTime;
        }
        else
        {
            owner.ChangeState(States.Idle);
            return;
        }
    }

    public void Exit(AI owner)
    {
        
    }
}
