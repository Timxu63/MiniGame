using Framework.EventSystem;
using Framework.Logic.Modules;
using Framework.SceneModule;
using Framework.State;
using Framework.ViewModule;
using UnityEngine;

namespace Framework.Runtime
{
    public class GameApp: MonoBehaviour
    {
        [SerializeField] public ViewModuleManager m_view = null;
        [SerializeField] public EventSystemManager m_event = null;
        [SerializeField] public ResourcesManager m_resources = null;
        [SerializeField] public StateManager m_state = null;
        [SerializeField] public SceneManager m_scene = null;
        [SerializeField] public RuntimeManager m_runtime = null;
        
        /// <summary>
        /// 事件管理器
        /// </summary>
        public static EventSystemManager Event { get; private set; }

        /// <summary>
        /// 显示管理器
        /// </summary>
        public static ViewModuleManager View { get; private set; }

        /// <summary>
        /// 资源管理器
        /// </summary>
        public static ResourcesManager Resources { get; private set; }

        /// <summary>
        /// 状态管理器
        /// </summary>
        public static StateManager State { get; private set; }
        
        /// <summary>
        /// 场景管理器
        /// </summary>
        public static SceneManager Scene { get; private set; }
        /// <summary>
        /// 热更新运行管理器
        /// </summary>
        public static RuntimeManager RunTime { get; private set; }
        public void OnStarUp()
        {
            Event = m_event;
            View = m_view;
            Resources = m_resources;
            State = m_state;
            Scene = m_scene;
            RunTime = m_runtime;
            State.RegisterState(new CheckAssetsState());
            State.ActiveState((int)StateName.CheckAssetsState);
        }

        public void OnUpdate()
        {
            
        }

        public void OnLateUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            
        }

        public void OnAppShutdown()
        {
            
        }

        public void OnAppFocus(bool hasFocus)
        {
            
        }

        public void OnAppPause(bool pauseStatus)
        {
            
        }

        public void OnRestart()
        {
            
        }
    }
}