using UnityEngine;

public class AttackState : IState
{

    public void Enter(AI owner, IState LastState)
    {
        owner.stat.isFacingTarget = true;
        //Timer = 数值页面
    }

    

    public void Update(AI owner)
    {
        if(owner.stat.currentAmmo <= 0)
        {
            if(owner.attackTimer <= 0f)
            {
                owner.ChangeState(States.Reload);
            }
            
            return;
        }

        if(owner.currentTarget == null && owner.attackTimer <= 0f)
        {
            if (!owner.TargetFinding())
            {
                owner.ChangeState(States.attackEnd);
                return;
            }
        }
        else
        {
            Vector3 toTarget = owner.MainObject.position - owner.currentTarget.position;
            toTarget.y = 0f;
            if(toTarget.magnitude > owner.stat.range)
            {
                owner.currentTarget = null;
                owner.targetStat = null;
                if (!owner.TargetFinding())
            {
                owner.ChangeState(States.attackEnd);
                return;
            }
            }
            
        }

        if(owner.targetStat != null && owner.targetStat.currentHealth <= 0f)
        {
            owner.targetStat.isDead = true;
            owner.currentTarget = null;
            owner.targetStat = null;

            if (!owner.TargetFinding())
            {
                owner.ChangeState(States.attackEnd);
                return;
            }
        }



        if (owner.attackTimer <= 0f)
        {
            if(owner.stat.shootingType == ShootingType.Fullauto)
            {
                owner.shootingFunction.Shooting(owner.stat, owner.targetStat);
                owner.animator.CrossFade(UnitAnim.Anim_Attack, 0.05f);
                owner.attackTimer = owner.attackTime;
            }

            else if(owner.stat.shootingType == ShootingType.Burst)
            {
                if(owner.burstCount > 0)
                {
                    owner.shootingFunction.BurstShooting(owner, owner.stat, owner.targetStat);
                    owner.animator.CrossFade(UnitAnim.Anim_Attack, 0.05f);
                    owner.attackTimer = owner.attackTime;

                    if(owner.burstCount == 0)
                    {
                        owner.firingDelayTimer = owner.stat.firingDelay;
                    }
                }
                else if(owner.firingDelayTimer <= 0f)
                {
                    owner.burstCount = owner.stat.burstTime;
                }
            }
            
            else if(owner.stat.shootingType == ShootingType.Bolt)
            {
                owner.shootingFunction.Shooting(owner.stat, owner.targetStat);
                owner.animator.CrossFade(UnitAnim.Anim_Attack, 0.05f);
                owner.attackTimer = owner.attackTime + owner.boltTime;
            }
            
        }
    }

    

    public void Exit(AI owner)
    {
        owner.stat.isFacingTarget = false;
    }
}
