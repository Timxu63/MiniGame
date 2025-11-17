using System;
using cfg;
using Game.Logic.BattleModule.Entity;

namespace Game.Logic.BattleModule.Component.AI
{
    /// <summary>
    /// 通用追逐状态
    /// </summary>
    public class AIChaseState : AIStateBase
    {
        private IAIBehavior _chaseBehavior;

        public AIChaseState() : base()
        {

        }
        public override void Initialize(AIComponent ai)
        {
            base.Initialize(ai);
            // 创建追逐行为
            _chaseBehavior = new AIMoveToTarget();
        }

        public override void Enter()
        {
            // 进入追逐状态时的处理
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

            // 如果目标不在感知范围内，切换到巡逻状态
            if (!AI.IsTargetInPerceptionRange())
            {
                AI.ClearTarget();
                AI.StateMachine.ChangeState<AIPatrolState>();
                return;
            }

            // 执行追逐行为
            AIBehaviorResult chaseResult = _chaseBehavior.Execute(AI, deltaTime);

            // 如果到达攻击范围，切换到攻击状态
            if (chaseResult == AIBehaviorResult.Success)
            {
                AI.StateMachine.ChangeState<AIAttackState>();
            }
        }

        public override void Exit()
        {
            // 退出追逐状态时的处理
        }
    }
}