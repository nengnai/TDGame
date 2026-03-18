
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]
public class PathDrawer : MonoBehaviour
{
    CharacterStat characterStat;
    private NavMeshAgent agent;
    private LineRenderer lineRenderer;
    [Header("视觉设置")]
    [Tooltip("抬高线条的高度，防止线和地板穿模（Z-Fighting）")]
    public float lineOffsetY = 0.2f;
    
    [Header("性能设置")]
    [Tooltip("底层路径检测的更新间隔（秒），建议0.1~0.2")]
    public float updateInterval = 0.1f; 
    void Start()
    {
        // 获取组件
        characterStat = GetComponentInChildren<CharacterStat>();
        agent = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>();
        
        // 确保线条在世界坐标下绘制
        lineRenderer.useWorldSpace = true;
        // 脚本启动时，开启协程进行低频的路径运算
        StartCoroutine(UpdatePathRoutine());
    }
    void Update()
    {
        if(characterStat.isSelected == false)
        {
            lineRenderer.enabled = false;
        }
        else
        {
            lineRenderer.enabled = true;
        }
        // 【进阶视觉优化：解决线脚脱节】
        // 虽然完整的路径每 0.2 秒才刷新一次，但我们在每帧都强制把线条的第 0 个点（起点）
        // 绑定在角色当前的位置。这样视觉上绝对平滑，完全没有延迟感。
        if (lineRenderer.positionCount > 0)
        {
            Vector3 startPoint = new Vector3(transform.position.x, transform.position.y + lineOffsetY, transform.position.z);
            lineRenderer.SetPosition(0, startPoint);
        }
    }
    // 这是一个协程，专门用来处理耗性能的底层路径获取任务
    IEnumerator UpdatePathRoutine()
    {
        while (true) 
        {
            // 如果Agent有路径，且距离终点还大于停止距离（说明还在跑）
            if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
            {
                DrawPath();
            }
            else
            {
                // 如果停下来了，或者没有路径，立刻清空线条
                lineRenderer.positionCount = 0;
            }
            // 让协程休眠指定的秒数，大大降低 CPU 占用
            yield return new WaitForSeconds(updateInterval);
        }
    }
    void DrawPath()
    {
        // 核心：获取导航网格计算出的路径拐点（这是一个相对耗时的操作，所以放在协程里）
        Vector3[] corners = agent.path.corners;
        // 防错处理：如果连起点和终点都不够，就不画线
        if (corners.Length < 2) return;
        // 设置 LineRenderer 的顶点数量
        lineRenderer.positionCount = corners.Length;
        // 遍历所有拐点，把它们赋值给 LineRenderer
        for (int i = 0; i < corners.Length; i++)
        {
            // 给所有的点加一点 Y 轴偏移，防止线贴在模型内部或地板里面
            Vector3 pointPosition = new Vector3(corners[i].x, corners[i].y + lineOffsetY, corners[i].z);
            lineRenderer.SetPosition(i, pointPosition);
        }
    }
}