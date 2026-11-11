using Pathfinding;
using UnityEngine;

public class CharacterAnim : MonoBehaviour
{
    CharacterStats MyStats;
    FollowerEntity 寻路系统;
    Animator 动画系统;
    FGameTag 当前动画;
    角色控制器 Controller;
    AbilityTimeManager 计时器;
    FTimerHandle 收枪计时;


    void Awake()
    {
        MyStats = GetComponent<CharacterStats>();
        寻路系统 = GetComponentInParent<FollowerEntity>();
        动画系统 = GetComponent<Animator>();
        Controller = GetComponent<角色控制器>();
        计时器 = new();

        当前动画 = TDGameTags.AnimIdle;
        动画系统.CrossFade("Idle", 0.1f, 0, 0f);
        
        // 监听射击事件
        MyStats.AnimatorEvent.ListenAction(TDGameTags.AnimatorFireEvent, ShootAnim);
    }

    void Update()
    {
        if (MyStats.IsDead)
        {
            if(当前动画 != TDGameTags.AnimDead)
            {
                动画系统.CrossFade("Dead", 0.1f, 0, 0f);
                动画系统.CrossFade("Shoot.Empty", 0.1f, 1, 0f);
                当前动画 = TDGameTags.AnimDead;
            }
            收枪计时.RemoveTimer();
            return;
        }

        
        //眩晕

        if (寻路系统.hasPath && !寻路系统.reachedDestination)
        {
            if(当前动画 !=  TDGameTags.AnimMoving)
            {
                动画系统.CrossFade("Moving", 0.1f, 0, 0f);
                动画系统.Play("Shoot.Empty", 1);
                当前动画 = TDGameTags.AnimMoving;
                收枪计时.RemoveTimer();
            }
        }
        else if(Controller.MyTarget == null)
        {
            if(当前动画 == TDGameTags.AnimShooting)
            {
                当前动画 = TDGameTags.AnimIdle;
                收枪计时 = 计时器.AddTimer(5f, false, false, () => 
            {
                动画系统.CrossFade("Base Layer.AttackEnd", 0.1f, 0, 0f);
                动画系统.Play("Shoot.Empty", 1);
            });
            }
            else if(当前动画 != TDGameTags.AnimIdle)
            {
                动画系统.CrossFade("MoveEnd", 0.1f, 0, 0f);
                当前动画 = TDGameTags.AnimIdle;
            }
        }
        else
        {
            if(当前动画 != TDGameTags.AnimShooting)
            {
                动画系统.CrossFade("Base Layer.AttackStart", 0.1f, 0, 0f);
                当前动画 = TDGameTags.AnimShooting; 
            }
        }

    }


    public void ShootAnim(AnimatorEventData AnimData)
    {   
        收枪计时.RemoveTimer();
        if(MyStats.IsDead) return;
        动画系统.CrossFade("Shoot.Attack", 0.1f, 1, 0f);
    }
}
