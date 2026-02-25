using UnityEngine;
using UnityEngine.AI;
public class MoveController : MonoBehaviour
{
    private NavMeshAgent agent;
    void Awake()
    {
        // 从父物体获取 NavMeshAgent（如果当前物体没有）
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            agent = GetComponentInParent<NavMeshAgent>();
    }
    public void MoveTo(Vector3 targetPosition)
    {
        if (agent != null)
            agent.SetDestination(targetPosition);
    }
    public bool IsMoving()
    {
        return agent != null &&
               agent.velocity.magnitude > 0.1f &&
               agent.remainingDistance > agent.stoppingDistance;
    }
}