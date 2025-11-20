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
                States = new List<string> 
                { 
                    AIStringMappings.StateTypeToString[AIStateType.Patrol],
                    AIStringMappings.StateTypeToString[AIStateType.Chase],
                    AIStringMappings.StateTypeToString[AIStateType.Attack],
                    AIStringMappings.StateTypeToString[AIStateType.Flee]
                },
                DecisionRules = new List<string> 
                { 
                    $"{AIStringMappings.DecisionRuleTypeToString[AIDecisionRuleType.LowHealthFlee]}:0.3", 
                    AIStringMappings.DecisionRuleTypeToString[AIDecisionRuleType.AttackTargetInRange], 
                    AIStringMappings.DecisionRuleTypeToString[AIDecisionRuleType.MoveToTarget], 
                    AIStringMappings.DecisionRuleTypeToString[AIDecisionRuleType.PatrolWhenNoTarget], 
                    AIStringMappings.DecisionRuleTypeToString[AIDecisionRuleType.DefaultIdle] 
                }
            };
        }
    }
}