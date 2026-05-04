using UnityEngine;

public class IdleReloadState : IState
{
    public void Enter(AI owner, IState LastState)
    {
        owner.animator.CrossFade(UnitAnim.Anim_IdleReloading, 0.1f);
        owner.idleReloadTimer = owner.idleReloadTime;
        owner.stat.isReloading = true;
    }

    public void Update(AI owner)
    {
        if(owner.idleReloadTimer > 0f)
        {
            owner.idleReloadTimer -= Time.deltaTime;
        }
        else
        {
            owner.stat.currentAmmo = owner.stat.fullAmmo;
            owner.ChangeState(States.Idle);
            return;
        }
    }

    public void Exit(AI owner)
    {
        owner.stat.isReloading = false;
    }
}
