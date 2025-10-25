using System;
using Framework.EventSystem;
using Framework.ViewModule;

namespace HotFix
{
    public class UILoadingViewModule : BaseViewModule
    {
        private Action m_showCallback;
        private Action m_finishCallback;
        public override void RegisterEvents(EventSystemManager manager)
        {
            
        }

        public override void UnRegisterEvents(EventSystemManager manager)
        {
            
        }
        
        public void PlayShow(Action callback)
        {
            m_showCallback = callback;
            m_showCallback?.Invoke();
        }
        public void PlayHide(Action finished)
        {
            m_finishCallback = finished;
            m_finishCallback?.Invoke();
        }
        public override void OnCreate(object data)
        {
            
        }

        public override void OnDelete()
        {
            
        }

        public override void OnOpen(object data)
        {
            
        }

        public override void OnClose()
        {
            
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            
        }

        
    }
}