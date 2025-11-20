using System.Collections.Generic;

namespace HotFixBattle.AI
{
    /// <summary>
    /// 精英怪物行为组配置
    /// </summary>
    public static class EliteMonsterBehaviorGroup
    {
        /// <summary>
        /// 创建精英怪物行为组
        /// </summary>
        /// <returns>精英怪物行为组</returns>
        public static AIBehaviorGroup Create()
        {
            return new AIBehaviorGroup
            {
                Name = "EliteMonster",
                States = new List<string> { "Patrol", "Chase", "Attack", "Flee" },
                DecisionRules = new List<string> 
                { 
                    "LowHealthFlee:0.2", 
                    "AttackTargetInRange", 
                    "PatrolWhenNoTarget", 
                    "DefaultIdle" 
                }
            };
        }
    }
}