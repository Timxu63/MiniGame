using Cysharp.Threading.Tasks;
using Framework.EventSystem;
using Framework.Logic.Modules;
using Framework.Runtime;
using Framework.State;
using Framework.SceneModule;
using Framework.ViewModule;
using UnityEngine.SceneManagement;

namespace HotFix
{
    public class MainState : State
    {
        public override int GetName()
        {
            return (int)StateName.MainState;
        }

        public override async void OnEnter()
        {
            await GameApp.Scene.LoadSceneAsync("Assets/_Resources/Scenes/Main.scene", LoadSceneMode.Single);
            await GameApp.View.OpenView((int)ViewName.UIMain, null, UILayers.First);
            if (GameApp.View.IsOpened(ViewName.UILoading))
            {
                var loadingViewModule =
                    GameApp.View.GetViewModule<UILoadingViewModule>(ViewName.UILoading);
                loadingViewModule.PlayHide(OnLoadingEnd);
            }
            else
            {
                GameApp.View.OpenView(ViewName.UILoading, null, UILayers.Second, null, (loadObj) =>
                {
                    var loadingViewModule =
                        GameApp.View.GetViewModule<UILoadingViewModule>(ViewName.UILoading);
                    loadingViewModule.PlayHide(OnLoadingEnd);
                });
            }
        }
        private void OnLoadingEnd()
        {
            GameApp.View.CloseView(ViewName.UILoading);
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
            GameApp.View.CloseAllView(new int[]
            {
                (int)ViewName.UILoading
            });
        }

        public override void RegisterEvents(EventSystemManager manager)
        {
            
        }

        public override void UnRegisterEvents(EventSystemManager manager)
        {
            
        }
    }
}