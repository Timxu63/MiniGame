using System;
using Framework.Runtime;
using Framework.State;
using Framework.ViewModule;

namespace HotFix.Runtime.Logic
{
    public static partial class Game
    {
        public static StateManager State => GameApp.State;
        public static void JoinBattle(int battleId, Action callback)
        {
            GameApp.View.OpenView(ViewName.UILoading, null, UILayers.Second, null, (loadObj) =>
            {
                var loadingViewModule =
                    GameApp.View.GetViewModule<UILoadingViewModule>(ViewName.UILoading);
                loadingViewModule.PlayShow(() =>
                {
                    State.ActiveState((int)StateName.GameState);
                });
            });
        }
    }
}