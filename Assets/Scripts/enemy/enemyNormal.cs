using System.Collections;
using System.Drawing;
using UnityEngine;

public class enemyNormal : MonoBehaviour
{

    private int pointIndex = 0;
    private Vector3 targetPosition = Vector3.zero;

    public float speed = 2;
    void Start()
    {
        StartCoroutine(InitTarget());
    }

    IEnumerator InitTarget()
    {
        while(Waypoint.Instance == null)
        {
            yield return null;
        }

        targetPosition = Waypoint.Instance.GetWaypoint(pointIndex);
    }


    // Update is called once per frame
    void Update()
    {
        if(targetPosition == Vector3.zero)
        return;

        transform.Translate((targetPosition-transform.position).normalized * speed * Time.deltaTime);


        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            moveNextpoint();


        }
    }

    private void moveNextpoint()
    {
        pointIndex++;
        if(pointIndex > (Waypoint.Instance.GetLength() - 1))
            {
                Die();
                return;
            }
        targetPosition = Waypoint.Instance.GetWaypoint(pointIndex);
    }

    void Die()
    {
        Destroy(gameObject);
        
        if(enemySpawner.Instance != null)
        enemySpawner.Instance.DecreateEnemyCount();
        else
        Debug.LogWarning("实例不存在");
        //TODO 漏怪惩罚
    }

}
