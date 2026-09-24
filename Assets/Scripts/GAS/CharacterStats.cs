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
    public ArmorType ArmorKind;
    public enum ArmorType 
    {
        Light,
        Heavy
    }
    


    public  int TeamID;                 //阵营 目前先用0和1表示敌人和友军 之后在其他地方建立一个局内阵营关系和存阵营的字典 然后这里改成enum 逻辑改成目标是否在当前关阵营敌对表内




    public int Damage;
    public ArmorType FocusArmorType;
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
