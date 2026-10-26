using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class CharacterAnim : MonoBehaviour
{
    CharacterStats MyStats;
    FollowerEntity 寻路系统;
    Animator 动画系统;
    List<FGameTag> 当前动画 = new(1);
    角色控制器 Controller;
    AbilitySystemComponent ASC;

    AbilityTimeManager 计时器;
    FTimerHandle 收枪计时;


    void Awake()
    {
        MyStats = GetComponent<CharacterStats>();
        寻路系统 = GetComponentInParent<FollowerEntity>();
        动画系统 = GetComponent<Animator>();
        Controller = GetComponent<角色控制器>();
        ASC = GetComponent<AbilitySystemComponent>();
        计时器 = new();

        当前动画.Add(TDGameTags.AnimIdle);
    }

    void Update()
    {
        if (MyStats.IsDead)
        {
            if(当前动画[0] != TDGameTags.AnimDead)
            {
                动画系统.CrossFade("Dead", 0.1f, 0, 0f);
                动画系统.Play("Shoot.Empty", 1);
                当前动画.Clear();
                当前动画.Add(TDGameTags.AnimDead);
            }
            return;
        }

        if (寻路系统.hasPath && !寻路系统.reachedDestination)
        {
            if(当前动画[0] != TDGameTags.AnimMoving)
            {
                动画系统.CrossFade("Moving", 0.1f, 0, 0f);
                当前动画.Clear();
                当前动画.Add(TDGameTags.AnimMoving);
            }
        }
        else
        {
            if (当前动画[0] != TDGameTags.AnimIdle && 当前动画[0] != TDGameTags.AnimShooting)
            {
                if(Controller.MyTarget == null && !ASC.Tags.HasTagExact(TDGameTags.IsAttack))  动画系统.CrossFade("MoveEnd", 0.1f, 0, 0f);
                当前动画.Clear();
                当前动画.Add(TDGameTags.AnimIdle);
            }
        }


        if(Controller.MyTarget == null && 当前动画[0] == TDGameTags.AnimShooting)
        {
            当前动画[0] = TDGameTags.AnimIdle;
            Debug.Log("没人了");
            收枪计时 = 计时器.AddTimer(5f, false, false, () => {动画系统.CrossFade("Base Layer.AttackEnd", 0.1f, 0, 0f);});
        }

    }


    public void ShootAnim()
    {   
        if(收枪计时.IsValid()) 计时器.RemoveTimer(收枪计时);

        动画系统.CrossFade("Base Layer.AttackStart", 0.1f, 0, 0f);
        动画系统.CrossFade("Shoot.Attack", 0.1f, 1, 0f);
        当前动画.Clear();
        当前动画.Add(TDGameTags.AnimShooting);
    }
}
