using UnityEngine;
using UnityEngine.AI;

public class CharacterState : MonoBehaviour
{   
    [Header("选中光圈")]
    public GameObject Ring; //角色脚下的光圈 （用于提示被选中）
    bool FirstTime = false; //生成后让光圈灭掉 毕竟生成的时候是未选中状态
    [Header("单位属性")]
    public bool isAlly = false; //判断是否是盟友 （盟友不可控）
    public bool isSelected = false; //判断是否被选中 （只有被选中的时候才可以进行移动攻击等指令）
    [Header("单位数值")]
    public float Maxhealth; //最大生命值
    float CurrentHealth;  //当前生命中
    public float Damage;  //伤害
    public float MoveSpeed;  //移速
    public float Acceleration;  //加速度
    public float AngularSpeed;  //转身速度
    public float StoppingDistance;  //停止距离（加点 不然会因为距离问题互相顶
    public float PreparationTime; //技能CD什么的
    private NavMeshAgent agent;
    private Transform agentTransform;



    
    
    void Awake()
    {
        if (!FirstTime)
        {
            Ring.SetActive(false);
            FirstTime = true;
        }
        else
        {
            return;
        }
    }//开局开关一下选中圆圈

    void Start()
    {
        agent = GetComponentInParent<NavMeshAgent>();//获取一下父级上的navmesh
        UpdateStatsToAgent(); //更新上面的数据


        agentTransform = agent.transform; //转向的玩意似乎 别动
    }


    void Update()
    {
        if(agent == null) return; //单位死亡后停止更新

        Vector3 currentVelocity = agent.velocity;


        if(currentVelocity.sqrMagnitude > 0.01f)
        {
            currentVelocity.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(currentVelocity);
            agentTransform.rotation = Quaternion.RotateTowards(agentTransform.rotation, targetRotation, AngularSpeed * Time.deltaTime);
        }
        //说是取消使用navmesh的转向 改用平滑解限转向
        //不太懂 感觉先别乱动 等吃性能的时候再说
    }

    public void UpdateStatsToAgent()
    {
        if(agent != null)
        {
            agent.speed = MoveSpeed;
            agent.acceleration = Acceleration;
            agent.angularSpeed = AngularSpeed;
            agent.stoppingDistance = StoppingDistance;
            //覆盖掉原agent参数

            agent.updateRotation = false; //解限转角速度
        }
    }





    public void UpdateSpeed(float newSpeed)
    {
        MoveSpeed = newSpeed;
        UpdateStatsToAgent();
    }
    //如果有被减速之类的效果 调用这个 会把新的速度更新给agent





}
