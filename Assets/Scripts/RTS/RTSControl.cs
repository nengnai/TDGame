using UnityEngine;
using UnityEngine.AI;

public class RTScontrol : MonoBehaviour
{
    
    CharacterStat selectedUnit;


    




    public void DeselectUnit()
    {
        if(selectedUnit == null) return;
        SwitchControlable(selectedUnit, false);
        SwitchtheRing(selectedUnit, false);
        selectedUnit = null;
    }
    //取消选中


    public void SelectUnit(CharacterStat targetCharacter)
    {
        if(selectedUnit != null) DeselectUnit();
        selectedUnit = targetCharacter;
        SwitchControlable(targetCharacter, true);
        SwitchtheRing(targetCharacter, true);
    }
    //选中单位



    public void MoveUnit(Vector3 location)
    {
        if(selectedUnit == null) return;
        NavMeshAgent agent = selectedUnit.GetComponentInParent<NavMeshAgent>();
        if(agent != null) agent.SetDestination(location);
    }
    //移动单位



    public void AttackTarget(GameObject target)
    {
        if(selectedUnit == null) return;
        NavMeshAgent agent = selectedUnit.GetComponentInParent<NavMeshAgent>();
        if(agent != null) agent.SetDestination(target.transform.position);  //暂且只做移动到敌人位置 之后换成攻击
    }
    //攻击敌人


    
























    void SwitchControlable(CharacterStat characterStat, bool Switch)
    {
        characterStat.isSelected = Switch;
        if(Switch)
        {
            Debug.Log("已选中" + characterStat.characterName);
        }
        else
        {
            Debug.Log("已取消选中" + characterStat.characterName);
        }
    }


    void SwitchtheRing(CharacterStat characterStat, bool Switch)
    {
        if(characterStat.Ring != null)
        {
             characterStat.Ring.SetActive(Switch);
        }
        else
        {
            Debug.Log(characterStat.characterName + "缺少选中圆环");
        }
    }




}
