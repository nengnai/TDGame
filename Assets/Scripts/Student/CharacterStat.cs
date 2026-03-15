using UnityEngine;
using UnityEngine.AI;

public class CharacterStat : MonoBehaviour
{
    public GameObject Ring;


    [Header("基础数据")]
    public string characterName;
    public int maxHealth;
    int currentHealth;
    public int damage;
    public float firingSpeed;
    public float reloadSpeed;
    
    
    
    
    
    public bool isAlly;






    
    

    [Header("移动速度")]
    public float moveSpeed;
    public float Acceleration;   //加速度
    public float AngularSpeed;   //转身速度





    public float StoppingDistance;   //停止距离（还是写一点 不然会因为距离问题互相顶
    public float SkillCD;
    public bool isSelected;


    [Header("其他")]
    NavMeshAgent agent;
    Transform agentTransform;
    public StudentSaveonButton button;






    void Awake()
    {
        isSelected = false;
    }


    void Start()
    {
        agent = GetComponentInParent<NavMeshAgent>();
        UpdateStatsToAgent();


        agentTransform = agent.transform;


        currentHealth = maxHealth;
    }









    void UpdateStatsToAgent()
    {
        if(agent != null)
        {
            agent.speed = moveSpeed;
            agent.acceleration = Acceleration;
            agent.angularSpeed = AngularSpeed;
            if(agent.autoBraking != false) agent.stoppingDistance = StoppingDistance;

            //覆盖掉原agent参数


            agent.updateRotation = false;    //解限转角速度
        }
    }

    
    public void UpdateSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
        UpdateStatsToAgent();
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




}
