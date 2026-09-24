using UnityEngine;

public static class TargetScoringLibrary
{

    public static bool IsValidTarget(CharacterStats Target, CharacterStats Self)
    {
        if(Target == null) return false;
        if(!IsAlive(Target)) return false;
        if(Target == Self) return false;
        if(!IsEnemyTeam(Target, Self)) return false;
        return true;
    }
    public static bool IsEnemyTeam(CharacterStats Target, CharacterStats Self)
    {
        if(Target.TeamID == Self.TeamID) return false;

        return true;
    }

    public static bool IsAlive(CharacterStats Target)
    {
        if(Target == null) return false;
        if(Target.IsDead) return false;
        return true;
    }


    public static void ScoreByDistance(CharacterStats Target, CharacterStats Self, ref float Score, float MaxRange, float Weight)
    {
        float Distance = Vector3.Distance(Target.transform.position, Self.transform.position);
        float Normalized = 1f - Mathf.Clamp01(Distance/MaxRange);
        Score += Normalized * Weight;
    }

    public static void ScoreByHealthShield(CharacterStats Target, ref float Score,  bool Shield, float Weight)
    {
        float Health = 1f / Mathf.Max(1f, Target.CurrentHealth + Target.CurrentShield * (Shield ? 1f : 0f));
        Score += Health * Weight;
    }

    public static void ScoreByArmorMatch(CharacterStats Target, ref float Score, CharacterStats.ArmorType Armor, float BonusScore = 2f)
    {
        if(Target.ArmorKind == Armor) Score += BonusScore;
    }

}
