using System;
using cfg;
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
                    var gameEnterArgs = GameApp.Event.GetEvent<EventArgsGameDataEnter>();
                    gameEnterArgs.GameModel = eChapterType.Normal;
                    var modeData = BattleUtil.GetMainBattleTransferDataFromServer(battleId);
                    gameEnterArgs.ModeData = modeData;
                    GameApp.Event.DispatchNow((int)LocalMessageName.CC_GameData_GameEnter, gameEnterArgs);
                    State.ActiveState((int)StateName.GameState);
                });
            });
        }
    }
}