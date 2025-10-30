using Framework.DataModule;
using Framework.EventSystem;
using Framework.Logic.Modules;
using Framework.PersistentData;
using Framework.SceneModule;
using Framework.State;
using Framework.ViewModule;
using UnityEngine;

namespace Framework.Runtime
{
    public class GameApp: MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] public bool isEditorScene = false;
#endif
        [SerializeField] public ViewModuleManager m_view = null;
        [SerializeField] public EventSystemManager m_event = null;
        [SerializeField] public ResourcesManager m_resources = null;
        [SerializeField] public StateManager m_state = null;
        [SerializeField] public SceneManager m_scene = null;
        [SerializeField] public RuntimeManager m_runtime = null;
        [SerializeField] public TableManager m_table = null;
        [SerializeField] public PersistentDataManager m_persistent = null;
        [SerializeField] public DataModuleManager m_dataModule = null;
        
#if UNITY_EDITOR
        public static bool IsEditorScene = false;
#endif
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
        
        /// <summary>
        /// 数据管理器
        /// </summary>
        public static TableManager Table { get; private set; }
        
        /// <summary>
        /// 持久化数据管理器
        /// </summary>
        public static PersistentDataManager PersistentData { get; private set; }
        
        /// <summary>
        /// 数据管理器
        /// </summary>
        public static DataModuleManager DataModule { get; private set; }

        
        public void OnStarUp()
        {
            Event = m_event;
            View = m_view;
            Resources = m_resources;
            State = m_state;
            Scene = m_scene;
            RunTime = m_runtime;
            Table = m_table;
            PersistentData = m_persistent;
            m_persistent.OnInit();
            DataModule = m_dataModule;
            IsEditorScene = isEditorScene;
            State.RegisterState(new CheckAssetsState());
            State.ActiveState((int)StateName.CheckAssetsState);
        }

        public void OnUpdate()
        {
            Event.OnUpdate(Time.deltaTime, Time.unscaledDeltaTime);

            View.OnUpdate(Time.deltaTime, Time.unscaledDeltaTime);

            State.OnUpdate(Time.deltaTime, Time.unscaledDeltaTime);

            PersistentData.OnUpdate(Time.unscaledDeltaTime);
        }

        public void OnLateUpdate()
        {
            View.OnLateUpdate(Time.deltaTime, Time.unscaledDeltaTime);
            State.OnLateUpdate(Time.deltaTime, Time.unscaledDeltaTime);
        }

        public void OnFixedUpdate()
        {
            RunTime.m_model.OnFixedUpdate();
        }

        public void OnAppShutdown()
        {
            m_state.OnShutDown();
            
            m_view.CloseAllView();
            m_event.UnRegisterAllEvent();
            m_state.UnRegisterAllState();
            m_table.SetITableManager(null);
            m_persistent.OnDeInit();
            RunTime.m_model.OnShutDown();
        }

        public void OnAppFocus(bool hasFocus)
        {
            m_state.OnAppFocus(hasFocus);
        }

        public void OnAppPause(bool pauseStatus)
        {
            m_state.OnAppPause(pauseStatus);
        }

        public void OnRestart()
        {
            
        }
    }
}