using System;

namespace HotFixBattle
{
    /// <summary>
    /// 实体状态枚举
    /// </summary>
    [Flags]
    public enum EntityState
    {
        None = 0,
        Normal = 1 << 0,      // 正常状态
        Frozen = 1 << 1,       // 冰冻状态
        Poisoned = 1 << 2,     // 中毒状态
        Stunned = 1 << 3,      // 眩晕状态
        Burning = 1 << 4,      // 燃烧状态
        Bleeding = 1 << 5,     // 流血状态
        Silenced = 1 << 6,     // 沉默状态
        Invincible = 1 << 7,   // 无敌状态
        Invisible = 1 << 8,    // 隐身状态
        Dead = 1 << 9          // 死亡状态
    }
}
