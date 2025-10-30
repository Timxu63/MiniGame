using System.Threading.Tasks;
using cfg;
using Framework.EventSystem;
using Framework.Runtime;
using Framework.State;
using HotFixBattle;

namespace HotFix
{
    public class GameState : State
    {
        private BattleFlowController _battleFlowController;
        private BattleDataModule _battleDataModule;
        private BattleWorldContext _worldContext;
        public override int GetName()
        {
            return (int) StateName.GameState;
        }

        public override void OnEnter()
        {
            InitWorldContent();
            _battleDataModule =
                GameApp.DataModule.GetDataModule<BattleDataModule>((int)DataName.BattleDataModule);
#if UNITY_EDITOR
            if (GameApp.IsEditorScene)
            {
                _battleDataModule.m_openBattleData = new EventArgsGameDataEnter();
                _battleDataModule.m_openBattleData.GameModel = eChapterType.Normal;
                _battleDataModule.m_openBattleData.ModeData = new ModeDataNormal();
                _battleDataModule.m_openBattleData.ModeData.ChapterId = 1;
            }
#endif
            // 创建战斗流程控制器
            _battleFlowController = new BattleFlowController(_worldContext);
            
            // 根据游戏模式初始化对应的战斗管理器
            System.Type managerType = BattleUtil.GetBattleManagerType(_battleDataModule.m_openBattleData.GameModel);
            _battleFlowController.InitializeBattleManager(managerType);
            AsyncInitAsset();
        }

        private async void AsyncInitAsset()
        {
            await AsyncInitSceneAsset();
            await AsyncInitUIAsset();
            OnAsyncFinish();
        }

        private void OnAsyncFinish()
        {
            _battleFlowController.StartBattle(_battleDataModule.m_openBattleData.ModeData.ChapterId);
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

        private void InitWorldContent()
        {
            _worldContext = new BattleWorldContext();
            _worldContext.Tables = GameTableProxy.Tables;
        }
        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            _battleFlowController.UpdateBattle(deltaTime);
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