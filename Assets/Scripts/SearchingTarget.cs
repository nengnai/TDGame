using UnityEngine;

public class SearchingTarget : MonoBehaviour
{
    public float ScanDur = 0.1f;             //检测间隔(sec)



    private CharacterStats CurrentTarget;
    private CharacterStats FocusTarget;
    private readonly Collider[] OverlapBuffer = new Collider[100];
    private float LastScanTime;

    

    public CharacterStats Target => FocusTarget ?? CurrentTarget;


    void Update()
    {
        if(Time.time - LastScanTime >= ScanDur)
        {
            LastScanTime = Time.time;
            ScanForTarget();
            
            if(!IsCompliant(CurrentTarget))
            {
                CurrentTarget = null;
            }
            if (!IsCompliant(FocusTarget))
            {
                FocusTarget = null;
            }
        }
    }



    bool IsCompliant(CharacterStats Target)
    {
        if(Target == null) return false;
        if(Target.IsDead) return false;
        if(Target.gameObject == null) return false;
        return true;
    }


    void ScanForTarget()
    {
        CharacterStats MyStats = GetComponent<CharacterStats>();
        if(MyStats == null) return;
        int HitCount = Physics.OverlapSphereNonAlloc(transform.position, MyStats.DetectRange, OverlapBuffer);                     //之后加一个layer过滤

        float ClosestDistance = float.MaxValue;
        CharacterStats ClosestTarget = null;
        for (int i = 0; i < HitCount; i++)
        {
            CharacterStats TargetStats = OverlapBuffer[i].GetComponent<CharacterStats>();
            if(TargetStats == null) continue;
            if(TargetStats == MyStats) continue;
            if(TargetStats.IsDead) continue;
            if(TargetStats.TeamID == MyStats.TeamID) continue;
            float Distance = Vector3.Distance(transform.position, TargetStats.transform.position);
            if(Distance < ClosestDistance)
            {
                ClosestTarget = TargetStats;
                ClosestDistance = Distance;
            }
        }
        CurrentTarget = ClosestTarget;

    }


    public void SetFocusTarget(CharacterStats Target)
    {
        FocusTarget = Target;
    }

    public void ClearFocusTarget()
    {
        FocusTarget = null;
    }

}
