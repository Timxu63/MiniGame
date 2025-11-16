
using Game.Logic.BattleModule.Entity;

namespace Game.Logic.BattleModule.Component.AI
{
    /// <summary>
    /// AI系统使用示例
    /// </summary>
    public static class AIExample
    {
        /// <summary>
        /// 为怪物实体添加AI的示例方法
        /// </summary>
        /// <param name="monster">怪物实体</param>
        public static void AddAIToMonster(MonsterEntity monster)
        {
            if (monster == null) return;

            // 使用工厂为怪物添加AI
            AIFactory.AddAIToEntity("Monster", monster);
        }

        /// <summary>
        /// 创建自定义AI的示例方法
        /// </summary>
        /// <param name="monster">怪物实体</param>
        public static void AddCustomAIToMonster(MonsterEntity monster)
        {
            if (monster == null) return;

            // 注册自定义AI创建器
            AIFactory.RegisterCreator("CustomMonster", owner => 
            {
                var ai = new MonsterAI(owner as MonsterEntity);

                // 可以在这里自定义AI的行为
                // 例如：修改感知范围、攻击范围等
                ai.PerceptionRange = 12.0f;
                ai.AttackRange = 3.0f;

                // 可以添加自定义决策规则
                ai.DecisionMaker.AddRule(new AIDecisionRule(
                    90, // 高优先级
                    aiComponent => aiComponent.Owner.CurrentHealth < aiComponent.Owner.MaxHealth * 0.5f, // 生命值低于50%
                    AIDecision.Flee // 逃跑
                ));

                return ai;
            });

            // 使用自定义AI
            AIFactory.AddAIToEntity("CustomMonster", monster);
        }
    }
}
