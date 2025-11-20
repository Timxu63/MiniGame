
using System;
using System.Collections.Generic;
using Game.Logic.BattleModule.Entity;
using UnityEngine;

namespace HotFixBattle.AI
{
    /// <summary>
    /// AI状态机，管理AI状态之间的转换
    /// </summary>
    public class AIStateMachine
    {
        /// <summary>
        /// 当前状态
        /// </summary>
        public IAIState CurrentState { get; private set; }

        /// <summary>
        /// 上一个状态
        /// </summary>
        public IAIState PreviousState { get; private set; }

        /// <summary>
        /// 全局状态，无论当前状态如何都会执行
        /// </summary>
        public IAIState GlobalState { get; set; }

        /// <summary>
        /// 状态字典，存储所有可用的状态
        /// </summary>
        private Dictionary<Type, IAIState> _states = new Dictionary<Type, IAIState>();

        /// <summary>
        /// 添加状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <returns>添加的状态实例</returns>
        public T AddState<T>() where T : IAIState, new()
        {
            Type stateType = typeof(T);
            
            // 如果已存在该类型的状态，则返回现有状态
            if (_states.ContainsKey(stateType))
            {
                return (T)_states[stateType];
            }

            // 创建新状态
            T state = new T();
            _states[stateType] = state;

            return state;
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <returns>状态实例，如果不存在则返回null</returns>
        public T GetState<T>() where T : IAIState
        {
            Type stateType = typeof(T);
            if (_states.ContainsKey(stateType))
            {
                return (T)_states[stateType];
            }

            return default(T);
        }

        /// <summary>
        /// 更改当前状态
        /// </summary>
        /// <typeparam name="T">新状态类型</typeparam>
        public void ChangeState<T>() where T : IAIState
        {
            Type stateType = typeof(T);
            if (!_states.ContainsKey(stateType))
            {
                return;
            }

            IAIState newState = _states[stateType];
            ChangeState(newState);
        }

        /// <summary>
        /// 更改当前状态
        /// </summary>
        /// <param name="newState">新状态</param>
        public void ChangeState(IAIState newState)
        {
            if (newState == null) return;
            // 退出当前状态
            CurrentState?.Exit();

            // 保存当前状态为上一个状态
            PreviousState = CurrentState;

            // 设置新状态为当前状态
            CurrentState = newState;

            // 进入新状态
            CurrentState.Enter();
        }

        /// <summary>
        /// 恢复到上一个状态
        /// </summary>
        public void RevertToPreviousState()
        {
            if (PreviousState != null)
            {
                ChangeState(PreviousState);
            }
        }

        /// <summary>
        /// 更新状态机
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public void Update(float deltaTime)
        {
            // 执行全局状态
            GlobalState?.Execute(deltaTime);
        }

        /// <summary>
        /// 检查当前是否处于指定状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <returns>如果当前处于指定状态返回true，否则返回false</returns>
        public bool IsInState<T>() where T : IAIState
        {
            return CurrentState?.GetType() == typeof(T);
        }

        /// <summary>
        /// 添加状态并初始化AI组件引用
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <param name="ai">AI组件</param>
        /// <returns>添加的状态实例</returns>
        public T AddState<T>(AIComponent ai) where T : IAIState, new()
        {
            T state = AddState<T>();

            // 初始化AI组件引用
            if (state != null)
            {
                state.Initialize(ai);
            }

            return state;
        }

        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="stateType">状态类型</param>
        /// <param name="state">状态实例</param>
        /// <returns>添加的状态实例</returns>
        public IAIState AddState(Type stateType, IAIState state)
        {
            // 如果已存在该类型的状态，则返回现有状态
            if (_states.ContainsKey(stateType))
            {
                return _states[stateType];
            }

            // 添加新状态
            _states[stateType] = state;

            return state;
        }
    }
}
