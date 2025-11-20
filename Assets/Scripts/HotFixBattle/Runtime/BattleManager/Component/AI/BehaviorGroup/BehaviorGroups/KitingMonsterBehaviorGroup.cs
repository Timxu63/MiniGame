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
                States = new List<string> 
                { 
                    AIStringMappings.StateTypeToString[AIStateType.Patrol],
                    AIStringMappings.StateTypeToString[AIStateType.Chase],
                    AIStringMappings.StateTypeToString[AIStateType.Attack]
                },
                DecisionRules = new List<string> 
                { 
                    $"{AIStringMappings.DecisionRuleTypeToString[AIDecisionRuleType.LowHealthFlee]}:0.2", 
                    AIStringMappings.DecisionRuleTypeToString[AIDecisionRuleType.AttackTargetInRange], 
                    AIStringMappings.DecisionRuleTypeToString[AIDecisionRuleType.PatrolWhenNoTarget] 
                }
            };
        }
    }
}