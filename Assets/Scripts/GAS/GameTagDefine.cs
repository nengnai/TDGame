// 定义基础 Tag

public static class TDGameTags
{
    /* ---- Modify变量 ---- */
    public static readonly FGameTag MaxHealth = new FGameTag("Modify.Stats.MaxHealth");
    public static readonly FGameTag Health = new FGameTag("Modify.Stats.Health");
    public static readonly FGameTag Damage = new FGameTag("Modify.Stats.Damage");
    // .....
    
    /* ---- 普攻相关标签 ---- */
    public static readonly FGameTag FireReady = new FGameTag("Game.Fire.FireReady");
    
    /* ---- 状态相关标签 ---- */
    public static readonly FGameTag IsMove = new FGameTag("Game.Stats.IsMove");
}