using System.Drawing;
using UnityEngine;

public class enemyNormal : MonoBehaviour
{

    private int pointIndex = 0;
    private Vector3 targetPosition = Vector3.zero;

    public float speed = 2;
    void Start()
    {
        targetPosition = Waypoint.Instance.GetWaypoint(pointIndex);
    }

    // Update is called once per frame
    void Update()
    {
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
        enemySpawner.Instance.DecreateEnemyCount();
        //TODO 漏怪惩罚
    }

}
