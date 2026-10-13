// 定义基础 Tag

public static class TDGameTags
{
    /* ---- Modify变量 ---- */
    public static readonly FGameTag MaxHealth = new("Modify.Stats.MaxHealth");
    public static readonly FGameTag Health = new("Modify.Stats.Health");               //*CurrentHealth
    public static readonly FGameTag MaxShield  = new("Modify.Stats.MaxShield");
    public static readonly FGameTag Shield  = new("Modify.Stats.Shield");              //*CurrentShield
    public static readonly FGameTag Armor = new("Modify.Stats.Armor");
    public static readonly FGameTag MoveSpeed = new("Modify.Stats.MoveSpeed");
    public static readonly FGameTag ArmorKind = new("Modify.Stats.ArmorKind");
    public static readonly FGameTag ExplosionDmg = new("Modify.Stats.ExplosionDmg");
    public static readonly FGameTag PenetrationDmg = new("Modify.Stats.PenetrationDmg");
    public static readonly FGameTag EnergyDmg = new("Modify.Stats.EnergyDmg");
    public static readonly FGameTag DetectRange = new("Modify.Stats.DetectRange");
    public static readonly FGameTag ShootRange = new("Modify.Stats.ShootRange");
    public static readonly FGameTag Damage = new("Modify.Stats.Damage");
    public static readonly FGameTag ShootDelay = new("Modify.Stats.ShootDelay");
    public static readonly FGameTag BurstDelay = new("Modify.Stats.BurstDelay");
    // .....
    
    /* ---- 普攻 ---- */
    public static readonly FGameTag FireReady = new("Game.Fire.FireReady");
    
    /* ---- 状态 ---- */
    public static readonly FGameTag IsMove = new("Game.Stats.IsMove");
    
    /* ---- 索敌逻辑 ---- */
    public static readonly FGameTag Search优先近距离 = new("Logic.Search.优先近距离");
    public static readonly FGameTag Search优先远距离 = new("Logic.Search.优先远距离");
    public static readonly FGameTag Search优先轻甲 = new("Logic.Search.优先轻甲");
    public static readonly FGameTag Search优先重甲 = new("Logic.Search.优先重甲");
    public static readonly FGameTag Search优先低血量护甲 = new("Logic.Search.优先低血量护甲");
    public static readonly FGameTag Search优先高血量护甲 = new("Logic.Search.优先高血量护甲");
    public static readonly FGameTag Search优先低血量 = new("Logic.Search.优先低血量");
    public static readonly FGameTag Search优先高血量 = new("Logic.Search.优先高血量");
    
    /* ---- 动画事件 ---- */
    public static readonly FGameTag AnimatorFireEvent = new("Animator.Event.Fire");
}