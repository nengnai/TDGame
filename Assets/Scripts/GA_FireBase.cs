using UnityEngine;

public class GA_FireBase : GA_WeaponBase
{
    public float AttackWindUpTime;
    public float AttackWindDownTime;
    public float ShootDelay;                //攻击间隔
    public int BurstTime;                  //一轮最大连发次数
    private int BurstCount;
    public int BurstDelay;                 //连发后间隔
    


    FTimerHandle ShootLoopHandle;
    SearchingTarget SearchingSystem;
    CharacterStats MyStats;



    public int AmmoCostPerShot;




    public override void OnGranted()
    {
        base.OnGranted();

        SearchingSystem = Owner.GetComponent<SearchingTarget>();
        MyStats = Owner.GetComponent<CharacterStats>();
        BurstCount = 0;
    }


    public override void Activate()
    {
        StartFiringLoop();
    }



    public override void Cancel()
    {
        //收枪
        base.Cancel();
        StopFiringLoop();
    }


    void Shoot()
    {
        SearchingSystem.ScanWithWeapon(this);
        CharacterStats Target = SearchingSystem.AttackTarget;

        if(Target == null)return;

        if(MyStats.CurrentAmmo <= 0)
        {
            StopFiringLoop();
             //之后接转换弹技能
            return;
        }

        DealDamage(Target);
        MyStats.CurrentAmmo -= AmmoCostPerShot;
        BurstCount++;


        if(BurstCount >= BurstTime)
        {
            TimeManager.RemoveTimer(ShootLoopHandle);
            TimeManager.AddTimer(BurstDelay, false, false, () => 
            {
                BurstCount = 0;
                StartFiringLoop();
            });
        }
    }




    void DealDamage(CharacterStats Target)
    {
        int Damage = MyStats.Damage;

        Target.CurrentHealth -= Damage;
        Target.DeathDetection();
    }

    void StartFiringLoop()
    {
        ShootLoopHandle = TimeManager.AddTimer(ShootDelay, true, false, Shoot);
        Shoot();
    }

    void StopFiringLoop()
    {
        if(ShootLoopHandle.IsValid) TimeManager.RemoveTimer(ShootLoopHandle);
    }


}
