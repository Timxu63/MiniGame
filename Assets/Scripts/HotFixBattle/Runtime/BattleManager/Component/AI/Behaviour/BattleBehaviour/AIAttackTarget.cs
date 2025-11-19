using System;
using cfg;
using Game.Logic.BattleModule.Component.Weapon;
using Game.Logic.BattleModule.Entity;

namespace Game.Logic.BattleModule.Component.AI
{
    /// <summary>
    /// 攻击目标行为
    /// </summary>
    public class AIAttackTarget : AIBehaviorBase
    {
        private float _lastAttackTime = 0f;
        private readonly float _attackCooldown;

        public override string Name => "AttackTarget";

        public AIAttackTarget(float attackCooldown = 1.0f)
        {
            _attackCooldown = attackCooldown;
        }

        public override AIBehaviorResult Execute(AIComponent ai, float deltaTime)
        {
            if (ai.CurrentTarget == null)
            {
                return AIBehaviorResult.Failure;
            }

            // 检查攻击冷却
            if (UnityEngine.Time.time - _lastAttackTime < _attackCooldown)
            {
                return AIBehaviorResult.Running;
            }

            // 检查是否在攻击范围内
            if (!ai.IsTargetInAttackRange())
            {
                return AIBehaviorResult.Failure;
            }

            // 尝试攻击
            // 注意：这里需要根据实际武器系统进行调整
            // 假设实体有武器组件
            var weaponComponent = ai.Owner.ComponentManager.GetComponent<WeaponComponent>();
            if (weaponComponent != null && weaponComponent.CurrentWeapon != null)
            {
                bool success = weaponComponent.CurrentWeapon.TryAttack(ai.CurrentTarget);
                if (success)
                {
                    _lastAttackTime = UnityEngine.Time.time;
                    return AIBehaviorResult.Success;
                }
            }

            return AIBehaviorResult.Failure;
        }
    }
}