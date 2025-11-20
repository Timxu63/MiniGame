
using System;
using System.Collections.Generic;
using Game.Logic.BattleModule.Entity;

namespace HotFixBattle.AI
{
    /// <summary>
    /// AI决策器，用于根据当前情况做出决策
    /// </summary>
    public class AIDecisionMaker
    {
        /// <summary>
        /// 决策规则列表
        /// </summary>
        private List<AIDecisionRule> _rules = new List<AIDecisionRule>();

        /// <summary>
        /// 添加决策规则
        /// </summary>
        /// <param name="rule">决策规则</param>
        public void AddRule(AIDecisionRule rule)
        {
            if (rule != null)
            {
                _rules.Add(rule);
            }
        }

        /// <summary>
        /// 移除决策规则
        /// </summary>
        /// <param name="rule">决策规则</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveRule(AIDecisionRule rule)
        {
            return _rules.Remove(rule);
        }

        /// <summary>
        /// 清除所有决策规则
        /// </summary>
        public void ClearRules()
        {
            _rules.Clear();
        }

        /// <summary>
        /// 做出决策
        /// </summary>
        /// <param name="ai">AI组件</param>
        /// <returns>决策结果</returns>
        public AIDecision MakeDecision(AIComponent ai)
        {
            // 按优先级从高到低检查规则
            _rules.Sort((a, b) => b.Priority.CompareTo(a.Priority));

            foreach (var rule in _rules)
            {
                if (rule.Condition(ai))
                {
                    return rule.Decision;
                }
            }

            // 如果没有规则匹配，返回默认决策
            return AIDecision.None;
        }
    }

    /// <summary>
    /// AI决策枚举
    /// </summary>
    public enum AIDecision
    {
        None,           // 无决策
        Attack,         // 攻击
        Move,           // 移动
        Flee,           // 逃跑
        Patrol,         // 巡逻
        Idle,           // 待机
        UseSkill,       // 使用技能
        Defend          // 防御
    }

    /// <summary>
    /// AI决策规则
    /// </summary>
    public class AIDecisionRule
    {
        /// <summary>
        /// 规则优先级，数值越高优先级越高
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 条件函数
        /// </summary>
        public Func<AIComponent, bool> Condition { get; set; }

        /// <summary>
        /// 决策结果
        /// </summary>
        public AIDecision Decision { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="priority">优先级</param>
        /// <param name="condition">条件函数</param>
        /// <param name="decision">决策结果</param>
        public AIDecisionRule(int priority, Func<AIComponent, bool> condition, AIDecision decision)
        {
            Priority = priority;
            Condition = condition;
            Decision = decision;
        }
    }

    /// <summary>
    /// 预定义的AI决策规则
    /// </summary>
    public static class AIDecisionRules
    {
        /// <summary>
        /// 生命值低时逃跑
        /// </summary>
        /// <param name="healthThreshold">生命值阈值（0-1）</param>
        /// <returns>决策规则</returns>
        public static AIDecisionRule LowHealthFlee(float healthThreshold = 0.2f)
        {
            return new AIDecisionRule(
                100,
                ai => ai.Owner.CurrentHealth / (float)ai.Owner.MaxHealth < healthThreshold,
                AIDecision.Flee
            );
        }

        /// <summary>
        /// 有目标且在攻击范围内则攻击
        /// </summary>
        /// <returns>决策规则</returns>
        public static AIDecisionRule AttackTargetInRange()
        {
            return new AIDecisionRule(
                80,
                ai => ai.CurrentTarget != null && ai.IsTargetInAttackRange(),
                AIDecision.Attack
            );
        }

        /// <summary>
        /// 有目标但不在攻击范围内则移动
        /// </summary>
        /// <returns>决策规则</returns>
        public static AIDecisionRule MoveToTarget()
        {
            return new AIDecisionRule(
                60,
                ai => ai.CurrentTarget != null && !ai.IsTargetInAttackRange(),
                AIDecision.Move
            );
        }

        /// <summary>
        /// 没有目标则巡逻
        /// </summary>
        /// <returns>决策规则</returns>
        public static AIDecisionRule PatrolWhenNoTarget()
        {
            return new AIDecisionRule(
                40,
                ai => ai.CurrentTarget == null,
                AIDecision.Patrol
            );
        }

        /// <summary>
        /// 默认待机
        /// </summary>
        /// <returns>决策规则</returns>
        public static AIDecisionRule DefaultIdle()
        {
            return new AIDecisionRule(
                0,
                ai => true,
                AIDecision.Idle
            );
        }
    }
}
