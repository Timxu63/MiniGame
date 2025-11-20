using System.Collections.Generic;

namespace HotFixBattle.AI
{
    /// <summary>
    /// 耐心怪物行为组配置
    /// </summary>
    public static class PatientMonsterBehaviorGroup
    {
        /// <summary>
        /// 创建耐心怪物行为组
        /// </summary>
        /// <returns>耐心怪物行为组</returns>
        public static AIBehaviorGroup Create()
        {
            return new AIBehaviorGroup
            {
                Name = "PatientMonster",
                States = new List<string> { "Patrol", "Chase", "Attack" },
                DecisionRules = new List<string> 
                { 
                    "LowHealthFlee:0.2", 
                    "AttackTargetInRange", 
                    "PersistentChase", 
                    "PatrolWhenNoTarget" 
                }
            };
        }
    }
}