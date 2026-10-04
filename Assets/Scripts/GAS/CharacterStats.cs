using System;
using System.Collections.Generic;
using UnityEngine;

public struct ValueChange
{
    public float Add;
    public float Percent;


    public float ChangedValue(float OriginalValue)
    {
        return (OriginalValue + Add) * (1 + Percent);
    }
}

public class CharacterStats : MonoBehaviour
{
    

    public float MaxHealth;
    public float CurrentHealth;
    public float MaxShield;
    public float CurrentShield;
    public float Armor;
    public float MoveSpeed;
    public ArmorType ArmorKind;
    public enum ArmorType 
    {
        Light,
        Medium,
        Heavy
    }
    


    public  int TeamID;                 //阵营 目前先用0和1表示敌人和友军 之后在其他地方建立一个局内阵营关系和存阵营的字典 然后这里改成enum 逻辑改成目标是否在当前关阵营敌对表内




    public float Damage;
    public float ExplosionDmg;
    public float PenetrationDmg;
    public float EnergyDmg;
    public float ShootDelay;
    public float BurstDelay;  
    
    public float DetectRange;
    public float ShootRange;


    

    public float FinalFixed;              //造成最终伤害之前的独立增减伤乘区值

    public Action OnDeath;
    public bool IsDead;




    public Dictionary<FName, float> AdditionalStats = new(); 
    private Dictionary<FGameTag, List<ValueChange>> Modifiers= new();
    private Dictionary<FGameTag, (Func<float> Get, Action<float> Set)> StatAccessors;

    void Awake()
    {
        CurrentHealth = MaxHealth;
        CurrentShield = MaxShield;
        IsDead = false;

        InitAccessors();
    }


    private void InitAccessors()
    {
        StatAccessors = new()
        {
            [TDGameTags.MaxHealth]      = (() => MaxHealth,       v => MaxHealth = v),
            [TDGameTags.Health]         = (() => CurrentHealth,   v => CurrentHealth = v),
            [TDGameTags.MaxShield]      = (() => MaxShield,       v => MaxShield = v),
            [TDGameTags.Shield]         = (() => CurrentShield,   v => CurrentShield = v),
            [TDGameTags.Armor]          = (() => Armor,           v => Armor = v),
            [TDGameTags.MoveSpeed]      = (() => MoveSpeed,       v => MoveSpeed = v),
            [TDGameTags.Damage]         = (() => Damage,          v => Damage = v),
            [TDGameTags.ExplosionDmg]   = (() => ExplosionDmg,    v => ExplosionDmg = v),
            [TDGameTags.PenetrationDmg] = (() => PenetrationDmg,  v => PenetrationDmg = v),
            [TDGameTags.EnergyDmg]      = (() => EnergyDmg,       v => EnergyDmg = v),
            [TDGameTags.DetectRange]    = (() => DetectRange,      v => DetectRange = v),
            [TDGameTags.ShootRange]     = (() => ShootRange,       v => ShootRange = v),
        };
    }



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


    



    public float GetModifiedValue(FGameTag Tag, float Value)
    {
        if(!Modifiers.TryGetValue(Tag, out List<ValueChange> ValueList) || ValueList.Count == 0) return Value;
        float Add = 0;
        float 最终正数值 = 0;
        float 最终负数值 = 0;
        for(int i = 0; i < ValueList.Count; i++)
        {
            Add += ValueList[i].Add;
            if (ValueList[i].Percent > 最终正数值) 最终正数值 = ValueList[i].Percent;
            if (ValueList[i].Percent < 最终负数值) 最终负数值 = ValueList[i].Percent;
        }

        float FinalValue = (Value + Add) * (1 + (最终正数值 + 最终负数值));
        return FinalValue;
    }

    public void AddModifier(FGameTag Tag, ValueChange VC)
    {
        if (!Modifiers.ContainsKey(Tag))
        {
            Modifiers.Add(Tag, new List<ValueChange>());
        }

        Modifiers[Tag].Add(VC);
    }


    public void RemoveModifier(FGameTag Tag, ValueChange VC)
    {
        if(!Modifiers.ContainsKey(Tag)) return;
        Modifiers[Tag].Remove(VC);

        if(Modifiers[Tag].Count == 0) Modifiers.Remove(Tag);
    }

    

    public List<ValueChange> GetModifyRef(FGameTag Tag)
    {
        if(!Modifiers.TryGetValue(Tag, out List<ValueChange> VC)) return null;
        return VC;
    }


    public float GetBaseValue(FGameTag Tag)
    {
        if(StatAccessors.TryGetValue(Tag, out var Accessor)) return Accessor.Get();
        string Full = Tag.ToString();
        string ShortName = Full.Contains('.') ? Full.Substring(Full.LastIndexOf('.') + 1) : Full;
        return GetAdditionalStat(new FName(ShortName));
    }


    public void SetBaseStat(FGameTag Tag, float Value)
    {
        if(StatAccessors.TryGetValue(Tag, out var Accessor))
        {
            Accessor.Set(Value);
            return;
        }
        string Full = Tag.ToString();
        string ShortName = Full.Contains('.') ? Full.Substring(Full.LastIndexOf('.') + 1) : Full;
        AdditionalStats[new FName(ShortName)] = Value;
    }


    public float GetModifiedValue(FGameTag Tag)
    {
        float Base = GetBaseValue(Tag);
        return GetModifiedValue(Tag, Base);
    }













    public void DeathDetection()
    {
        if (CurrentHealth <= 0 && IsDead != true)
        {
            IsDead = true;
            OnDeath?.Invoke();
        }
    }


    public void DecreaseHealth(float Damage)
    {
        float FinalDamage = Mathf.FloorToInt((Damage - Armor) * (1 + FinalFixed));

        CurrentHealth -= FinalDamage;
        if(CurrentHealth < 0)
        {
            CurrentHealth = 0;
            DeathDetection();
        }
    }



    public float 保留一位小数(float Value)
    {
        return Mathf.Floor(Value * 10f + 0.5f) / 10f;
    }




    void Start()
    {
        
    }














}
