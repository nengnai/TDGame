
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

    

}
