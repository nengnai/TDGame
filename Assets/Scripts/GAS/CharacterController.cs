using Pathfinding;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    
    CharacterStats MyStats;
    CharacterStats MyTarget;
    SearchingTarget SearchingTarget;
    AbilitySystemComponent ASC;
    FollowerEntity 寻路系统;


    float 目标距离;
    Vector3 移动目标地点;



    FAbilityHandle 普攻;



    void Awake()
    {
        MyStats = GetComponent<CharacterStats>();
        SearchingTarget = GetComponent<SearchingTarget>();
        ASC = GetComponent<AbilitySystemComponent>();
        寻路系统 = GetComponentInParent<FollowerEntity>();

        普攻 = ASC.AddAbility(MyStats.NormalAttack);
    }


    void Update()
    {
        if(MyStats.IsDead) return;
        if (ASC.Tags.HasTag(TDGameTags.IsStun))
        {
            寻路系统.SetPath(null);
            ASC.Tags.RemoveTag(TDGameTags.IsMove);

            ASC.EndAbility(普攻);
            //去掉tag
        }



        //主动/被动技能检测和释放


        if (寻路系统.hasPath)
        {
            ASC.EndAbility(普攻);
            //去掉tag
            寻路系统.SetDestination(移动目标地点);
            ASC.Tags.AddTag(TDGameTags.IsMove);

            if (寻路系统.reachedDestination)
            {
                寻路系统.SetPath(null);
                ASC.Tags.RemoveTag(TDGameTags.IsMove);
            }
        }

        if(MyTarget != null)
        {
            目标距离 = Vector3.Distance(MyStats.transform.position, MyTarget.transform.position);
            if(目标距离 > MyStats.ShootRange)
            {
                寻路系统.SetDestination(MyTarget.transform.position);
            }
            else                            //记得加减攻击tag
            {
                if (!寻路系统.reachedDestination)
                {                   
                    寻路系统.SetPath(null);
                    ASC.ActivateAbility(普攻);
                }
            }
        }


    }
}
