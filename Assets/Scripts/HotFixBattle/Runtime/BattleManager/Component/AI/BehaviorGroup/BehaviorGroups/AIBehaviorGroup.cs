using System;
using System.Collections.Generic;
using cfg;
using Game.Logic.BattleModule.Entity;

namespace HotFixBattle.AI
{
    /// <summary>
    /// AI行为组合，将状态、行为和决策规则组合成一个完整的AI行为
    /// </summary>
    public class AIBehaviorGroup
    {
        /// <summary>
        /// 行为组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 状态列表
        /// </summary>
        public List<string> States { get; set; } = new List<string>();

        /// <summary>
        /// 决策规则列表
        /// </summary>
        public List<string> DecisionRules { get; set; } = new List<string>();

        /// <summary>
        /// 初始化AI组件
        /// </summary>
        /// <param name="ai">AI组件</param>
        /// <param name="charactor">角色配置</param>
        public void Initialize(AIComponent ai, Charactor charactor)
        {
            // 初始化状态
            InitializeStates(ai);

            // 初始化行为和决策规则
            this.InitializeBehaviors(ai, charactor);
        }

        /// <summary>
        /// 初始化AI状态
        /// </summary>
        /// <param name="ai">AI组件</param>
        private void InitializeStates(AIComponent ai)
        {
            IAIState initialState = null;

            foreach (string stateConfig in States)
            {
                string[] parts = stateConfig.Split(':');
                string stateName = parts[0].Trim();

                if (AIConfigLoader.TryGetStateFactory(stateName, out var factory))
                {
                    IAIState state = factory(ai);

                    // 如果是巡逻状态，设置目标过滤器
                    if (state is AIPatrolState patrolState)
                    {
                        Func<BaseEntity, bool> monsterTargetFilter = entity => entity.IsAlive && entity.Type == eEntityType.Player;
                        patrolState.SetTargetFilter(monsterTargetFilter);
                    }

                    // 使用状态类型添加状态
                    ai.StateMachine.AddState(state.GetType(), state);

                    // 记录第一个状态作为初始状态
                    if (initialState == null)
                    {
                        initialState = state;
                    }
                }
            }

            // 设置初始状态为第一个状态
            if (initialState != null)
            {
                ai.StateMachine.ChangeState(initialState);
            }
        }

        /// <summary>
        /// 初始化AI行为和决策规则
        /// </summary>
        /// <param name="ai">AI组件</param>
        /// <param name="charactor">角色配置</param>
        private void InitializeBehaviors(AIComponent ai, Charactor charactor)
        {
            // 初始化决策规则
            foreach (string ruleConfig in DecisionRules)
            {
                string[] parts = ruleConfig.Split(':');
                string ruleName = parts[0].Trim();
                string[] parameters = parts.Length > 1 ? parts[1].Split(',') : new string[0];
            
                // 然后尝试从基础决策规则工厂中获取
                if (AIConfigLoader.TryGetDecisionRuleFactory(ruleName, out var factory))
                {
                    AIDecisionRule rule = factory(parameters);
                    ai.DecisionMaker.AddRule(rule);
                }
                // 如果都没找到，记录警告
                else
                {
                    UnityEngine.Debug.LogWarning($"未找到AI决策规则: {ruleName}");
                }
            }
        }
    }
}
