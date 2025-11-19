using System;
using cfg;
using Game.Logic.BattleModule.Entity;

namespace Game.Logic.BattleModule.Component.AI
{
    /// <summary>
    /// 怪物AI组件，控制怪物的行为
    /// </summary>
    public class MonsterAI : AIComponent
    {
        // 使用通用状态类，无需在此定义私有状态

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="owner">AI所属的怪物实体</param>
        public MonsterAI(MonsterEntity owner) : base(owner)
        {
            // 根据怪物类型设置不同的参数
            switch (owner.MonsterType)
            {
                case eEntityType.Monster:
                    PerceptionRange = 100.0f;
                    AttackRange = 2.0f;
                    break;
                case eEntityType.Elite:
                    PerceptionRange = 100.0f;
                    AttackRange = 2.5f;
                    break;
                case eEntityType.Boss:
                    PerceptionRange = 100.0f;
                    AttackRange = 3.0f;
                    break;
                default:
                    PerceptionRange = 100.0f;
                    AttackRange = 2.0f;
                    break;
            }
        }

        /// <summary>
        /// 初始化AI状态
        /// </summary>
        protected override void InitializeStates()
        {
            // 定义怪物寻找目标的过滤器
            Func<BaseEntity, bool> monsterTargetFilter = entity => entity.IsAlive && entity.Type == eEntityType.Player;

            // 添加通用状态
            AIPatrolState aiPatrolState = StateMachine.AddState<AIPatrolState>(this);
            aiPatrolState.SetTargetFilter(monsterTargetFilter);
            
            StateMachine.AddState<AIChaseState>(this);
            StateMachine.AddState<AIAttackState>(this);
            StateMachine.AddState<AIFleeState>(this);
            // 设置初始状态为巡逻状态
            StateMachine.ChangeState<AIPatrolState>();
        }

        /// <summary>
        /// 初始化AI行为
        /// </summary>
        protected override void InitializeBehaviors()
        {
            // 添加决策规则
            // DecisionMaker.AddRule(AIDecisionRules.LowHealthFlee(0.3f));  // 生命值低于30%时逃跑
            DecisionMaker.AddRule(AIDecisionRules.AttackTargetInRange());   // 有目标且在攻击范围内则攻击
            DecisionMaker.AddRule(AIDecisionRules.MoveToTarget());         // 有目标但不在攻击范围内则移动
            DecisionMaker.AddRule(AIDecisionRules.PatrolWhenNoTarget());   // 没有目标则巡逻
            DecisionMaker.AddRule(AIDecisionRules.DefaultIdle());           // 默认待机
        }

        /// <summary>
        /// 感知周围环境
        /// </summary>
        protected override void Perception()
        {
            // 基类中已经实现了基本的感知逻辑
            base.Perception();

            // 这里可以添加特定的感知逻辑
            // 例如：感知特定类型的实体、感知危险等
        }

        /// <summary>
        /// 做出决策
        /// </summary>
        protected override void MakeDecision()
        {
            base.MakeDecision();
        }
    }
}