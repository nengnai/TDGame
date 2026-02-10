using System;
using UnityEngine;



[Serializable]
public class TowerData
{
    public GameObject 基础塔;
    public int 放置花费;
    public GameObject 一级升级塔;
    public int 一级升级费用;

    public TowerType 塔种类;


    public enum TowerType
    {
        Reisa,
        Misaki
    }

}
