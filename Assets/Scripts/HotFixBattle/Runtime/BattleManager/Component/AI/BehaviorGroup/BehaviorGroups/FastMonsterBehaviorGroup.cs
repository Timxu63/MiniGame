using System.Collections.Generic;

namespace HotFixBattle.AI
{
    /// <summary>
    /// 快速怪物行为组配置
    /// </summary>
    public static class FastMonsterBehaviorGroup
    {
        /// <summary>
        /// 创建快速怪物行为组
        /// </summary>
        /// <returns>快速怪物行为组</returns>
        public static AIBehaviorGroup Create()
        {
            return new AIBehaviorGroup
            {
                Name = "FastMonster",
                States = new List<string> { "Patrol", "Chase", "Attack" },
                DecisionRules = new List<string> 
                { 
                    "LowHealthFlee:0.2", 
                    "AttackTargetInRange", 
                    "AggressiveChase", 
                    "FlickerMove", 
                    "PatrolWhenNoTarget" 
                }
            };
        }
    }
}