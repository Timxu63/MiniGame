using System;
using System.Collections.Generic;
using Framework.EventSystem;
using UnityEngine;
using System.Linq;

namespace Framework.State
{
    public class StateManager : MonoBehaviour
    {
        private State m_currentState = null;

        private Dictionary<int, State> m_states = new Dictionary<int, State>();

        [SerializeField] private EventSystemManager m_eventSystemManager = null;

        /// <summary>
        /// 激活状态
        /// </summary>
        /// <param name="stateName"></param>
        public void ActiveState(int stateName)
        {
            State _state = GetState<State>(stateName);
            Logger.Log($"<color=red>[State]</color>onEnter StateName = {_state.GetName()}" );
            if (m_currentState != null)
            {
                Logger.Log($"<color=red>[State]</color>onExit {m_currentState.GetName()} " );
                m_currentState.UnRegisterEvents(m_eventSystemManager);
                m_currentState.OnExit();
                
                //上报 退出状态
                // GameApp.SDK?.Analyze.Track("StateManager", new Dictionary<string, object>()
                // {
                //     {"int0", m_currentState.GetName()},
                //     {"int1", 2},
                // });
            }
            
            //上报 进入状态
            // GameApp.SDK?.Analyze.Track("StateManager", new Dictionary<string, object>()
            // {
            //     {"int0", _state.GetName()},
            //     {"int1", 1},
            // });
            
            _state.OnEnter();
            _state.RegisterEvents(m_eventSystemManager);
            m_currentState = _state;
        }

        /// <summary>
        /// 获得当前状态的名称
        /// </summary>
        /// <returns></returns>
        public int GetCurrentStateName()
        {
            if (m_currentState == null) return -1;
            return m_currentState.GetName();
        }

        /// <summary>
        /// 获得状态机
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public T GetState<T>(int stateName) where T : State
        {
            T _t = default(T);
            State _state = null;
            if (m_states.TryGetValue(stateName, out _state))
            {
                _t = _state as T;
            }

            return _t;
        }

        /// <summary>
        /// 注册状态
        /// </summary>
        /// <param name="state"></param>
        public void RegisterState(State state)
        {
            if (state != null)
            {
                m_states[state.GetName()] = state;
            }
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        /// <param name="state"></param>
        public void UnRegisterState(State state)
        {
            if (state != null)
            {
                m_states.Remove(state.GetName());
            }
        }

        /// <summary>
        /// 通过名字取消注册
        /// </summary>
        /// <param name="stateName"></param>
        public void UnRegisterStateByName(int stateName)
        {
            m_states.Remove(stateName);
        }

        /// <summary>
        /// 卸载所有的状态模块
        /// </summary>
        /// <param name="ignoreIDs">忽略的ID</param>
        public void UnRegisterAllState(params int[] ignoreIDs)
        {
            if (m_currentState != null) m_currentState.UnRegisterEvents(m_eventSystemManager);
            
            List<State> unRegisters = new List<State>();
            foreach (var item in m_states)
            {
                if (item.Value == null) continue;
                if (ignoreIDs != null && ignoreIDs.Contains(item.Key)) continue;
                unRegisters.Add(item.Value);
            }

            for (int i = 0; i < unRegisters.Count; i++)
            {
                var data = unRegisters[i];
                if (data == null) continue;
                UnRegisterState(data);
            }
        }


        /// <summary>
        /// 状态Update运行，只有当前状态才会运行
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="unscaledDeltaTime"></param>
        public void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            if (m_currentState != null) m_currentState.OnUpdate(deltaTime, unscaledDeltaTime);
        }

        /// <summary>
        /// 状态LateUpdate运行，只有当前状态才会运行
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="unscaledDeltaTime"></param>
        public void OnLateUpdate(float deltaTime, float unscaledDeltaTime)
        {
            if (m_currentState != null) m_currentState.OnLateUpdate(deltaTime, unscaledDeltaTime);
        }

        public void OnShutDown()
        {
            m_currentState?.OnShutDown();
        }

        public void OnAppFocus(bool hasFocus)
        {
            m_currentState?.OnAppFocus(hasFocus);
        }

        public void OnAppPause(bool pauseStatus)
        {
            m_currentState?.OnAppPause(pauseStatus);
        }
    }
}