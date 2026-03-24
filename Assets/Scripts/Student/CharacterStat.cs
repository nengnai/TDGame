using UnityEngine;
using UnityEngine.AI;

public class CharacterStat : MonoBehaviour
{
    public GameObject Ring;


    [Header("基础数据")]
    public string characterName;
    public int maxHealth;
    public int currentHealth;
    public float destroiedTime;
    
    public int damage;
    public float range;
    public float firingWindup;                //射击前摇瞄准 比如制导导弹需要提前瞄准一段时间才能发射
    public float firingInterval;              //射击后的延迟时间 也就是两个子弹发射之间的耗时 同时也是射击动画播放的时长
    public float reloadTime;
    public int fullAmmo;    
    public int currentAmmo;
    
    
    
    public bool isAlly;
    public bool isUndestroied;

    [Header("状态")]
    public bool isMoving;
    public bool isShooting;
    public bool isDead;
    public bool isInvincible;              //无敌状态
    public bool isStunned;                 //是否被眩晕
    public bool isMarked;                  //是否被标记集火
    public bool isFacingTarget;             



    
    

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
    public new Collider collider;
    public Transform thisUnit;
    public StudentSaveonButton button;






    void Awake()
    {
        isSelected = false;
        isDead = false;
        currentAmmo = fullAmmo;
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

        if(currentHealth <= 0f && !isDead)
        {
            isDead = true;
            agent.ResetPath();
            collider.enabled = false;
            if(destroiedTime > 0)
            {
                destroiedTime -= Time.deltaTime;
            }
            else
            {
                if (isUndestroied)
                {
                    return;
                }
                else
                {
                    Destroy(thisUnit.gameObject);
                }
            }
        }


        if (isShooting)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }











        Vector3 currentVelocity = agent.velocity;


        if(currentVelocity.sqrMagnitude > 0.01f)
        {
            currentVelocity.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(currentVelocity);
            agentTransform.rotation = Quaternion.RotateTowards(agentTransform.rotation, targetRotation, AngularSpeed * Time.deltaTime);
        }
        //说是取消使用navmesh的转向 改用平滑解限转向
        //不太懂 感觉先别乱动 等吃性能的时候再说
        
        if(currentVelocity.sqrMagnitude == 0f)
        {
            agent.avoidancePriority = 0;
        }
        else
        {
            agent.avoidancePriority = 50;
        }
    
    }




}
