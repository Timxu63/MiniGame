using System;
using cfg;
using Game.Logic.BattleModule.Entity;

namespace HotFixBattle.AI
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
            // 使用AIBehaviorGroupLoader从Charactor配置中加载AI状态
            AIBehaviorGroupLoader.InitializeAI(this, Owner.Charactor);

            // 添加通用状态

            

        }

        /// <summary>
        /// 初始化AI行为
        /// </summary>
        protected override void InitializeBehaviors()
        {
            // 使用AIBehaviorGroupLoader从Charactor配置中加载AI行为和决策规则
            AIBehaviorGroupLoader.InitializeAI(this, Owner.Charactor);

            // // 根据怪物属性添加特殊行为
            // if (Owner.Charactor.Speed > 6.0f)
            // {
            //     // 高速怪物特殊行为
            //     DecisionMaker.AddRule(SpecialDecisionRules.FlickerMove); // 闪烁移动
            // }
            //
            // if (Owner.Charactor.Attack > 15.0f)
            // {
            //     // 高攻击力怪物特殊行为
            //     DecisionMaker.AddRule(SpecialDecisionRules.PowerAttack); // 重击
            // }
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