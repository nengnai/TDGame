using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    

    public int MaxHealth;
    public int CurrentHealth;
    public int MaxShield;
    public int CurrentShield;
    public int Armor;
    public float MoveSpeed;
    public enum ArmorType
    {
        Light,
        Heavy
    }





    public int Damage;
    public enum ShootType                           //d
    {
        Normal,
        Burst,
    }

    public float ShootSpeed;    //【连发模式】一秒内能射几次          射击速度小于射击动画播放时长的时候就得需要同时加快射击动画播放速度
                                //非连发模式的角色将不使用该参数

    public int MaxBurstCount;
    public int CurrentBurstCount;
    public float BurstShootSpeed;    //【爆发射击模式】一秒内能射几次   动画同理
                                     //受到buff影响则和BurstDelay一起改变
    public float BurstDelay;         //【爆发射击模式】射完一组爆发后等待的时间
                                     //非爆发射击模式的角色将不使用该参数     受到buff影响则和BurstShootSpeed一起改变


    public float DetectRange;
    public float ShootRange;

    public int MaxAmmo;
    public int CurrentAmmo;




    public Action OnDeath;

    public bool IsDead;



    
    public float AniIdleTime;
    public float AniShootTime;
    public float AniMoveTime;
    public float AniReloadTime;
    public float AniBoltTime;
    public float AniAttackWindupTime;
    public float AniAttackWinddownTime;



    public Dictionary<FName, float> AdditionalStats = new Dictionary<FName, float>(); 
    public void NewAdditionalStat(FName StatName, float Value)
    {
        AdditionalStats[StatName] = Value;
    }


    public float GetAdditionalStat(FName StatName)
    {
        if(AdditionalStats.TryGetValue(StatName, out float StatValue))
        {
            return StatValue;
        }
        else
        {
            throw new System.Exception($"没有找到{StatName}/{StatName}不存在");
        }
    }


    public bool HasAdditionalStat(FName StatName)
    {
        return AdditionalStats.ContainsKey(StatName);
    }







    public void DeathDetection()
    {
        if (this.CurrentHealth <= 0 && IsDead != true)
        {
            IsDead = true;
            OnDeath?.Invoke();
        }
    }





    void Start()
    {
        CurrentHealth = MaxHealth;
        CurrentAmmo = MaxAmmo;
        CurrentShield = MaxShield;
        IsDead = false;
    }














}
