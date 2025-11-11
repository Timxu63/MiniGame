using System.Threading.Tasks;
using cfg;
using Framework.EventSystem;
using Framework.Runtime;
using Framework.State;
using Game.Logic.BattleModule.Entity;
using HotFixBattle;
using UnityEngine;

namespace HotFix
{
    public class GameState : State
    {
        private BattleFlowController _battleFlowController;
        private BattleDataModule _battleDataModule;
        private BattleWorldContext _worldContext;
        private CameraManager _cameraManager;
        public override int GetName()
        {
            return (int) StateName.GameState;
        }

        public override void OnEnter()
        {
            AsyncInitCamera();
            InitWorldContent();
            // 初始化实体视图管理器
            EntityViewManager.Instance.Initialize();
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
            await AsyncInitPlayer();
            OnAsyncFinish();
        }

        private async Task AsyncInitCamera()
        {
            _cameraManager = new CameraManager();
            _cameraManager.InitCamera();
        }

        private async Task AsyncInitPlayer()
        {
            // 获取角色数据
            var charactor = _worldContext.Tables.TbCharactor.GetOrDefault(1);
            if (charactor == null)
            {
                Debug.LogError("[GameState] 找不到ID为1的角色数据");
                return;
            }

            // 获取地图中间点
            var mapManager = MapManager.Instance;
            if (!mapManager.IsInitialized)
            {
                Debug.LogError("[GameState] 地图管理器未初始化");
                return;
            }

            // 计算地图中心点世界坐标
            Vector3 centerPosition = new Vector3(
                mapManager.Width * mapManager.CellSize * 0.5f, 
                0, 
                mapManager.Height * mapManager.CellSize * 0.5f
            );

            // 使用EntityFactory创建玩家实体
            var playerParams = new PlayerCreationParams
            {
                Name = charactor.Name,
                MaxHealth = 100,  // 默认最大生命值
                CharactorConfig = charactor,
                Level = 1,        // 默认等级
                AttackPower = 10, // 默认攻击力
                Defense = 5,      // 默认防御力
                Position = centerPosition // 设置玩家位置为地图中心点
            };

            var playerEntity = EntityFactory.CreateEntity(eEntityType.Player, playerParams) as PlayerEntity;

            // EntityFactory已自动设置位置并将实体添加到SimpleEntityManager

            Debug.Log($"[GameState] 成功创建玩家实体，ID: {playerEntity.Id}, 位置: {centerPosition}");
        }

        private void OnAsyncFinish()
        {
            _battleFlowController.StartBattle(_battleDataModule.m_openBattleData.ModeData.ChapterId);
        }

        public override void OnLateUpdate(float deltaTime, float unscaledDeltaTime)
        {
            _cameraManager.OnLateUpdate(deltaTime, unscaledDeltaTime);
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
            MapManager.Instance.Initialize(_worldContext);
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
            _cameraManager = null;
            // 清理实体视图管理器
            EntityViewManager.Instance.Cleanup();
            GameApp.View.CloseAllView(new int[]
            {
                (int)ViewName.UILoading
            });
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