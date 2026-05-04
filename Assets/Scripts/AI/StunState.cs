using UnityEngine;

public class StunState : IState
{
    public void Enter(AI owner, IState LastState)
    {
        owner.stat.isStunned = true;
        owner.animator.CrossFade(UnitAnim.Anim_Stun, 0.1f);
        owner.stunTimer = owner.stunTime;
    }

    public void Update(AI owner)
    {
        if(owner.stunTimer > 0f)
        {
            owner.stunTimer -= Time.deltaTime;
        }
        else
        {
            owner.ChangeState(States.Idle);
            return;
        }
    }

    public void Exit(AI owner)
    {
        owner.stat.isStunned = false;
    }
}
