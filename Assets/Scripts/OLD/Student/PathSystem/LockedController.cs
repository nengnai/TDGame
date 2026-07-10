using Pathfinding;
using UnityEngine;

public class LockedController : MonoBehaviour
{
    
    FollowerEntity AI;
    bool isLocked;

    void Awake()
    {
        AI = GetComponent<FollowerEntity>();
        isLocked = AI.rvoSettings.locked;
        SetBool(true);
    }

    void Update()
    {
        SetBool(AI.reachedEndOfPath || !AI.hasPath);
    }

    void SetBool(bool NewBool)
    {
        if(isLocked != NewBool)
        {
            var agent = AI.rvoSettings;
            agent.locked = NewBool;
            AI.rvoSettings = agent;
            isLocked = NewBool;
        }
    }
}
