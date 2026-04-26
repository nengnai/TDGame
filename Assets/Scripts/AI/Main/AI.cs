using UnityEngine;
using UnityEngine.AI;

public class AI
{
    [Header("引用")]
    CharacterStat stat;
    Animator animator;
    NavMeshAgent agent;
    Transform MainObject;





    [Header("状态相关")]
    IState currentState;
    
    public void ChangeState(IState TargetState)
    {
        
    }

    void Update()
    {
        
    }



    [Header("计时器")]
    float detectTimer;               //索敌检测间隔
    float attackTimer;               //开火用时（其实就是相当于攻击间隔）
    float reloadTimer;
    float idleReloadTimer;
    float stunTimer;
    //不加延迟弹药检查了 弹药不满直接换弹
    int burstCount;



    [Header("目标相关")]
    Transform currentTarget;
    CharacterStat targetStat;

    public void TryFindTarget()               //寻敌 之后拉出去单独做脚本
    {
        
    }

    public void FaceTarget()                   //面向敌人 同上
    {
        
    }


    [Header("配置")]
    public float detectDur;
    public float turnSpeed;
    public LayerMask targetMask;

}
