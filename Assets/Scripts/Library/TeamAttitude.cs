// 声明用于保存所属阵营的类型，与判断阵营之间关系

using UnityEngine;

namespace TDGameLibrary
{
    public enum ETeamAttitude
    {
        Neutral,
        Hostile,
        Friendly
    }
    
    public struct FTeamID
    {
        private uint ID;
        
        public FTeamID(uint InID)
        {
            ID = InID;
        }
        
        public void SetTeamID(uint InID)
        {
            ID = InID;
        }
        
        public ETeamAttitude GetTargetAttitude(FTeamID TargetTeamID)
        {
            // todo:以后修改为根据字典等方式判断阵营的对立关系
            if (TargetTeamID.ID == ID) return ETeamAttitude.Friendly;
            return ETeamAttitude.Hostile;
        }
    }
}