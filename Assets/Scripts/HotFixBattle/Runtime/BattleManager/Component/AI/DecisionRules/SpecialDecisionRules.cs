using System;
using Game.Logic.BattleModule.Entity;
using UnityEngine;

namespace Game.Logic.BattleModule.Component.AI
{
    /// <summary>
    /// 特殊AI决策规则类，用于不同怪物的特殊行为
    /// </summary>
    public static class SpecialDecisionRules
    {
        /// <summary>
        /// 积极追逐规则 - 怪物更积极地追逐目标
        /// </summary>
        /// <param name="ai">AI组件</param>
        /// <returns>决策结果</returns>
        public static AIBehaviorResult AggressiveChase(AIComponent ai)
        {
            if (ai.CurrentTarget == null || !ai.CurrentTarget.IsAlive)
                return AIBehaviorResult.Failure;

            // 计算到目标的距离
            float distance = Vector3.Distance(ai.Owner.LocalPosition, ai.CurrentTarget.LocalPosition);

            // 如果在感知范围内但不在攻击范围内，积极追逐
            if (distance <= ai.PerceptionRange && distance > ai.AttackRange)
            {
                // 直接移动到目标，不等待
                Vector3 direction = (ai.CurrentTarget.LocalPosition - ai.Owner.LocalPosition).normalized;
                ai.Owner.Move(new Vector2(direction.x, direction.z));
                return AIBehaviorResult.Running;
            }

            return AIBehaviorResult.Failure;
        }

        /// <summary>
        /// 持久追逐规则 - 怪物不易放弃目标
        /// </summary>
        /// <param name="ai">AI组件</param>
        /// <returns>决策结果</returns>
        public static AIBehaviorResult PersistentChase(AIComponent ai)
        {
            if (ai.CurrentTarget == null || !ai.CurrentTarget.IsAlive)
                return AIBehaviorResult.Failure;

            // 即使目标稍微超出感知范围，也继续追逐
            float distance = Vector3.Distance(ai.Owner.LocalPosition, ai.CurrentTarget.LocalPosition);

            // 扩大追逐范围
            float extendedRange = ai.PerceptionRange * 1.5f;
            if (distance <= extendedRange && distance > ai.AttackRange)
            {
                Vector3 direction = (ai.CurrentTarget.LocalPosition - ai.Owner.LocalPosition).normalized;
                ai.Owner.Move(new Vector2(direction.x, direction.z));
                return AIBehaviorResult.Running;
            }

            return AIBehaviorResult.Failure;
        }

        /// <summary>
        /// 游击攻击规则 - 保持距离攻击
        /// </summary>
        /// <param name="ai">AI组件</param>
        /// <returns>决策结果</returns>
        public static AIBehaviorResult KitingAttack(AIComponent ai)
        {
            if (ai.CurrentTarget == null || !ai.CurrentTarget.IsAlive)
                return AIBehaviorResult.Failure;

            float distance = Vector3.Distance(ai.Owner.LocalPosition, ai.CurrentTarget.LocalPosition);

            // 保持理想攻击距离，稍微大于攻击范围
            float idealRange = ai.AttackRange * 1.2f;
            if (distance > idealRange)
            {
                Vector3 direction = (ai.CurrentTarget.LocalPosition - ai.Owner.LocalPosition).normalized;
                ai.Owner.Move(new Vector2(direction.x, direction.z));
                return AIBehaviorResult.Running;
            }

            return AIBehaviorResult.Failure;
        }

        /// <summary>
        /// 群体攻击规则 - 召唤同伴
        /// </summary>
        /// <param name="ai">AI组件</param>
        /// <returns>决策结果</returns>
        public static AIBehaviorResult GroupAttack(AIComponent ai)
        {
            // 实现群体攻击逻辑
            // 这里可以添加召唤同伴的代码
            return AIBehaviorResult.Failure;
        }

        /// <summary>
        /// 积极移动规则 - 更积极地移动到目标
        /// </summary>
        /// <param name="ai">AI组件</param>
        /// <returns>决策结果</returns>
        public static AIBehaviorResult AggressiveMoveToTarget(AIComponent ai)
        {
            if (ai.CurrentTarget == null || !ai.CurrentTarget.IsAlive)
                return AIBehaviorResult.Failure;

            // 计算到目标的距离
            float distance = Vector3.Distance(ai.Owner.LocalPosition, ai.CurrentTarget.LocalPosition);

            // 如果不在攻击范围内，更积极地移动
            if (distance > ai.AttackRange)
            {
                // 使用更直线的路径
                Vector3 direction = (ai.CurrentTarget.LocalPosition - ai.Owner.LocalPosition).normalized;
                ai.Owner.Move(new Vector2(direction.x, direction.z));
                return AIBehaviorResult.Running;
            }

            return AIBehaviorResult.Failure;
        }

        /// <summary>
        /// 智能移动规则 - 智能移动到目标
        /// </summary>
        /// <param name="ai">AI组件</param>
        /// <returns>决策结果</returns>
        public static AIBehaviorResult SmartMoveToTarget(AIComponent ai)
        {
            if (ai.CurrentTarget == null || !ai.CurrentTarget.IsAlive)
                return AIBehaviorResult.Failure;

            // 计算到目标的距离
            float distance = Vector3.Distance(ai.Owner.LocalPosition, ai.CurrentTarget.LocalPosition);

            // 如果不在攻击范围内，智能移动
            if (distance > ai.AttackRange)
            {
                // 预测目标移动方向，进行拦截
                // 这里可以添加更复杂的路径规划逻辑
                Vector3 direction = (ai.CurrentTarget.LocalPosition - ai.Owner.LocalPosition).normalized;
                ai.Owner.Move(new Vector2(direction.x, direction.z));
                return AIBehaviorResult.Running;
            }

            return AIBehaviorResult.Failure;
        }

        /// <summary>
        /// 闪烁移动规则 - 高速怪物的特殊移动
        /// </summary>
        /// <param name="ai">AI组件</param>
        /// <returns>决策结果</returns>
        public static AIBehaviorResult FlickerMove(AIComponent ai)
        {
            // 实现闪烁移动效果
            // 这里可以添加随机传送或短距离瞬移的逻辑
            return AIBehaviorResult.Failure;
        }

        /// <summary>
        /// 重击规则 - 高攻击力怪物的特殊攻击
        /// </summary>
        /// <param name="ai">AI组件</param>
        /// <returns>决策结果</returns>
        public static AIBehaviorResult PowerAttack(AIComponent ai)
        {
            // 实现重击效果
            // 这里可以添加触发特殊攻击的逻辑
            return AIBehaviorResult.Failure;
        }
    }
}
