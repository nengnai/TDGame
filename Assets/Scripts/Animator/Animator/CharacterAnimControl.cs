using UnityEngine;
using UnityEngine.AI;



public enum ActionType
{
    Idle,
    Attack,
    Move,
    Stun,
    Reload
}
public class CharacterAnimControl : MonoBehaviour
{
    public Transform MainObject;
    public NavMeshAgent agent;
    public CharacterStat stat;
    public LayerMask TargetMask;
    public float turnSpeed;
    public float detectDur;                //待机状态下侦测敌人间隔时间
    float detectTime;
    public float AmmoCheckDur;              //待机时延迟检查弹药时间
    float AmmoCheckTimer;

    Collider[] hitColliders = new Collider[200];


    ActionType currentAction;
    ActionType lastAction;




    #region 动画相关
        
    
    public Animator animator;
    static readonly int Anim_Idle = Animator.StringToHash("Idle");
    static readonly int Anim_Moving = Animator.StringToHash("Moving");
    static readonly int Anim_MoveEnd = Animator.StringToHash("Move_End");
    static readonly int Anim_AttackStart = Animator.StringToHash("Attack_Start");
    static readonly int Anim_AttackEnd = Animator.StringToHash("Attack_End");
    static readonly int Anim_Attacking = Animator.StringToHash("Attacking");
    static readonly int Anim_Reloading = Animator.StringToHash("Reloading");
    


        
    [Header("动画1倍用时")]
    public float MoveEnd;
    public float Attack_WindUp;
    public float Attack_WindDown;
    public float Reloading;

    float MoveEndTimer, AttackTimer, AttackWindupTimer, AttackWinddownTimer, ReloadTimer, StunTimer;
    public float StunDur;


    [Header("最适配移动速度")]
    public float MoveSpeed;







    


    #endregion


    






    #region 敌人相关
        
    
    Transform currentTarget;
    CharacterStat TargetStat;
    Vector3 StudentPosition;
    Vector3 currentTargetPosition;
    float distance;
    bool isTargetRunaway;

    
    #endregion




    void Awake()
    {
        currentAction = ActionType.Idle;
        MoveEndTimer = AttackTimer = AttackWindupTimer = AttackWinddownTimer = ReloadTimer = StunTimer = 0f;
        
    }


    





    void Update()
    {
        if(currentTarget == null)                       //目标挂了的话目标属性也清理一下
        {
            TargetStat = null;
        }
        else
        {
            TargetRunaway();
        }


        if(stat.isFacingTarget && currentTarget != null)
        {
            FaceTarget();
        }
        








        if (agent.hasPath)
        {
            stat.isMoving = true;
        }
        else
        {
            stat.isMoving = false;
        }

        

        if (stat.isStunned)                                 //眩晕优先级最高
        {
            SwitchTo(ActionType.Stun);
            return;
        }

        
        if(stat.isMoving && !stat.isStunned && currentAction != ActionType.Move && !stat.isShooting)
        {
            SwitchTo(ActionType.Move, currentAction);
            return;
        }













        switch (currentAction) 
        {
            case ActionType.Stun:
            UpdateStun();
            break;

            case ActionType.Idle:
            UpdateIdle();
            break;

            case ActionType.Reload:
            UpdateReload();
            break;

            case ActionType.Move:
            UpdateMove();
            break;

            case ActionType.Attack:
            UpdateAttack();
            break;
        }




    }


    void UpdateStun()
    {
        MoveEndTimer = 0f;                                  //因被眩晕而回到idle的情况下不需要做任何前摇
        AttackWinddownTimer = 0f;
        AttackWindupTimer = 0f; 
        AttackTimer = 0f;
        stat.isFacingTarget = false;
        stat.isShooting = false;
        agent.isStopped = true;                             //先暂停agent以防继续移动
        agent.ResetPath();                             //被眩晕后会忘记之前的路径
        if(StunTimer > 0)
        {
            StunTimer -= Time.deltaTime;
        }
        else                                                //眩晕结束后解开移动限制
        {
            agent.isStopped = false;                              
            stat.isStunned = false;
            SwitchTo(ActionType.Idle, ActionType.Stun);
            Debug.Log("结束眩晕");
        }

    }



    void UpdateIdle()
    {
        

        if(lastAction == ActionType.Move && MoveEndTimer > 0f)                                  //前摇动画
        {
            MoveEndTimer -= Time.deltaTime;
            return;
        }
        else if(lastAction == ActionType.Attack && AttackWinddownTimer > 0f)
        {
            AttackWinddownTimer -= Time.deltaTime;
            return;
        }

        
            
        if(stat.currentAmmo <= 0)                        //立刻检查弹药是否为0 是的话就换弹 不执行下面的
        {
            SwitchTo(ActionType.Reload, ActionType.Idle);
            return;
        }

        animator.CrossFade(Anim_Idle, 0.1f);
        
        if(AmmoCheckTimer > 0f)                           //不是0就倒计时 同时进行下面的目标寻找
        {
            AmmoCheckTimer -= Time.deltaTime;
        }
        else                                             //如果弹药不满并且没找到敌人 换弹 不执行下面的
        {
            if(stat.currentAmmo != stat.fullAmmo)
            {
                SwitchTo(ActionType.Reload, ActionType.Idle);
                return;
            }
        }






        if(detectTime <= 0f)                         //立刻寻找一次目标 找到就攻击 没找到就重置时间继续等
        {
            TryFindTarget();
            if(currentTarget != null)
            {
                SwitchTo(ActionType.Attack, ActionType.Idle);
                return;
            }
            else
            {
                detectTime = detectDur;
            }
        }
        else
        {
            detectTime -= Time.deltaTime;
        }


        
    }



    void UpdateMove()                          //应该只需要检测一下有没有到目的地
    {
        
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            SwitchTo(ActionType.Idle, ActionType.Move);
            animator.CrossFade(Anim_MoveEnd, 0.1f);
            return;
        }
    }





    void UpdateAttack()
    {
        
        if(AttackWindupTimer > 0f)
        {
            AttackWindupTimer -= Time.deltaTime;
            return;
        }
        
        
        if(stat.currentAmmo <= 0 && !stat.isShooting)
        {
            
            SwitchTo(ActionType.Reload, ActionType.Attack);
            Debug.Log("战斗中换弹");
            return;
        }

        if(currentTarget == null || TargetStat == null || TargetStat.isDead || isTargetRunaway)
        {

            stat.isShooting = false;
            stat.isFacingTarget = false;

            currentTarget = null;
            TargetStat = null;

            TryFindTarget();

            if(currentTarget == null)
            {
                SwitchTo(ActionType.Idle, ActionType.Attack);
                return;
            }
        }
        
        AttackTimer -= Time.deltaTime;
        
        bool Shooting = AttackTimer > 0f;

        if (AttackTimer <= 0f)
        {
            if(stat.currentAmmo > 0)
            {
                Shoot();
                AttackTimer = stat.firingInterval;
                Shooting = true;
            }
        }
        stat.isShooting = Shooting;
    }
















    void UpdateReload()
    {
        if(ReloadTimer > 0f)             //等换弹动画播放完毕
        {
            ReloadTimer -= Time.deltaTime;
        }
        else
        {
            stat.currentAmmo = stat.fullAmmo;
            if(currentTarget != null && TargetStat != null && !TargetStat.isDead && !isTargetRunaway)    //如果目标没被销毁 目标没死以及目标没跑出距离 就继续攻击目标
            {
                SwitchTo(ActionType.Attack, ActionType.Reload);
                return;
            }
            else               //否则先立刻检查附近有没有其他可攻击目标 有就继续攻击 没有就去待机
            {
                TryFindTarget();
                if(currentTarget != null)
                {
                    SwitchTo(ActionType.Attack, ActionType.Reload);
                    return;
                }
                else
                {
                    SwitchTo(ActionType.Idle, ActionType.Reload);
                    animator.CrossFade(Anim_AttackEnd, 0.1f);
                    return;
                }
            }
        }
    }






    void SwitchTo(ActionType actionType, ActionType? lastType = null)
    {
        
        if(lastType == ActionType.Attack)
        {
            stat.isShooting = false;
            stat.isFacingTarget = false;

            
        }



        if(currentAction == actionType) return;
        lastAction = lastType ?? currentAction;
        currentAction = actionType;

        switch (actionType)
        {
            case ActionType.Move:                         //目前应该不需要做什么
            stat.isFacingTarget = false;
            animator.CrossFade(Anim_Moving, 0.1f);
            break; 

            case ActionType.Idle:
            stat.isShooting = false;
            stat.isFacingTarget = false;
            detectTime = 0f;
            AmmoCheckTimer = AmmoCheckDur;
                if(lastAction == ActionType.Attack)
                {
                    AttackWinddownTimer = Attack_WindDown;
                }
                else if(lastAction == ActionType.Move)
                {
                    MoveEndTimer = MoveEnd;   
                }
                else
                {
                    MoveEndTimer = 0f;
                    AttackWinddownTimer = 0f;
                }
            break;

            case ActionType.Stun:                          //重置眩晕倒计时
            //播放眩晕动画
            stat.isFacingTarget = false;
            StunTimer = StunDur;
            break;

            case ActionType.Reload:                       //重置换弹动画倒计时
            stat.isShooting = false;
            stat.isFacingTarget = false;
            animator.CrossFade(Anim_Reloading, 0.1f);
            ReloadTimer = stat.reloadTime;
            break;

            case ActionType.Attack:                             //重置前后摇和攻击倒计时
            stat.isFacingTarget = true;
            animator.CrossFade(Anim_AttackStart, 0.1f);
            AttackWindupTimer = Attack_WindUp;
            AttackTimer = 0f;
            break;
        }
    }       



    void Shoot()
    {
        if(currentTarget == null) return;
        
        if(agent.hasPath) agent.ResetPath();

        animator.CrossFade(Anim_Attacking, 0.05f);

        AttacktheTarget();
        stat.currentAmmo -= 1;
    }



    void TargetRunaway()
    {
        if(currentTarget != null)
        {
            StudentPosition = new Vector3(transform.position.x, 0f, transform.position.z);              //持续更新敌我双方位置
            currentTargetPosition = new Vector3(currentTarget.position.x, 0f, currentTarget.position.z);    

            distance = Vector3.Distance(StudentPosition, currentTargetPosition);
            isTargetRunaway = distance > stat.range;                 //随时更新检查目标是否超过自身射击距离
        }
    }





    public void TryFindTarget()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, stat.range, hitColliders, TargetMask);

        if(count == 0)
        {
            currentTarget = null;
            TargetStat = null;
            return;
        }

        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        Vector3 myPosition = new Vector3(transform.position.x, 0f, transform.position.z);

        for(int i = 0; i < count; i++)                    //一直检测到离自己最近的目标
        {
            Collider col = hitColliders[i];
            if(col.transform == transform) continue;
            CharacterStat targetStat = col.GetComponentInChildren<CharacterStat>();
            if(targetStat != null && targetStat.isDead) continue;

            Vector3 targetPosition = new Vector3(col.transform.position.x, 0f, col.transform.position.z);
            float distance = Vector3.Distance(myPosition, targetPosition);

            if(distance < closestDistance)
            {   
                closestDistance = distance;
                closestTarget = col.transform;
            }
        }

        if(closestTarget == null)
        {
            currentTarget = null;
            TargetStat = null;
            return;
        }

        currentTarget = closestTarget;                           //找到后设为当前锁定目标
        TargetStat = closestTarget.GetComponentInChildren<CharacterStat>();
    }





    void AttacktheTarget()
    {
        if(currentTarget != null && TargetStat != null)
        {
            if(TargetStat.isDead) return;

            TargetStat.currentHealth -= stat.damage;
            
            if (TargetStat.isDead)
            {
                currentTarget = null;
                TargetStat = null;
            }
        }
    }




    void FaceTarget()
    {
        if(currentTarget != null)
        {
            Vector3 direction = (currentTarget.position - MainObject.position).normalized;
            direction.y = 0;
            if(direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                MainObject.rotation = Quaternion.Slerp(MainObject.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
        }
    }




}
