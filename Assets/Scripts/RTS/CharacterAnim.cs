using UnityEngine;
using UnityEngine.AI;

public class CharacterAnim : MonoBehaviour
{

    private NavMeshAgent agent;
    private Animator anim;
    private CharacterState stat;


    void Start()
    {
        agent = GetComponentInParent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        stat = GetComponent<CharacterState>();
    }

    void Update()
    {

        float currentSpeed = agent.velocity.magnitude;   //获取当前移速
        float maxSpeed = agent.speed;                 //characterstat里设置的最大移速
        float speedRatio = currentSpeed / maxSpeed;      //计算比例
        speedRatio = Mathf.Max(speedRatio, 0.1f);        //给个0.1兜底防止速度很慢的时候静止不动
        anim.SetFloat("RunAnimSpeed", speedRatio);       //比例传给animator
        //动画播放速度跟移动速度挂钩 防止停下的时候很奇怪 以及被减速的时候有对应效果

        if(agent != null && anim != null)
        {
            bool isMoving = agent.velocity.sqrMagnitude > 0.1f || agent.pathPending || agent.remainingDistance > agent.stoppingDistance;
            //判断，当移速大于0.1，或者agent还在计算路径（刚点的一瞬间），或者剩余路径长度大于设定好的停止距离，则判定角色在行走，输出true
            anim.SetBool("isWalking", isMoving);
            //传给动画器 播放移动动画
        }
    }

}
