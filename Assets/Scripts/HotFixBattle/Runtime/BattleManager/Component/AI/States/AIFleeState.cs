using System;
using cfg;
using Game.Logic.BattleModule.Entity;

namespace Game.Logic.BattleModule.Component.AI
{
    /// <summary>
    /// 通用逃跑状态
    /// </summary>
    public class AIFleeState : AIStateBase
    {
        private readonly IAIBehavior _fleeBehavior;
        private float _fleeTimer = 0f;
        private readonly float _fleeDuration = 3.0f;

        public AIFleeState(AIComponent ai) : base(ai)
        {
            // 创建逃跑行为
            _fleeBehavior = new AIRandomMove(0.5f);
        }

        public AIFleeState() : base()
        {

        }

        public override void Enter()
        {
            // 重置逃跑计时器
            _fleeTimer = 0f;
        }

        public override void Execute(float deltaTime)
        {
            // 更新逃跑计时器
            _fleeTimer += deltaTime;

            // 如果逃跑时间超过持续时间，切换到巡逻状态
            if (_fleeTimer >= _fleeDuration)
            {
                AI.ClearTarget();
                AI.StateMachine.ChangeState<AIPatrolState>();
                return;
            }

            // 执行逃跑行为
            _fleeBehavior.Execute(AI, deltaTime);
        }

        public override void Exit()
        {
            // 退出逃跑状态时的处理
        }
    }
}