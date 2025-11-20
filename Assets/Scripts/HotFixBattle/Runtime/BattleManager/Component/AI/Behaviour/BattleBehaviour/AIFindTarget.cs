using System;
using cfg;
using Game.Logic.BattleModule.Component.Weapon;
using Game.Logic.BattleModule.Entity;

namespace HotFixBattle.AI
{
    /// <summary>
    /// 寻找目标行为
    /// </summary>
    public class AIFindTarget : AIBehaviorBase
    {
        private Func<BaseEntity, bool> _targetFilter;

        public override string Name => "FindTarget";

        public AIFindTarget(Func<BaseEntity, bool> targetFilter = null)
        {
            _targetFilter = targetFilter ?? (entity => entity.IsAlive && entity.Type != eEntityType.Monster);
        }
        public void SetTargetFilter(Func<BaseEntity, bool> targetFilter = null)
        {
            _targetFilter = targetFilter ?? (entity => entity.IsAlive && entity.Type != eEntityType.Monster);
        }
        public override AIBehaviorResult Execute(AIComponent ai, float deltaTime)
        {
            // 如果已有目标且目标存活，则返回成功
            if (ai.CurrentTarget != null && ai.CurrentTarget.IsAlive)
            {
                return AIBehaviorResult.Success;
            }

            // 寻找新目标
            // 注意：这里需要根据实际实体管理系统进行调整
            // 假设有一个全局的实体管理器
            var entityManager = SimpleEntityManager.Instance;
            if (entityManager != null)
            {
                foreach (var entity in entityManager.GetAllEntities())
                {
                    if (_targetFilter(entity))
                    {
                        float distance = UnityEngine.Vector3.Distance(ai.Owner.LocalPosition, entity.LocalPosition);
                        if (distance <= ai.PerceptionRange)
                        {
                            ai.SetTarget(entity);
                            return AIBehaviorResult.Success;
                        }
                    }
                }
            }

            return AIBehaviorResult.Failure;
        }
    }
}