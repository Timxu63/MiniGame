
using System;
using System.Collections.Generic;
using Game.Logic.BattleModule.Entity;

namespace Game.Logic.BattleModule.Component.AI
{
    /// <summary>
    /// AI组件基类，所有AI组件都应继承自此类
    /// </summary>
    public abstract class AIComponent : EntityComponent
    {
        /// <summary>
        /// AI状态机
        /// </summary>
        public AIStateMachine StateMachine { get; private set; }

        /// <summary>
        /// AI决策器
        /// </summary>
        public AIDecisionMaker DecisionMaker { get; private set; }

        /// <summary>
        /// 当前目标实体
        /// </summary>
        public BaseEntity CurrentTarget { get; protected set; }

        /// <summary>
        /// AI感知范围
        /// </summary>
        public float PerceptionRange { get; set; } = 100.0f;

        /// <summary>
        /// AI攻击范围
        /// </summary>
        public float AttackRange { get; set; } = 2.0f;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="owner">AI所属的实体</param>
        protected AIComponent(BaseEntity owner) : base(owner)
        {
            StateMachine = new AIStateMachine();
            DecisionMaker = new AIDecisionMaker();
        }

        /// <summary>
        /// 初始化AI组件
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            InitializeStates();
            InitializeBehaviors();
        }

        /// <summary>
        /// 更新AI组件
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public override void Update(float deltaTime)
        {
            if (!IsEnabled) return;

            // 感知周围环境
            Perception();

            // 做出决策
            MakeDecision();

            // 更新状态机
            StateMachine.Update(deltaTime);

            // 执行当前状态的行为
            StateMachine.CurrentState?.Execute(deltaTime);
        }

        /// <summary>
        /// 初始化AI状态
        /// </summary>
        protected abstract void InitializeStates();

        /// <summary>
        /// 初始化AI行为
        /// </summary>
        protected abstract void InitializeBehaviors();

        /// <summary>
        /// 感知周围环境
        /// </summary>
        protected virtual void Perception()
        {
            // 子类可重写此方法实现特定的感知逻辑
        }

        /// <summary>
        /// 做出决策
        /// </summary>
        protected virtual void MakeDecision()
        {
            // 子类可重写此方法实现特定的决策逻辑
            // 使用决策器做出决策
            AIDecision decision = DecisionMaker.MakeDecision(this);

            // 根据决策结果切换状态
            switch (decision)
            {
                case AIDecision.Flee:
                    if (!StateMachine.IsInState<AIFleeState>())
                    {
                        StateMachine.ChangeState<AIFleeState>();
                    }
                    break;
                case AIDecision.Attack:
                    if (!StateMachine.IsInState<AIAttackState>())
                    {
                        StateMachine.ChangeState<AIAttackState>();
                    }
                    break;
                case AIDecision.Move:
                    if (!StateMachine.IsInState<AIChaseState>())
                    {
                        StateMachine.ChangeState<AIChaseState>();
                    }
                    break;
                case AIDecision.Patrol:
                    if (!StateMachine.IsInState<AIPatrolState>())
                    {
                        StateMachine.ChangeState<AIPatrolState>();
                    }
                    break;
                case AIDecision.Idle:
                    // 可以添加待机状态，这里暂时使用巡逻状态
                    if (!StateMachine.IsInState<AIPatrolState>())
                    {
                        StateMachine.ChangeState<AIPatrolState>();
                    }
                    break;
            }
        }

        /// <summary>
        /// 设置目标
        /// </summary>
        /// <param name="target">目标实体</param>
        public virtual void SetTarget(BaseEntity target)
        {
            CurrentTarget = target;
        }

        /// <summary>
        /// 清除目标
        /// </summary>
        public virtual void ClearTarget()
        {
            CurrentTarget = null;
        }

        /// <summary>
        /// 获取与目标的距离
        /// </summary>
        /// <returns>与目标的距离，如果没有目标则返回float.MaxValue</returns>
        public virtual float GetDistanceToTarget()
        {
            if (CurrentTarget == null) return float.MaxValue;
            return UnityEngine.Vector3.Distance(Owner.LocalPosition, CurrentTarget.LocalPosition);
        }

        /// <summary>
        /// 检查目标是否在攻击范围内
        /// </summary>
        /// <returns>如果目标在攻击范围内返回true，否则返回false</returns>
        public virtual bool IsTargetInAttackRange()
        {
            return GetDistanceToTarget() <= AttackRange;
        }

        /// <summary>
        /// 检查目标是否在感知范围内
        /// </summary>
        /// <returns>如果目标在感知范围内返回true，否则返回false</returns>
        public virtual bool IsTargetInPerceptionRange()
        {
            return GetDistanceToTarget() <= PerceptionRange;
        }
    }
}
