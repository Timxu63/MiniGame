
using System;
using Game.Logic.BattleModule.Entity;

namespace Game.Logic.BattleModule.Component.AI
{
    /// <summary>
    /// AI行为接口
    /// </summary>
    public interface IAIBehavior
    {
        /// <summary>
        /// 行为名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 执行行为
        /// </summary>
        /// <param name="ai">AI组件</param>
        /// <param name="deltaTime">时间增量</param>
        /// <returns>行为执行结果</returns>
        AIBehaviorResult Execute(AIComponent ai, float deltaTime);
    }

    /// <summary>
    /// AI行为执行结果
    /// </summary>
    public enum AIBehaviorResult
    {
        Success,    // 成功
        Failure,    // 失败
        Running     // 运行中
    }

    /// <summary>
    /// AI行为基类
    /// </summary>
    public abstract class AIBehaviorBase : IAIBehavior
    {
        /// <summary>
        /// 行为名称
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 执行行为
        /// </summary>
        /// <param name="ai">AI组件</param>
        /// <param name="deltaTime">时间增量</param>
        /// <returns>行为执行结果</returns>
        public abstract AIBehaviorResult Execute(AIComponent ai, float deltaTime);
    }
}
