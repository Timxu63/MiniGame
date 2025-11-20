using System;
using cfg;
using Game.Logic.BattleModule.Entity;

namespace HotFixBattle.AI
{
    /// <summary>
    /// 通用巡逻状态
    /// </summary>
    public class AIPatrolState : AIStateBase
    {
        private IAIBehavior _patrolBehavior;
        private AIFindTarget _findTargetBehavior;
        private Func<BaseEntity, bool> _targetFilter;

        public AIPatrolState(AIComponent ai) : base(ai)
        {
            Initialize(ai);
        }
        public override void Initialize(AIComponent ai)
        {
            base.Initialize(ai);
            // 创建巡逻行为
            var patrolSequence = new AISequence();
            patrolSequence.AddChild(new AIRandomMove(2.0f));
            patrolSequence.AddChild(new AIWait(1.0f));
            _patrolBehavior = patrolSequence;
            // 创建寻找目标行为
            _findTargetBehavior = new AIFindTarget(_targetFilter);
        }

        public void SetTargetFilter(Func<BaseEntity, bool> targetFilter)
        {
            if(targetFilter == null)
                return;
            _targetFilter = targetFilter;
            _findTargetBehavior.SetTargetFilter(targetFilter);
        }

        public override void Enter()
        {
            // 进入巡逻状态时的处理
        }

        public override void Execute(float deltaTime)
        {
            // 执行寻找目标行为
            AIBehaviorResult findResult = _findTargetBehavior.Execute(AI, deltaTime);

            // 如果找到目标，切换到追逐状态
            if (findResult == AIBehaviorResult.Success)
            {
                AI.StateMachine.ChangeState<AIChaseState>();
                return;
            }

            // 执行巡逻行为
            _patrolBehavior.Execute(AI, deltaTime);
        }

        public override void Exit()
        {
            // 退出巡逻状态时的处理
        }
    }
}