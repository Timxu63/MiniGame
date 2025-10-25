//-----------------------------------------------------------------
//
//              Maggic @  2023-09-19 14:49 
//
//----------------------------------------------------------------

using Framework.EventSystem;
using Framework.Runtime;
using UnityEngine;

namespace Framework.ViewModule
{
    /// <summary>
    ///  显示层基类
    /// </summary>
    public abstract class BaseViewModule : MonoBehaviour
    {
        public int m_viewName { get; private set; }

        private BaseViewModuleLoader m_loader;

        public BaseViewModuleLoader Loader
        {
            get { return m_loader; }
        }
        public void SetViewData(int viewName)
        {
            m_viewName = viewName;
        }
        internal void SetLoader(BaseViewModuleLoader loader)
        {
            m_loader = loader;
        }

        public virtual void OnLateUpdate(float deltaTime, float unscaledDeltaTime)
        {
        }

        public abstract void RegisterEvents(EventSystemManager manager);
        public abstract void UnRegisterEvents(EventSystemManager manager);

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="data"></param>
        public abstract void OnCreate(object data);

        /// <summary>
        /// 删除
        /// </summary>
        public abstract void OnDelete();

        /// <summary>
        /// 打开
        /// </summary>
        public abstract void OnOpen(object data);

        /// <summary>
        /// 关闭
        /// </summary>
        public abstract void OnClose();

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="unscaledDeltaTime"></param>
        public abstract void OnUpdate(float deltaTime, float unscaledDeltaTime);

        /// <summary>
        /// 安卓返回键支持，默认不关闭
        /// </summary>
        public virtual bool IsAddBack()
        {
            return false;
        }

        public void SetAddBack(bool value)
        {
            m_isAddBack = value;
        }

        public bool m_isAddBack { get; private set; } = false;


        protected void CloseSelf()
        {
            if (GameApp.View.IsOpenedOrLoading(m_viewName))
            {
                GameApp.View.CloseView(m_viewName);
            }
        }
#if UNITY_EDITOR

        public void Start()
        {
        }

        public void Awake()
        {
        }

        public void OnEnable()
        {
        }

        public void OnDisable()
        {
        }

        public void Update()
        {
        }

        public void LateUpdate()
        {
        }

        public void FixedUpdate()
        {
        }

        public void OnApplicationPause(bool pauseStatus)
        {
        }

        public void OnApplicationFocus(bool focusStatus)
        {
        }

        public void OnApplicationQuit()
        {
        }

        public void OnDrawGizmos()
        {
        }

        public void OnDrawGizmosSelected()
        {
        }

        public void Reset()
        {
        }

        public void OnValidate()
        {
        }

        public void OnDestroy()
        {
        }
#endif
    }
}