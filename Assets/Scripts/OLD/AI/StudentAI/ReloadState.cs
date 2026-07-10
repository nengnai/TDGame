using UnityEngine;

public class ReloadState : IState
{
    public virtual void Enter(AI owner, IState LastState)
    {
        owner.animator.CrossFade(UnitAnim.Anim_Reloading, 0.1f);
        owner.reloadTimer = owner.reloadTime;
        owner.stat.isReloading = true;
    }

    public virtual void Update(AI owner)
    {
        if(owner.reloadTimer > 0f)
        {
            owner.reloadTimer -= Time.deltaTime;
        }
        else
        {
            owner.stat.currentAmmo = owner.stat.fullAmmo;
            owner.ChangeState(owner.attackState);
            return;
        }
    }

    public virtual void Exit(AI owner)
    {
        owner.stat.isReloading = false;
    }
}
