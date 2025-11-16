
using System;

namespace Game.Logic.BattleModule.Component.AI
{
    /// <summary>
    /// AI状态基类，所有AI状态都应继承自此类
    /// </summary>
    public abstract class AIStateBase : IAIState
    {
        /// <summary>
        /// AI组件引用
        /// </summary>
        protected AIComponent AI { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ai">AI组件</param>
        public AIStateBase(AIComponent ai)
        {
            AI = ai;
        }

        /// <summary>
        /// 默认构造函数，用于通过反射创建状态实例
        /// </summary>
        public AIStateBase()
        {
        }

        /// <summary>
        /// 初始化状态，设置AI组件引用
        /// </summary>
        /// <param name="ai">AI组件</param>
        public virtual void Initialize(AIComponent ai)
        {
            AI = ai;
        }

        /// <summary>
        /// 进入状态时调用
        /// </summary>
        public abstract void Enter();

        /// <summary>
        /// 执行状态逻辑
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public abstract void Execute(float deltaTime);

        /// <summary>
        /// 退出状态时调用
        /// </summary>
        public abstract void Exit();
    }

    /// <summary>
    /// AI状态接口
    /// </summary>
    public interface IAIState
    {
        /// <summary>
        /// 进入状态时调用
        /// </summary>
        void Enter();

        /// <summary>
        /// 执行状态逻辑
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        void Execute(float deltaTime);

        /// <summary>
        /// 退出状态时调用
        /// </summary>
        void Exit();

        /// <summary>
        /// 初始化状态，设置AI组件引用
        /// </summary>
        /// <param name="ai">AI组件</param>
        void Initialize(AIComponent ai);
    }
}
