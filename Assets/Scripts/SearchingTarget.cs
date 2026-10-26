
using System.Collections.Generic;
using UnityEngine;

public class SearchingTarget : MonoBehaviour
{
    private CharacterStats Myself;


    private CharacterStats MyTarget;


    private Collider[] OverlapBuffer = new Collider[100];
    private List<CharacterStats> CandidateBuffer = new();







    private AbilitySystemComponent AbilitySystem;


    void Awake()
    {
        Myself = GetComponent<CharacterStats>();
        AbilitySystem = GetComponent<AbilitySystemComponent>();
    }

    void Update()
    {
        if(!IsTargetValid(MyTarget))  MyTarget = null;
    }



    bool IsTargetValid(CharacterStats Target)
    {
        return TargetScoringLibrary.IsAlive(Target);
    }




    public CharacterStats ScanWithWeapon(GA_BaseAttack Weapon)
    {
        if(Weapon == null) return null;
        if(Myself == null) return null;


        // todo:这里限制时间的逻辑不对，1:没区分武器 2:未到的时候应该返回上次检查的缓存值
        /*if(Time.time - LastScanTime < ScanDur) return null;
        LastScanTime = Time.time;*/



        CollectCandidatesInRange(Weapon.GetShootRange());
        if(CandidateBuffer.Count > 0)
        {
            MyTarget = SelectNormalShootTarget(CandidateBuffer, Weapon, Myself);
            return MyTarget;
        }

        CollectCandidatesInRange(Weapon.GetDetectRange());
        MyTarget = SelectNormalShootTarget(CandidateBuffer, Weapon, Myself);
        return MyTarget;
    }


    void CollectCandidatesInRange(float Range)
    {
        CandidateBuffer.Clear();
        int HitCount = Physics.OverlapSphereNonAlloc(transform.position, Range, OverlapBuffer);
        for (int i = 0; i < HitCount; i++)
        {
            CharacterStats TargetStats = OverlapBuffer[i].GetComponentInChildren<CharacterStats>();
            if(TargetStats == null) continue;
            if(TargetScoringLibrary.IsValidTarget(TargetStats, Myself)) CandidateBuffer.Add(TargetStats);
        }
    }



    CharacterStats SelectNormalShootTarget(List<CharacterStats> Candidates, GA_BaseAttack Weapon, CharacterStats Self)
    {
        if(Candidates.Count == 0) return null;
        CharacterStats BestTarget = null;

        float BestScore = float.MinValue;
        float Range = Weapon.GetDetectRange();
        foreach(var Candidate in Candidates)
        {
            float CurrentScore = 0f;
            if(!TargetScoringLibrary.IsEnemyTeam(Candidate, Self)) continue;
            if(!TargetScoringLibrary.IsAlive(Candidate)) continue;
            if(AbilitySystem.Tags.HasTagExact(TDGameTags.Search优先近距离))
            {
                TargetScoringLibrary.ScoreByDistance(Candidate, Self, ref CurrentScore, Range, 1f);
            }
            else if (AbilitySystem.Tags.HasTagExact(TDGameTags.Search优先远距离))
            {
                TargetScoringLibrary.ScoreByDistance(Candidate, Self, ref CurrentScore, Range, -1f);
            }

            if(AbilitySystem.Tags.HasTagExact(TDGameTags.Search优先轻甲))
            {
                TargetScoringLibrary.ScoreByArmorMatch(Candidate, ref CurrentScore, CharacterStats.ArmorType.Light, 2f);
            }
            else if(AbilitySystem.Tags.HasTagExact(TDGameTags.Search优先重甲))
            {
                TargetScoringLibrary.ScoreByArmorMatch(Candidate, ref CurrentScore, CharacterStats.ArmorType.Heavy, 2f);
            }
            


            if(AbilitySystem.Tags.HasTagExact(TDGameTags.Search优先低血量护甲))
            {
                TargetScoringLibrary.ScoreByHealthShield(Candidate, ref CurrentScore, true, 1f);
            }
            else if(AbilitySystem.Tags.HasTagExact(TDGameTags.Search优先高血量护甲))
            {
                TargetScoringLibrary.ScoreByHealthShield(Candidate, ref CurrentScore, true, -1f);
            }
            else if(AbilitySystem.Tags.HasTagExact(TDGameTags.Search优先低血量))
            {
                TargetScoringLibrary.ScoreByHealthShield(Candidate, ref CurrentScore, false, 11f);
            }
            else if(AbilitySystem.Tags.HasTagExact(TDGameTags.Search优先高血量))
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
