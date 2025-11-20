using System.Collections.Generic;

namespace HotFixBattle.AI
{
    /// <summary>
    /// AI枚举到字符串的映射
    /// </summary>
    public static class AIStringMappings
    {
        /// <summary>
        /// 状态类型到字符串的映射
        /// </summary>
        public static readonly Dictionary<AIStateType, string> StateTypeToString = new Dictionary<AIStateType, string>
        {
            { AIStateType.Patrol, "Patrol" },
            { AIStateType.Chase, "Chase" },
            { AIStateType.Attack, "Attack" },
            { AIStateType.Flee, "Flee" }
        };

        /// <summary>
        /// 行为类型到字符串的映射
        /// </summary>
        public static readonly Dictionary<AIBehaviorType, string> BehaviorTypeToString = new Dictionary<AIBehaviorType, string>
        {
            { AIBehaviorType.MoveToTarget, "MoveToTarget" },
            { AIBehaviorType.RandomMove, "RandomMove" },
            { AIBehaviorType.Wait, "Wait" },
            { AIBehaviorType.Attack, "Attack" },
            { AIBehaviorType.AIFindTarget, "AIFindTarget" },
            // { AIBehaviorType.AggressiveMoveToTarget, "AggressiveMoveToTarget" },
            // { AIBehaviorType.SmartMoveToTarget, "SmartMoveToTarget" },
            // { AIBehaviorType.AggressiveChase, "AggressiveChase" },
            // { AIBehaviorType.FlickerMove, "FlickerMove" },
            // { AIBehaviorType.PersistentChase, "PersistentChase" },
            // { AIBehaviorType.KitingAttack, "KitingAttack" },
            // { AIBehaviorType.GroupAttack, "GroupAttack" },
            // { AIBehaviorType.PowerAttack, "PowerAttack" }
        };

        /// <summary>
        /// 决策规则类型到字符串的映射
        /// </summary>
        public static readonly Dictionary<AIDecisionRuleType, string> DecisionRuleTypeToString = new Dictionary<AIDecisionRuleType, string>
        {
            { AIDecisionRuleType.LowHealthFlee, "LowHealthFlee" },
            { AIDecisionRuleType.AttackTargetInRange, "AttackTargetInRange" },
            { AIDecisionRuleType.MoveToTarget, "MoveToTarget" },
            { AIDecisionRuleType.PatrolWhenNoTarget, "PatrolWhenNoTarget" },
            { AIDecisionRuleType.DefaultIdle, "DefaultIdle" },
            // { AIDecisionRuleType.AggressiveChase, "AggressiveChase" },
            // { AIDecisionRuleType.PersistentChase, "PersistentChase" },
            // { AIDecisionRuleType.KitingAttack, "KitingAttack" },
            // { AIDecisionRuleType.GroupAttack, "GroupAttack" },
            // { AIDecisionRuleType.AggressiveMoveToTarget, "AggressiveMoveToTarget" },
            // { AIDecisionRuleType.SmartMoveToTarget, "SmartMoveToTarget" },
            // { AIDecisionRuleType.FlickerMove, "FlickerMove" },
            // { AIDecisionRuleType.PowerAttack, "PowerAttack" }
        };
    }
}