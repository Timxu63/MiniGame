using System.Collections.Generic;

namespace HotFixBattle.AI
{
    /// <summary>
    /// 群体怪物行为组配置
    /// </summary>
    public static class GroupMonsterBehaviorGroup
    {
        /// <summary>
        /// 创建群体怪物行为组
        /// </summary>
        /// <returns>群体怪物行为组</returns>
        public static AIBehaviorGroup Create()
        {
            return new AIBehaviorGroup
            {
                Name = "GroupMonster",
                States = new List<string> { "Patrol", "Chase", "Attack" },
                DecisionRules = new List<string> 
                { 
                    "LowHealthFlee:0.2", 
                    "AttackTargetInRange", 
                    "GroupAttack", 
                    "PatrolWhenNoTarget" 
                }
            };
        }
    }
}