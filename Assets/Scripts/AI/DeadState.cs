using UnityEngine;

public class DeadState : IState
{
    public void Enter(AI owner, IState LastState)
    {
        owner.animator.CrossFade(UnitAnim.Anim_Dead, 0.1f);
        owner.DeadTimer = 5f;
    }

    public void Update(AI owner)
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

    public void Exit(AI owner)
    {
        
    }
}
