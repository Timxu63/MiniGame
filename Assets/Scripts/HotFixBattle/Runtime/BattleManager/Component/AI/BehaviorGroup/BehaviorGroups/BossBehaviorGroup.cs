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
                States = new List<string> 
                { 
                    AIStringMappings.StateTypeToString[AIStateType.Patrol],
                    AIStringMappings.StateTypeToString[AIStateType.Chase],
                    AIStringMappings.StateTypeToString[AIStateType.Attack],
                    AIStringMappings.StateTypeToString[AIStateType.Flee]
                },
                DecisionRules = new List<string>
                {
                    $"{AIStringMappings.DecisionRuleTypeToString[AIDecisionRuleType.LowHealthFlee]}:0.1",
                    AIStringMappings.DecisionRuleTypeToString[AIDecisionRuleType.AttackTargetInRange],
                    AIStringMappings.DecisionRuleTypeToString[AIDecisionRuleType.PatrolWhenNoTarget],
                    AIStringMappings.DecisionRuleTypeToString[AIDecisionRuleType.DefaultIdle]
                }
            };
        }
    }
}