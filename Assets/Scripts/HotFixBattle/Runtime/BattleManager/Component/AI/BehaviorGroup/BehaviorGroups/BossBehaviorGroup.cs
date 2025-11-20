using System.Collections.Generic;

namespace HotFixBattle.AI
{
    /// <summary>
    /// Boss行为组配置
    /// </summary>
    public static class BossBehaviorGroup
    {
        /// <summary>
        /// 创建Boss行为组
        /// </summary>
        /// <returns>Boss行为组</returns>
        public static AIBehaviorGroup Create()
        {
            return new AIBehaviorGroup
            {
                Name = "Boss",
                States = new List<string> { "Patrol", "Chase", "Attack", "Flee" },
                DecisionRules = new List<string> 
                { 
                    "LowHealthFlee:0.1", 
                    "AttackTargetInRange", 
                    "PatrolWhenNoTarget", 
                    "DefaultIdle" 
                }
            };
        }
    }
}