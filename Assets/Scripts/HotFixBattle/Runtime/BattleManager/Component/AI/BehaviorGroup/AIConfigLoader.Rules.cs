using System;
using System.Collections.Generic;

namespace HotFixBattle.AI
{
    /// <summary>
    /// AI配置加载器 - 决策规则工厂部分
    /// </summary>
    public static partial class AIConfigLoader
    {
        /// <summary>
        /// AI决策规则工厂字典，根据名称创建对应的决策规则
        /// </summary>
        private static readonly Dictionary<string, Func<string[], AIDecisionRule>> _decisionRuleFactories =
            new Dictionary<string, Func<string[], AIDecisionRule>>
            {
                // 基础决策规则
                { "LowHealthFlee", CreateLowHealthFleeRule },
                { "AttackTargetInRange", CreateAttackTargetInRangeRule },
                { "MoveToTarget", CreateMoveToTargetRule },
                { "PatrolWhenNoTarget", CreatePatrolWhenNoTargetRule },
                { "DefaultIdle", CreateDefaultIdleRule },
            };

        /// <summary>
        /// 创建低血量逃跑决策规则
        /// </summary>
        /// <param name="parameters">参数数组，第一个元素为血量阈值</param>
        /// <returns>决策规则实例</returns>
        private static AIDecisionRule CreateLowHealthFleeRule(string[] parameters)
        {
            float threshold = parameters.Length > 1 && float.TryParse(parameters[1], out float parsedValue) ? parsedValue : 0.3f;
            return AIDecisionRules.LowHealthFlee(threshold);
        }

        /// <summary>
        /// 创建攻击范围内目标决策规则
        /// </summary>
        /// <param name="parameters">参数数组</param>
        /// <returns>决策规则实例</returns>
        private static AIDecisionRule CreateAttackTargetInRangeRule(string[] parameters)
        {
            return AIDecisionRules.AttackTargetInRange();
        }

        /// <summary>
        /// 创建移动到目标决策规则
        /// </summary>
        /// <param name="parameters">参数数组</param>
        /// <returns>决策规则实例</returns>
        private static AIDecisionRule CreateMoveToTargetRule(string[] parameters)
        {
            return AIDecisionRules.MoveToTarget();
        }

        /// <summary>
        /// 创建无目标时巡逻决策规则
        /// </summary>
        /// <param name="parameters">参数数组</param>
        /// <returns>决策规则实例</returns>
        private static AIDecisionRule CreatePatrolWhenNoTargetRule(string[] parameters)
        {
            return AIDecisionRules.PatrolWhenNoTarget();
        }

        /// <summary>
        /// 创建默认待机决策规则
        /// </summary>
        /// <param name="parameters">参数数组</param>
        /// <returns>决策规则实例</returns>
        private static AIDecisionRule CreateDefaultIdleRule(string[] parameters)
        {
            return AIDecisionRules.DefaultIdle();
        }

        /// <summary>
        /// 尝试获取AI决策规则工厂
        /// </summary>
        /// <param name="name">规则名称</param>
        /// <param name="factory">规则工厂方法</param>
        /// <returns>是否找到工厂</returns>
        public static bool TryGetDecisionRuleFactory(string name, out Func<string[], AIDecisionRule> factory)
        {
            return _decisionRuleFactories.TryGetValue(name, out factory);
        }

        /// <summary>
        /// 注册新的AI决策规则工厂
        /// </summary>
        /// <param name="name">规则名称</param>
        /// <param name="factory">规则工厂方法</param>
        public static void RegisterDecisionRuleFactory(string name, Func<string[], AIDecisionRule> factory)
        {
            _decisionRuleFactories[name] = factory;
        }
    }
}