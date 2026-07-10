using UnityEngine;

public class AttackState : IState
{

    public virtual void Enter(AI owner, IState LastState)
    {
        owner.stat.isFacingTarget = true;
        //Timer = 数值页面
    }

    

    public virtual void Update(AI owner)
    {
        if(owner.stat.currentAmmo <= 0)            //子弹打空 并且开枪冷却结束后进行换弹
        {
            if(owner.attackTimer <= 0f)
            {
                owner.ChangeState(owner.reloadState);
            }
            
            return;
        }

        if(owner.currentTarget == null && owner.attackTimer <= 0f)       //目标死亡 并且开枪冷却结束 执行收枪
        {
            if (!owner.TargetFinding())
            {
                owner.ChangeState(owner.attackEndState);
                return;
            }
        }
        else                                   //目标存活 检查是否超出射击范围
        {
            Vector3 toTarget = owner.MainObject.position - owner.currentTarget.position;
            toTarget.y = 0f;
            if(toTarget.magnitude > owner.stat.range)            //超出射击范围 再次检查范围内是否有其他可射击目标
            {
                owner.currentTarget = null;
                owner.targetStat = null;
                if (!owner.TargetFinding())                      //如果没有其他目标则收枪 否则将新目标数据覆盖到当前目标上
                {
                    owner.ChangeState(owner.attackEndState);
                    return;
                }
            }
            
        }

        if(owner.targetStat != null && owner.targetStat.currentHealth <= 0f)    //目标血量归零则判定死亡
        {
            owner.targetStat.isDead = true;
            owner.currentTarget = null;
            owner.targetStat = null;

            if (!owner.TargetFinding())
            {
                owner.ChangeState(owner.attackEndState);
                return;
            }
        }



        if (owner.attackTimer <= 0f)
        {
            if(owner.stat.shootingType == ShootingType.Fullauto)
            {
                FullautoShooting(owner);
            }

            else if(owner.stat.shootingType == ShootingType.Burst)
            {
                BurstShooting(owner);
            }
            
            else if(owner.stat.shootingType == ShootingType.Bolt)
            {
                BoltShooting(owner);
            }
            
        }
    }

    

    public virtual void Exit(AI owner)
    {
        owner.stat.isFacingTarget = false;
    }

    public void FullautoShooting(AI owner)
    {
        owner.shootingFunction.Shooting(owner.stat, owner.targetStat);
        owner.animator.CrossFade(UnitAnim.Anim_Attack, 0.05f);
        owner.attackTimer = owner.attackTime;
    }

    public void BurstShooting(AI owner)
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

    public void BoltShooting(AI owner)
    {
        owner.shootingFunction.Shooting(owner.stat, owner.targetStat);
        owner.animator.CrossFade(UnitAnim.Anim_Attack, 0.05f);
        owner.attackTimer = owner.attackTime + owner.boltTime;
    }

}
