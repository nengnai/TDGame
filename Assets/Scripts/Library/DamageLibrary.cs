// 定义了伤害等结构

namespace TDGameLibrary
{
    public enum ArmorType 
    {
        Null,
        Light,
        Medium,
        Heavy
    }
    
    [System.Serializable]
    public struct FDamageDefine
    {
        public float Damage;
        public float VS轻甲;
        public float VS中甲;
        public float VS重甲;

        public FDamageDefine(float BaseDamage)
        {
            Damage = BaseDamage;
            VS轻甲 = 0;
            VS中甲 = 0;
            VS重甲 = 0;
        }

        public float GetCurrentDamage(ArmorType TargetArmorType)
        {
            float ResultDamage = Damage;
            switch (TargetArmorType)
            {
                case ArmorType.Light:
                    ResultDamage += VS轻甲;
                    break;

                case ArmorType.Medium:
                    ResultDamage += VS中甲;
                    break;

                case ArmorType.Heavy:
                    ResultDamage += VS重甲;
                    break;
            }
            return ResultDamage;
        }
    }
}