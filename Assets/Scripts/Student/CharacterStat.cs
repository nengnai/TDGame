using UnityEngine;
using UnityEngine.AI;


public enum ShootingType
{
    Fullauto,
    Burst,
    Bolt
}

public enum AttackType
{
    Normal,
    Pierce
}

public enum ArmorType
{
    Light,
    Heavy
}


public class CharacterStat : MonoBehaviour
{
    public GameObject Ring;


    [Header("基础数据")]
    public string characterName;
    public int maxHealth;
    public int currentHealth;
    public float destroiedTime;
    
    public int damage;
    public float range;
    public float firingWindup;                //射击前摇瞄准 比如制导导弹需要提前瞄准一段时间才能发射 如果不需要就不填写
    public float firingDelay;                 //如果是爆发连射模式就给个数值
    public int fullAmmo;    
    public int currentAmmo;

    public int burstTime;                      //如果是爆发射击模式 一次爆发射出的弹药量

    [Header("动画时长")]
    public float AttackStartTime;
    public float AttackTime;
    public float AttackEndTime;
    public float ReloadTime;
    public float IdleReloadTime;
    
    public bool isAlly;                       //是否是队友
    public bool isUndestroied;                //倒地时是否可被摧毁（复活技能用的）
    public ShootingType shootingType;
    public AttackType attackType;
    public ArmorType armorType;

    [Header("状态")]
    public bool isMoving;
    public bool isIdling;
    public bool isReloading;
    public bool isShooting;
    public bool isDead;
    public bool isInvincible;              //无敌状态
    public bool isStunned;                 //是否被眩晕
    public bool isMarked;                  //是否被标记集火
    public bool isFacingTarget;            

    [Header("AI逻辑")]
    public bool HasIdleReloadAnim;           //待机换弹的时候是否有专属动画

    
    

    [Header("移动速度")]
    public float moveSpeed;
    public float Acceleration;   //加速度
    public float AngularSpeed;   //转身速度





    public float StoppingDistance;   //停止距离（还是写一点 不然会因为距离问题互相顶
    public float SkillCD;
    public bool isSelected;


    [Header("其他")]
    NavMeshAgent agent;
    Transform agentTransform;
    public new Collider collider;
    public Transform thisUnit;
    public StudentSaveonButton button;












}
