using Framework.EventSystem;
using Framework.Runtime;
using Framework.State;
using Framework.ViewModule;

namespace HotFix
{
    public class LoginState : State
    {
        public override int GetName()
        {
            return (int) StateName.LoginState;
        }

        public override void OnEnter()
        {
            GameApp.View.OpenView((int)ViewName.UILoading, null, UILayers.Second, null, (obj) =>
            {
                var loadingViewModule = GameApp.View.GetViewModule<UILoadingViewModule>((int)ViewName.UILoading);
                loadingViewModule.PlayShow(() =>
                {
                    GameApp.State.ActiveState((int)StateName.MainState);
                });
            });
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            
        }

        public override void OnShutDown()
        {
            
        }

        public override void OnAppFocus(bool hasFocus)
        {
            
        }

        public override void OnAppPause(bool pauseStatus)
        {
            
        }

        public override void OnExit()
        {
            
        }

        public override void RegisterEvents(EventSystemManager manager)
        {
            
        }

        public override void UnRegisterEvents(EventSystemManager manager)
        {
            
        }
    }
}