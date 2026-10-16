using System.Collections.Generic;
using UnityEngine;

public class tester : MonoBehaviour
{
    //这个脚本是给单位归属玩家用的 这样可以选中单位进行测试了
    public List<GameObject> 单位;
    public void 变成玩家单位()
    {
        foreach(GameObject gameObject in 单位)
        {
            AbilitySystemComponent ASC = gameObject.GetComponentInChildren<AbilitySystemComponent>();
            ASC.Tags.AddTag(TDGameTags.IsPlayer);
        }
        Debug.Log("已归属");
    }

    public void 测试按钮()
{
    Debug.Log("按钮点击事件正常！");
}
}
