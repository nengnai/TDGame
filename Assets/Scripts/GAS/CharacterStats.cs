// 用于角色数据的储存

using System;
using System.Collections.Generic;
using UnityEngine;
using TDGameLibrary;

public class ValueChange
{
    public float Add;
    public readonly List<float> Percent = new();
    public float MaxPercent = 0.0f;
    public float NegativeMaxPercent = 0.0f;


    public float ChangedValue(float OriginalValue)
    {
        return (OriginalValue + Add) * (1 + MaxPercent + NegativeMaxPercent);
    }

    public void AddAddChanged(float InAdd)
    {
        Add += InAdd;
    }

    public void AddPercentChanged(float InPercent)
    {
        if (InPercent == 0.0f) return;
        
        Percent.Add(InPercent);
        if (InPercent > 0.0f)
        {
            MaxPercent = Math.Max(InPercent, MaxPercent);
        }
        else
        {
            NegativeMaxPercent = Math.Min(InPercent, NegativeMaxPercent);
        }
    }

    public void RemoveAddChanged(float InAdd)
    {
        Add -= InAdd;
    }
    public void RemovePercentChanged(float InPercent)
    {
        if (InPercent == 0.0f) return;
        if (!Percent.Remove(InPercent)) return;
        
        const float Tolerance = 0.00001f;
        if (InPercent > 0.0f)
        {
            // 只有当被移除的值可能影响正最大值时才重算
            if (InPercent >= (MaxPercent - Tolerance))
            {
                MaxPercent = 0.0f;
                foreach (var i in Percent)
                {
                    if (i > 0.0f)
                    {
                        MaxPercent = Math.Max(i, MaxPercent);
                    }
                }
            }
        }
        else if (InPercent < 0.0f)
        {
            // 只有当被移除的值可能影响负最大值时才重算
            if (InPercent <= (NegativeMaxPercent + Tolerance))
            {
                NegativeMaxPercent = 0.0f;
                foreach (var i in Percent)
                {
                    if (i < 0.0f)
                    {
                        NegativeMaxPercent = Math.Min(i, NegativeMaxPercent);
                    }
                }
            }
        }
    }
}

public struct AnimatorEventData
{
    public float AnimatorTime;

    public AnimatorEventData(float InAnimatorTime)
    {
        AnimatorTime =  InAnimatorTime;
    }
}


public class CharacterStats : MonoBehaviour
{
    // 动画事件
    public EventSystem<AnimatorEventData> AnimatorEvent = new();

    public float MaxHealth = 300.0f;
    public float CurrentHealth;
    public float MaxShield = 0.0f;
    public float CurrentShield;
    public float Armor = 10.0f;
    public float MoveSpeed = 5.0f;
    public ArmorType ArmorKind = ArmorType.Light;
    public enum ArmorType 
    {
        Light,
        Medium,
        Heavy
    }
    

    // 阵营
    public FTeamID TeamID;




    public float Damage = 10.0f;
    public float ExplosionDmg = 70.0f;
    public float PenetrationDmg;
    public float EnergyDmg;

    public float FinalFixed = 0.0f;              //造成最终伤害之前的独立增减伤乘区值

    public Action OnDeath;
    public bool IsDead;




    public readonly Dictionary<FName, float> AdditionalStats = new(); 
    private readonly Dictionary<FGameTag, ValueChange> Modifiers= new();
    private Dictionary<FGameTag, (Func<float> Get, Action<float> Set)> StatAccessors;

    void Awake()
    {
        CurrentHealth = MaxHealth;
        CurrentShield = MaxShield;
        IsDead = false;

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
        => !Modifiers.TryGetValue(Tag, out ValueChange ValueList) ? Value : ValueList.ChangedValue(Value);
    

    public void AddAddModifier(FGameTag Tag, float InAddModifier)
    {
        if(!Modifiers.TryGetValue(Tag, out ValueChange ValueList)) return;
        ValueList.AddAddChanged(InAddModifier);
    }
    public void AddPercentModifier(FGameTag Tag, float InPercentModifier)
    {
        if(!Modifiers.TryGetValue(Tag, out ValueChange ValueList)) return;
        ValueList.AddPercentChanged(InPercentModifier);
    }


    public void RemoveAddModifier(FGameTag Tag, float InAddModifier)
    {
        if(!Modifiers.TryGetValue(Tag, out ValueChange ValueList)) return;
        ValueList.RemoveAddChanged(InAddModifier);
    }
    public void RemovePercentModifier(FGameTag Tag, float InPercentModifier)
    {
        if(!Modifiers.TryGetValue(Tag, out ValueChange ValueList)) return;
        ValueList.RemovePercentChanged(InPercentModifier);
    }

    

    public ValueChange GetModifyRef(FGameTag Tag)
    {
        if(!Modifiers.TryGetValue(Tag, out var VC)) return null;
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


    public void DecreaseHealth(float InDamage)
    {
        float FinalDamage = Mathf.FloorToInt((InDamage - Armor) * (1 + FinalFixed));

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
