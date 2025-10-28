using System.Threading.Tasks;
using Framework.EventSystem;
using Framework.Runtime;
using Framework.State;

namespace HotFix
{
    public class GameState : State
    {
        public override int GetName()
        {
            return (int) StateName.GameState;
        }

        public override void OnEnter()
        {
            AsyncInitAsset();
        }

        private async void AsyncInitAsset()
        {
            await AsyncInitSceneAsset();
            await AsyncInitUIAsset();
        }

        private async Task AsyncInitSceneAsset()
        {
            
        }

        private async Task AsyncInitUIAsset()
        {
            await GameApp.View.OpenViewTask(ViewName.UIBattle);
            var uiLoadingViewModule = GameApp.View.GetViewModule<UILoadingViewModule>(ViewName.UILoading);
            if (uiLoadingViewModule != null)
            {
                uiLoadingViewModule.PlayHide(() =>
                {
                    GameApp.View.CloseView(ViewName.UILoading);
                });
            }
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
            GameNode.Instance.OnWorldToMain();
        }

        public override void RegisterEvents(EventSystemManager manager)
        {
        }

        public override void UnRegisterEvents(EventSystemManager manager)
        {
        }
    }
}