using Unity.Mathematics;
using UnityEngine;

public class BattleScripts
{
    public static bool TryFindTarget(Vector3 pos, float range, LayerMask mask, out Transform Target, out CharacterStat targetStat)               //寻敌 之后拉出去单独做脚本
    {
        Target = null;
        targetStat = null;
        float closestDis = float.MaxValue;
        Collider[] hits = Physics.OverlapSphere(pos, range, mask);
        for (int i = 0; i < hits.Length; i++)
        {
            Vector3 diff = hits[i].transform.position - pos;
            diff.y = 0f;
            float dist = diff.sqrMagnitude;
            if(dist < closestDis)
            {
                CharacterStat stat = hits[i].GetComponentInChildren<CharacterStat>();
                if(stat != null && !stat.isDead)
                {
                    closestDis = dist;
                    Target = hits[i].transform;
                    targetStat = stat;
                }
            }
        }
        return Target != null;
    }

    public static void FaceTarget(Transform self, Vector3 targetPos, float turnSpeed)                   //面向敌人 同上
    {
        Vector3 dir = targetPos - self.position;
        dir.y = 0;


        if(dir.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        self.rotation = Quaternion.Slerp(self.rotation, targetRot, turnSpeed * Time.deltaTime);
    }
}
