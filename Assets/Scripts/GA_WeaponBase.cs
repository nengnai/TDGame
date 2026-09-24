
public class GA_WeaponBase : GameAbility
{
    public float ShootRange;
    public float ChaseRange;


    public virtual float GetShootRange()
    {
        return ShootRange;
    }

    public virtual float GetChaseRange()
    {
        return ChaseRange;
    }

    public virtual bool ScoreTarget(CharacterStats Target, CharacterStats Self, ref float Score, float Range)
    {
        if(!TargetScoringLibrary.IsEnemyTeam(Target, Self)) return false;
        if(!TargetScoringLibrary.IsAlive(Target)) return false;
        TargetScoringLibrary.ScoreByDistance(Target, Self, ref Score, Range, 1f);
        return true;
    }

}
