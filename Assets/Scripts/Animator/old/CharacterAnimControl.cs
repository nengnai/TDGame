
using UnityEngine;
using UnityEngine.AI;



public enum CombatState
{
    Idle,
    Moving,
    StopMoving,
    AttackStart,
    Attacking,
    AttackEnd,
    Reloading
}



public class CharacterCombat : MonoBehaviour
{
    int enemyMask;
    public CharacterStat stat;
    public Animator animator;
    AnimatorStateInfo stateInfo;
    public NavMeshAgent agent;
    float range;                       //索敌距离 （可控学生的索敌距离 = 射程）
    
    public float reloadCheck;          //待机后开始检查弹药然后换弹的时间
    public float detectInterval;       //间隔
    float detectTimer;                 //检索附近目标的计时

    public Transform firingPoint;      //开火特效位置和子弹出现位置（如果需要）











    Transform currentTarget;           //当前敌对目标
    Transform concentratedTarget;      //集火目标（通过技能释放）
    CharacterStat targetStat;          //当前敌对目标属性


    CombatState currentState;          //当前战斗状态
    //float stateTimer;                  //状态公用计时器（需要及时清零
    //AttackPhase currentPhase;          //当前开火阶段

    [Header("最佳默认动画耗时")]
    public float windUpTime;          //开火前摇

    public float stopMoveing;         //停止移动
    
    public float bestSpeed;          //1倍移动动画耗时
    public float reloadSpeed;        //1倍换弹动画耗时
    public float firingSpeed;        //1倍开火动画耗时
    public float actualWindUpTime;    //别动 给隔壁脚本射击前后摇做倍率用的

    [Header("开关")]
   



    [Header("转身速度")]

    public float turnSpeed;




    void Awake()
    {
        enemyMask = LayerMask.GetMask("Enemy");
        range = stat.range;
        currentState = CombatState.Idle;
        detectTimer= 0f;
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        
    }





    void Update()
    {


        




        


        float CurrentMovespeed = agent.velocity.magnitude;
        

        float Multiple = CurrentMovespeed / bestSpeed;        
        animator.SetFloat("RunAnimSpeed", Multiple);

        float MultipleReloadspeed = reloadSpeed / stat.reloadTime;
        animator.SetFloat("ReloadAnimSpeed", MultipleReloadspeed);

        float MultipleFiringspeed = firingSpeed / stat.firingInterval;
        animator.SetFloat("ShootAnimSpeed", MultipleFiringspeed);


        if(currentState == CombatState.Attacking || stat.isStunned)
        {
            agent.isStopped = true;
        }
        else if (currentState != CombatState.Attacking && !stat.isStunned)
        {
            agent.isStopped = false;
        }

        if(CurrentMovespeed > 0.1f && agent.hasPath)    //如果有速度 有移动路线 (停止距离默认是0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
        



        


















        switch (currentState)
        {
            case CombatState.Idle:
            
            break;


            case CombatState.Moving:
            
            break;


            case CombatState.StopMoving:
            
            break;

            
            case CombatState.AttackStart:
            
            break;


            case CombatState.Attacking:
            
            break;


            case CombatState.AttackEnd:
            UpdateAttackEnd();
            break;


            case CombatState.Reloading:
            UpdateReload();
            break;
        }
    }



    void ChangeState(CombatState state)
    {
        switch (state)
        {
            case CombatState.Idle:
            
            break;


            case CombatState.Moving:
            

            break;


            case CombatState.StopMoving:
            

            break;


            case CombatState.AttackStart:
            
            break;


            case CombatState.Attacking:
            animator.SetTrigger("Shoot");
            break;


            case CombatState.AttackEnd:
            animator.SetBool("isAttacking", false);              //这里关掉这个bool之后 idle那边就不需要关了
            break;


            case CombatState.Reloading:
            animator.SetTrigger("NeedReload");
            break;
        }
    }






    void UpdateIdle()
    {
        if(stat.currentAmmo <= 0)                                 //如果当前弹药是0 则立刻换弹 不执行之后的逻辑
        {
            ChangeState(CombatState.Reloading);
            return;
        }



        
        
        detectTimer += Time.deltaTime;
        if(detectTimer >= detectInterval)
        {
            detectTimer = 0f;
            FindClosestEnemy();
        }







    }




    void UpdateAttackEnd()
    {
        if(!animator.IsInTransition(0) && stateInfo.IsName("Attack_End"))
        {
            if(stateInfo.normalizedTime >= 1f)                 //等待动画结束
            {
                ChangeState(CombatState.Idle);
            }
        }
    }


    void UpdateReload()
    {
       if(!animator.IsInTransition(0) && stateInfo.IsName("Reloading"))
        {
            if(stateInfo.normalizedTime >= 1f)                 //等待换弹动画结束
            {
                if(currentTarget == null || Vector3.Distance(transform.position, currentTarget.position) > range || targetStat.isDead)    //如果目标没了 超范围了 死了
                {
                    FindClosestEnemy();                                              //瞬间搜一圈附近有没有可攻击单位
                    if(currentTarget == null)
                    {
                        ChangeState(CombatState.AttackEnd);
                    }

                }
            }
        }
    }









    void FindClosestEnemy()
    {
        Collider[] enemy = Physics.OverlapSphere(transform.position, range, enemyMask);
        float closestDist = Mathf.Infinity;
        currentTarget = null;
        foreach(Collider col in enemy)
        {
            CharacterStat tempStat = col.GetComponentInChildren<CharacterStat>();
            
            if(tempStat != null && !tempStat.isDead)
            {
                if (tempStat.isMarked)                                   //率先检查有没有被上集火标记的
                {
                    currentTarget = col.transform;
                    targetStat = tempStat;
                    return;
                }
                else
                {
                    float dist = Vector3.Distance(transform.position, col.transform.position);          //然后检查最近距离的目标
                    if(dist < closestDist)
                    {
                        closestDist = dist;
                        currentTarget = col.transform;
                        targetStat = tempStat;
                        
                    }
                }
            }
        }
    }
}
