using UnityEngine;
using UnityEngine.AI;
public class FastTurnAgent : MonoBehaviour
{
    public NavMeshAgent agent;
    
    // 转向速度（度/秒）
    public float turnSpeed; // 相当于每秒转3圈，极快
    void Awake()
    {
        // 获取 NavMeshAgent
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
        // 关闭默认旋转，让我们自己控制
        agent.updateRotation = false;
    }
    void Update()
    {
        // 有移动速度时才旋转
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Vector3 direction = agent.velocity.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            // 用非常快的速度平滑旋转
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                targetRotation, 
                turnSpeed * Time.deltaTime
            );
        }
    }
}