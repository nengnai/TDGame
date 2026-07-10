using UnityEngine;

public class AttackStartState : IState
{

    public void Enter(AI owner, IState LastState)
    {
        owner.animator.CrossFade(UnitAnim.Anim_AttackStart, 0.1f);
        owner.attackStartTimer = owner.attackStartTime;
        owner.stat.isFacingTarget = true;
    }

    public void Update(AI owner)
    {
        if(owner.attackStartTimer > 0f)
        {
            owner.attackStartTimer -= Time.deltaTime;
        }
        else
        {
            owner.ChangeState(owner.attackState);
            return;
        }
    }

    public void Exit(AI owner)
    {
        owner.stat.isFacingTarget = false;
    }
}
