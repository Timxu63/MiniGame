using System;
using cfg;
using Game.Logic.BattleModule.Entity;

namespace HotFixBattle.AI
{
    /// <summary>
    /// 通用攻击状态
    /// </summary>
    public class AIAttackState : AIStateBase
    {
        private AIAttackTarget _attackBehavior;
        public AIAttackState(AIComponent ai) : base(ai)
        {
            Initialize(ai);
        }
        public override void Initialize(AIComponent ai)
        {
            base.Initialize(ai);
            // 创建攻击行为
            _attackBehavior = new AIAttackTarget(1.0f); // 1秒攻击冷却
        }
        public override void Enter()
        {
            // 进入攻击状态时的处理
        }

        public override void Execute(float deltaTime)
        {
            // 如果目标不存在或死亡，切换到巡逻状态
            if (AI.CurrentTarget == null || !AI.CurrentTarget.IsAlive)
            {
                AI.ClearTarget();
                AI.StateMachine.ChangeState<AIPatrolState>();
                return;
            }

            // 如果目标不在攻击范围内，切换到追逐状态
            if (!AI.IsTargetInAttackRange())
            {
                AI.StateMachine.ChangeState<AIChaseState>();
                return;
            }

            // 执行攻击行为
            _attackBehavior.Execute(AI, deltaTime);
        }

        public override void Exit()
        {
            // 退出攻击状态时的处理
        }
    }
}