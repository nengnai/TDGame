// 定义基础 Tag

public static class TDGameTags
{
    /* ---- Modify变量 ---- */
    public static readonly FGameTag MaxHealth = new FGameTag("Modify.Stats.MaxHealth");
    public static readonly FGameTag Health = new FGameTag("Modify.Stats.Health");               //*CurrentHealth
    public static readonly FGameTag MaxShield  = new FGameTag("Modify.Stats.MaxShield");
    public static readonly FGameTag Shield  = new FGameTag("Modify.Stats.Shield");              //*CurrentShield
    public static readonly FGameTag Armor = new FGameTag("Modify.Stats.Armor");
    public static readonly FGameTag MoveSpeed = new FGameTag("Modify.Stats.MoveSpeed");
    public static readonly FGameTag ArmorKind = new FGameTag("Modify.Stats.ArmorKind");
    public static readonly FGameTag ExplosionDmg = new FGameTag("Modify.Stats.ExplosionDmg");
    public static readonly FGameTag PenetrationDmg = new FGameTag("Modify.Stats.PenetrationDmg");
    public static readonly FGameTag EnergyDmg = new FGameTag("Modify.Stats.EnergyDmg");
    public static readonly FGameTag DetectRange = new FGameTag("Modify.Stats.DetectRange");
    public static readonly FGameTag ShootRange = new FGameTag("Modify.Stats.ShootRange");
    public static readonly FGameTag Damage = new FGameTag("Modify.Stats.Damage");
    public static readonly FGameTag ShootDelay = new FGameTag("Modify.Stats.ShootDelay");
    public static readonly FGameTag BurstDelay = new FGameTag("Modify.Stats.BurstDelay");
    // .....
    
    /* ---- 普攻相关标签 ---- */
    public static readonly FGameTag FireReady = new FGameTag("Game.Fire.FireReady");
    
    /* ---- 状态相关标签 ---- */
    public static readonly FGameTag IsMove = new FGameTag("Game.Stats.IsMove");
}