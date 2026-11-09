// 用于目标搜索等功能的组件

using System;
using System.Collections.Generic;
using TDGameLibrary;
using UnityEngine;

public class SearchingTarget : MonoBehaviour
{
    public float ScanDur = 0.1f;
    public float ReCheckTargetDur = 60.0f;
    
    private CharacterStats Myself;
    private CharacterStats MyTarget;
    
    private Collider[] OverlapBuffer = new Collider[100];
    private List<CharacterStats> CandidateBuffer = new();
    
    private AbilitySystemComponent AbilitySystem;
    
    
    private FTimerHandle UpdateLogicTimer;


    // 搜索缓存
    struct SearchingResult
    {
        public SearchingResult(CharacterStats InTarget)
        {
            Target = InTarget;
            Time = UnityEngine.Time.time;
        }
        
        public CharacterStats Target;
        public float Time;
    }
    private Dictionary<GA_BaseAttack, SearchingResult> SearchingCaches = new();
    private List<GA_BaseAttack> _removeBuffer = new();


    void Awake()
    {
        Myself = GetComponent<CharacterStats>();
        AbilitySystem = GetComponent<AbilitySystemComponent>();

        UpdateLogicTimer = TimerSubsystem.GetSubsystem().AddTimer(ReCheckTargetDur, true, false,
            () =>
            {
                if(!IsTargetValid(MyTarget))  MyTarget = null;
                
                _removeBuffer.Clear();
                foreach (var Cache in SearchingCaches)
                {
                    if(!IsTargetValid(Cache.Value.Target)) _removeBuffer.Add(Cache.Key);
                }
                
                foreach (var key in _removeBuffer) SearchingCaches.Remove(key);
                _removeBuffer.Clear();
            });
    }

    private void OnDestroy()
    {
        UpdateLogicTimer.RemoveTimer();
    }


    bool IsTargetValid(CharacterStats Target)
    {
        return TargetScoringLibrary.IsAlive(Target);
    }




    public CharacterStats ScanWithWeapon(GA_BaseAttack Weapon)
    {
        if(!Weapon) return null;
        if(!Myself) return null;

        SearchingResult Cache;
        if (SearchingCaches.TryGetValue(Weapon, out Cache))
        {
            if(Time.time - Cache.Time < ScanDur) return Cache.Target;
        }

        CollectCandidatesInRange(Weapon.GetShootRange());
        if (CandidateBuffer.Count > 0)
        {
            Cache = new(SelectNormalShootTarget(CandidateBuffer, Weapon, Myself));
        }
        else
        {
            CollectCandidatesInRange(Weapon.GetDetectRange());
            Cache = new(SelectNormalShootTarget(CandidateBuffer, Weapon, Myself));
        }
        SearchingCaches[Weapon] = Cache;

        //@todo:不应该将所有武器的结果都设置为全局目标吧？！
        MyTarget = Cache.Target;
        
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
                TargetScoringLibrary.ScoreByArmorMatch(Candidate, ref CurrentScore, ArmorType.Light, 2f);
            }
            else if(AbilitySystem.Tags.HasTagExact(TDGameTags.Search优先重甲))
            {
                TargetScoringLibrary.ScoreByArmorMatch(Candidate, ref CurrentScore, ArmorType.Heavy, 2f);
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
