using System.Collections.Generic;

namespace HotFixBattle.AI
{
    /// <summary>
    /// 普通怪物行为组配置
    /// </summary>
    public static class NormalMonsterBehaviorGroup
    {
        /// <summary>
        /// 创建普通怪物行为组
        /// </summary>
        /// <returns>普通怪物行为组</returns>
        public static AIBehaviorGroup Create()
        {
            return new AIBehaviorGroup
            {
                Name = "NormalMonster",
                States = new List<string> { "Patrol", "Chase", "Attack", "Flee" },
                DecisionRules = new List<string> 
                { 
                    "LowHealthFlee:0.3", 
                    "AttackTargetInRange", 
                    "MoveToTarget", 
                    "PatrolWhenNoTarget", 
                    "DefaultIdle" 
                }
            };
        }
    }
}