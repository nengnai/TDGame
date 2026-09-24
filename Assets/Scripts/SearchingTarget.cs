
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class SearchingTarget : MonoBehaviour
{
    public float ScanDur = 0.1f;             //检测间隔(sec)
    private float LastScanTime;
    public CharacterStats Myself;



    private CharacterStats CurrentAttackTarget;
    private CharacterStats CurrentChaseTarget;
    private CharacterStats FocusTarget;

    private Collider[] OverlapBuffer = new Collider[100];
    private List<CharacterStats> CandidateBuffer = new();




    public CharacterStats AttackTarget => FocusTarget ?? CurrentAttackTarget;
    public CharacterStats ChaseTarget => CurrentChaseTarget;




    private AbilitySystemComponent AbilitySystem;


    void Awake()
    {
        AbilitySystem = GetComponent<AbilitySystemComponent>();
    }

    void Update()
    {
        if(!IsTargetValid(CurrentAttackTarget)) CurrentAttackTarget = null;
        if(!IsTargetValid(CurrentChaseTarget)) CurrentChaseTarget = null;
        if(!IsTargetValid(FocusTarget)) FocusTarget = null;
    }



    bool IsTargetValid(CharacterStats Target)
    {
        return TargetScoringLibrary.IsAlive(Target);
    }

    public void SetFocusTarget(CharacterStats Target)
    {
        FocusTarget = Target;
    }

    public void ClearFocusTarget()
    {
        FocusTarget = null;
    }



    public void ScanWithWeapon(GA_WeaponBase Weapon)
    {
        if(Weapon == null) return;
        if(Myself == null) return;


        if(Time.time - LastScanTime < ScanDur) return;
        LastScanTime = Time.time;



        CollectCandidatesInRange(Weapon.GetShootRange());
        if(CandidateBuffer.Count > 0)
        {
            CurrentAttackTarget = SelectBestTarget(CandidateBuffer, Weapon, Myself, false);
            CurrentChaseTarget = null;
            return;
        }

        CollectCandidatesInRange(Weapon.GetChaseRange());
        CurrentChaseTarget = SelectBestTarget(CandidateBuffer, Weapon, Myself, true);
        CurrentAttackTarget = null;
    }


    void CollectCandidatesInRange(float Range)
    {
        CandidateBuffer.Clear();
        int HitCount = Physics.OverlapSphereNonAlloc(transform.position, Range, OverlapBuffer);
        for (int i = 0; i < HitCount; i++)
        {
            CharacterStats TargetStats = OverlapBuffer[i].GetComponent<CharacterStats>();
            if(TargetStats == null) continue;
            if(TargetScoringLibrary.IsValidTarget(TargetStats, Myself)) CandidateBuffer.Add(TargetStats);
        }
    }



    CharacterStats SelectBestTarget(List<CharacterStats> Candidates, GA_WeaponBase Weapon, CharacterStats Self, bool IsForChase)
    {
        if(Candidates.Count == 0) return null;
        CharacterStats BestTarget = null;

        float BestScore = float.MinValue;
        float Range = IsForChase ? Weapon.GetChaseRange() : Weapon.GetShootRange();
        foreach(var Candidate in Candidates)
        {
            float CurrentScore = 0f;
            if(!Weapon.ScoreTarget(Candidate, Self, ref CurrentScore, Range)) continue;

            if(AbilitySystem.Tags.HasTagExact(new FGameTag("逻辑.索敌.优先轻甲")))
            {
                TargetScoringLibrary.ScoreByArmorMatch(Candidate, ref CurrentScore, CharacterStats.ArmorType.Light, 2f);
            }
            else if(AbilitySystem.Tags.HasTagExact(new FGameTag("逻辑.索敌.优先重甲")))
            {
                TargetScoringLibrary.ScoreByArmorMatch(Candidate, ref CurrentScore, CharacterStats.ArmorType.Heavy, 2f);
            }
            


            if(AbilitySystem.Tags.HasTagExact(new FGameTag("逻辑.索敌.优先低血量护甲")))
            {
                TargetScoringLibrary.ScoreByHealthShield(Candidate, ref CurrentScore, true, 1f);
            }
            else if(AbilitySystem.Tags.HasTagExact(new FGameTag("逻辑.索敌.优先高血量护甲")))
            {
                TargetScoringLibrary.ScoreByHealthShield(Candidate, ref CurrentScore, true, -1f);
            }
            else if(AbilitySystem.Tags.HasTagExact(new FGameTag("逻辑.索敌.优先低血量")))
            {
                TargetScoringLibrary.ScoreByHealthShield(Candidate, ref CurrentScore, false, 11f);
            }
            else if(AbilitySystem.Tags.HasTagExact(new FGameTag("逻辑.索敌.优先高血量")))
            {
                TargetScoringLibrary.ScoreByHealthShield(Candidate, ref CurrentScore, false, -1f);
            }

            




            if(CurrentScore > BestScore)
            {
                BestScore = CurrentScore;
                BestTarget = Candidate;
            }
        }

        return BestTarget;
    }

}
