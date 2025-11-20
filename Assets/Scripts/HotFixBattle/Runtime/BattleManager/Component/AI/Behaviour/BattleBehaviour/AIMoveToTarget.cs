using System;
using cfg;
using Game.Logic.BattleModule.Component.Weapon;
using Game.Logic.BattleModule.Entity;

namespace HotFixBattle.AI
{
    /// <summary>
    /// 移动到目标行为
    /// </summary>
    public class AIMoveToTarget : AIBehaviorBase
    {
        public override string Name => "MoveToTarget";

        public override AIBehaviorResult Execute(AIComponent ai, float deltaTime)
        {
            if (ai.CurrentTarget == null)
            {
                return AIBehaviorResult.Failure;
            }

            // 计算方向
            UnityEngine.Vector3 direction = ai.CurrentTarget.LocalPosition - ai.Owner.LocalPosition;
            direction.y = 0; // 确保只在水平面上移动
            direction.Normalize();

            // 移动
            ai.Owner.Move(new UnityEngine.Vector2(direction.x, direction.z));

            // 检查是否到达攻击范围
            if (ai.IsTargetInAttackRange())
            {
                return AIBehaviorResult.Success;
            }

            return AIBehaviorResult.Running;
        }
    }
}