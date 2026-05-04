using Pathfinding;
using UnityEngine;

public class AI : MonoBehaviour
{
    [Header("引用")]
    public CharacterStat stat;
    public Animator animator;
    public FollowerEntity PathAI;
    public Transform MainObject;
    public ShootingFunction shootingFunction;





    [Header("状态相关")]
    IState currentState;

    void Awake()
    {
        shootingFunction = new ShootingFunction();
    }

    void Start()
    {
        attackStartTime = stat.AttackStartTime;
        attackTime = stat.AttackTime;
        boltTime = stat.boltTime;
        attackEndTime = stat.AttackEndTime;
        reloadTime = stat.ReloadTime;
        idleReloadTime = stat.IdleReloadTime;
        
        firingDelayTimer = stat.firingDelay;
        burstCount = stat.burstTime;
    }

    public void ChangeState(IState TargetState)
    {
        IState lastState = currentState;
        currentState?.Exit(this);
        currentState = TargetState;
        currentState?.Enter(this, lastState);
    }

    void Update()
    {
        if(attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }
        if(firingDelayTimer > 0f)
        {
            firingDelayTimer -= Time.deltaTime;
        }

        if(stat.currentHealth <= 0f) stat.isDead = true;
        if (stat.isDead && currentState != States.Dead)
        {
            ChangeState(States.Dead);
            return;
        }


        if (stat.isStunned && currentState != States.Stun)
        {
            ChangeState(States.Stun);
        }
  

        if(PathAI.hasPath && !PathAI.reachedDestination && currentState != States.Stun)
        {
            if(currentState != States.Move)
            {
                ChangeState(States.Move);
            }
        }

        if (stat.isFacingTarget && currentTarget != null)
        {
            BattleScripts.FaceTarget(MainObject, currentTarget.position, turnSpeed);
        }


        currentState?.Update(this);
    }



    [Header("计时器")]
    public float detectTime;
    [HideInInspector] public float attackStartTime, attackTime, boltTime, attackEndTime, reloadTime, idleReloadTime, stunTime;
    [HideInInspector] public float detectTimer;               //索敌检测间隔
    [HideInInspector] public float attackStartTimer;
    [HideInInspector] public float attackTimer;               //开火用时（其实就是相当于攻击间隔）
    [HideInInspector] public float firingDelayTimer;          //爆发模式下打出一组后的等待时间
    [HideInInspector] public float attackEndTimer;
    [HideInInspector] public float reloadTimer;
    [HideInInspector] public float idleReloadTimer;
    [HideInInspector] public float stunTimer;                 //不加延迟弹药检查了 弹药不满直接换弹
    [HideInInspector] public float DeadTimer;
    public int burstCount;



    [Header("目标相关")]
    [HideInInspector] public Transform currentTarget;
    [HideInInspector] public CharacterStat targetStat;


    


    [Header("配置")]
    public float turnSpeed;











    public bool TargetFinding()
    {
        if(!BattleScripts.TryFindTarget(MainObject.position, stat.range, stat.targetMask, out Transform newTarget, out CharacterStat newtargetStat))
        {
            return false;
        }
        currentTarget = newTarget;
        targetStat = newtargetStat;
        return true;
    }

    public void Dead()
    {
        Destroy(MainObject.gameObject);
    }

}
