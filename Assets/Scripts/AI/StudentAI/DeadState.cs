using UnityEngine;

public class DeadState : IState
{
    public virtual void Enter(AI owner, IState LastState)
    {
        owner.animator.CrossFade(UnitAnim.Anim_Dead, 0.1f);
        owner.DeadTimer = 5f;
    }

    public virtual void Update(AI owner)
    {
        if(owner.DeadTimer > 0f)
        {
            owner.DeadTimer -= Time.deltaTime;
        }
        else
        {
            owner.Dead();
        }
    }

    public virtual void Exit(AI owner)
    {
        
    }
}
