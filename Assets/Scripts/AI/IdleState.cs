
using UnityEngine;

public class IdleState : IState
{
    public void Enter(AI owner, IState LastState)
    {
        owner.animator.CrossFade(UnitAnim.Anim_Idle, 0.1f);
        owner.detectTimer = 0f;
        owner.stat.isIdling = true;
        Debug.Log("进入待机状态");
    }

    public void Update(AI owner)
    {
        if(owner.detectTimer > 0f)
        {
            owner.detectTimer -= Time.deltaTime;
        }
        else
        {
            if(owner.TargetFinding())
            {
                owner.ChangeState(States.attackStart);
                return;
            }
            else
            {
                if(owner.stat.currentAmmo < owner.stat.fullAmmo)
                {
                    if (owner.stat.HasIdleReloadAnim)
                    {
                        owner.ChangeState(States.IdleReload);
                        return;
                    }
                    else
                    {
                        owner.ChangeState(States.Reload);
                        return;
                    }
                }
            }
            owner.detectTimer = owner.detectTime;
        }
    }

    public void Exit(AI owner)
    {
        owner.stat.isIdling = false;
    }
}
