
using System;
using System.Collections.Generic;
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

    /// <summary>
    /// AI行为组合器，用于组合多个行为
    /// </summary>
    public abstract class AIBehaviorComposite : AIBehaviorBase
    {
        /// <summary>
        /// 子行为列表
        /// </summary>
        protected List<IAIBehavior> Children { get; } = new List<IAIBehavior>();

        /// <summary>
        /// 添加子行为
        /// </summary>
        /// <param name="behavior">子行为</param>
        public void AddChild(IAIBehavior behavior)
        {
            if (behavior != null)
            {
                Children.Add(behavior);
            }
        }

        /// <summary>
        /// 清除所有子行为
        /// </summary>
        public void ClearChildren()
        {
            Children.Clear();
        }
    }

    /// <summary>
    /// AI行为序列，依次执行子行为，直到有一个失败或全部成功
    /// </summary>
    public class AISequence : AIBehaviorComposite
    {
        private int _currentChild = 0;

        public override string Name => "Sequence";

        public override AIBehaviorResult Execute(AIComponent ai, float deltaTime)
        {
            while (_currentChild < Children.Count)
            {
                AIBehaviorResult result = Children[_currentChild].Execute(ai, deltaTime);

                if (result == AIBehaviorResult.Failure)
                {
                    _currentChild = 0;
                    return AIBehaviorResult.Failure;
                }
                else if (result == AIBehaviorResult.Running)
                {
                    return AIBehaviorResult.Running;
                }

                _currentChild++;
            }

            _currentChild = 0;
            return AIBehaviorResult.Success;
        }
    }

    /// <summary>
    /// AI行为选择器，依次执行子行为，直到有一个成功或全部失败
    /// </summary>
    public class AISelector : AIBehaviorComposite
    {
        private int _currentChild = 0;

        public override string Name => "Selector";

        public override AIBehaviorResult Execute(AIComponent ai, float deltaTime)
        {
            while (_currentChild < Children.Count)
            {
                AIBehaviorResult result = Children[_currentChild].Execute(ai, deltaTime);

                if (result == AIBehaviorResult.Success)
                {
                    _currentChild = 0;
                    return AIBehaviorResult.Success;
                }
                else if (result == AIBehaviorResult.Running)
                {
                    return AIBehaviorResult.Running;
                }

                _currentChild++;
            }

            _currentChild = 0;
            return AIBehaviorResult.Failure;
        }
    }

    /// <summary>
    /// AI并行行为，同时执行所有子行为
    /// </summary>
    public class AIParallel : AIBehaviorComposite
    {
        public enum Policy
        {
            RequireOne,    // 只需要一个成功
            RequireAll     // 需要全部成功
        }

        private readonly Policy _policy;

        public override string Name => "Parallel";

        public AIParallel(Policy policy = Policy.RequireOne)
        {
            _policy = policy;
        }

        public override AIBehaviorResult Execute(AIComponent ai, float deltaTime)
        {
            int successCount = 0;
            int failureCount = 0;
            int runningCount = 0;

            foreach (var child in Children)
            {
                AIBehaviorResult result = child.Execute(ai, deltaTime);

                switch (result)
                {
                    case AIBehaviorResult.Success:
                        successCount++;
                        break;
                    case AIBehaviorResult.Failure:
                        failureCount++;
                        break;
                    case AIBehaviorResult.Running:
                        runningCount++;
                        break;
                }
            }

            if (_policy == Policy.RequireOne && successCount > 0)
            {
                return AIBehaviorResult.Success;
            }
            else if (_policy == Policy.RequireAll && failureCount == 0 && runningCount == 0)
            {
                return AIBehaviorResult.Success;
            }
            else if (runningCount > 0)
            {
                return AIBehaviorResult.Running;
            }

            return AIBehaviorResult.Failure;
        }
    }
}
