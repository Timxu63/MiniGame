using System;
using System.Collections.Generic;
using Game.Logic.BattleModule.Entity;

namespace HotFixBattle.AI
{
    /// <summary>
    /// AI配置加载器 - 状态工厂部分
    /// </summary>
    public static partial class AIConfigLoader
    {
        /// <summary>
        /// AI状态工厂字典，根据名称创建对应的状态
        /// </summary>
        private static readonly Dictionary<string, Func<AIComponent, IAIState>> _stateFactories =
            new Dictionary<string, Func<AIComponent, IAIState>>
            {
                // 基础状态
                { "Patrol", CreatePatrolState },
                { "Chase", CreateChaseState },
                { "Attack", CreateAttackState },
                { "Flee", CreateFleeState },
            };

        #region 状态工厂方法

        /// <summary>
        /// 创建巡逻状态
        /// </summary>
        /// <param name="ai">AI组件</param>
        /// <returns>巡逻状态实例</returns>
        private static IAIState CreatePatrolState(AIComponent ai)
        {
            return new AIPatrolState(ai);
        }

        /// <summary>
        /// 创建追逐状态
        /// </summary>
        /// <param name="ai">AI组件</param>
        /// <returns>追逐状态实例</returns>
        private static IAIState CreateChaseState(AIComponent ai)
        {
            return new AIChaseState(ai);
        }

        /// <summary>
        /// 创建攻击状态
        /// </summary>
        /// <param name="ai">AI组件</param>
        /// <returns>攻击状态实例</returns>
        private static IAIState CreateAttackState(AIComponent ai)
        {
            return new AIAttackState(ai);
        }

        /// <summary>
        /// 创建逃跑状态
        /// </summary>
        /// <param name="ai">AI组件</param>
        /// <returns>逃跑状态实例</returns>
        private static IAIState CreateFleeState(AIComponent ai)
        {
            return new AIFleeState(ai);
        }

        #endregion

        /// <summary>
        /// 尝试获取AI状态工厂
        /// </summary>
        /// <param name="name">状态名称</param>
        /// <param name="factory">状态工厂方法</param>
        /// <returns>是否找到工厂</returns>
        public static bool TryGetStateFactory(string name, out Func<AIComponent, IAIState> factory)
        {
            return _stateFactories.TryGetValue(name, out factory);
        }

        /// <summary>
        /// 注册新的AI状态工厂
        /// </summary>
        /// <param name="name">状态名称</param>
        /// <param name="factory">状态工厂方法</param>
        public static void RegisterStateFactory(string name, Func<AIComponent, IAIState> factory)
        {
            _stateFactories[name] = factory;
        }
    }
}