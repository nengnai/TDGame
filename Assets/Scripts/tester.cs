using System.Collections.Generic;
using UnityEngine;

public class tester : MonoBehaviour
{
    //这个脚本是给单位归属玩家用的 这样可以选中单位进行测试了
    public List<GameObject> 单位;
    public List<GameObject> 敌方单位;

    public void 变成玩家单位()
    {
        foreach(GameObject gameObject in 单位)
        {
            AbilitySystemComponent ASC = gameObject.GetComponentInChildren<AbilitySystemComponent>();
            CharacterStats CS = gameObject.GetComponentInChildren<CharacterStats>();
            ASC.Tags.AddTag(TDGameTags.IsPlayer);
            ASC.Tags.AddTag(TDGameTags.Search优先近距离);
            CS.TeamID.SetTeamID(0);
        }
        Debug.Log("已归属");
    }

    public void 敌军单位部署()
    {
        foreach(GameObject gameObject in 敌方单位)
        {
            AbilitySystemComponent ASC = gameObject.GetComponentInChildren<AbilitySystemComponent>();
            CharacterStats CS = gameObject.GetComponentInChildren<CharacterStats>();
            ASC.Tags.RemoveTag(TDGameTags.IsPlayer);
            ASC.Tags.AddTag(TDGameTags.Search优先近距离);
            CS.TeamID.SetTeamID(1);
        }
        Debug.Log("敌军已部署");
    }

}
