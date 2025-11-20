using System.Collections.Generic;

namespace HotFixBattle.AI
{
    /// <summary>
    /// 游击怪物行为组配置
    /// </summary>
    public static class KitingMonsterBehaviorGroup
    {
        /// <summary>
        /// 创建游击怪物行为组
        /// </summary>
        /// <returns>游击怪物行为组</returns>
        public static AIBehaviorGroup Create()
        {
            return new AIBehaviorGroup
            {
                Name = "KitingMonster",
                States = new List<string> { "Patrol", "Chase", "Attack" },
                DecisionRules = new List<string> 
                { 
                    "LowHealthFlee:0.2", 
                    "AttackTargetInRange", 
                    "KitingAttack", 
                    "PatrolWhenNoTarget" 
                }
            };
        }
    }
}