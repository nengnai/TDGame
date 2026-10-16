using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class CharacterAnim : MonoBehaviour
{
    FollowerEntity 寻路系统;
    Animator 动画系统;
    List<FGameTag> 当前动画 = new(1);


    void Awake()
    {
        寻路系统 = GetComponentInParent<FollowerEntity>();
        动画系统 = GetComponent<Animator>();
        当前动画.Add(TDGameTags.AnimIdle);
    }

    void Update()
    {


        if (寻路系统.hasPath && !寻路系统.reachedDestination)
        {
            if(当前动画[0] != TDGameTags.AnimMoving)
            {
                动画系统.CrossFade("Moving", 0.1f);
                当前动画.Clear();
                当前动画.Add(TDGameTags.AnimMoving);
            }
        }
        else
        {
            if (当前动画[0] != TDGameTags.AnimIdle)
            {
                动画系统.CrossFade("MoveEnd", 0.1f);
                当前动画.Clear();
                当前动画.Add(TDGameTags.AnimIdle);
            }
        }
    }
}
