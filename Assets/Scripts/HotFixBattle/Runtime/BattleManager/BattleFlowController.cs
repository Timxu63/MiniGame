using System;
using cfg;

namespace HotFixBattle
{
    /// <summary>
    /// 战斗流程控制器
    /// </summary>
    public class BattleFlowController
    {
        private BattleManagerBase _battleManager;
        private BattleWorldContext _worldContext;

        public BattleManagerBase BattleManager => _battleManager;
        public bool IsBattleRunning => _battleManager != null && _battleManager.IsRunning;

        public BattleFlowController(BattleWorldContext worldContext)
        {
            _worldContext = worldContext;
        }

        /// <summary>
        /// 初始化战斗管理器
        /// </summary>
        /// <param name="battleManagerType">战斗管理器类型</param>
        public void InitializeBattleManager(Type battleManagerType)
        {
            if (battleManagerType == null || !typeof(BattleManagerBase).IsAssignableFrom(battleManagerType))
            {
                return;
            }

            _battleManager = (BattleManagerBase)Activator.CreateInstance(battleManagerType);
            _battleManager.Initialize(_worldContext);
        }

        /// <summary>
        /// 开始战斗
        /// </summary>
        /// <param name="chapterId">章节ID</param>
        public void StartBattle(int chapterId)
        {
            if (_battleManager == null)
            {
                return;
            }

            _battleManager.StartBattle(chapterId);
        }

        /// <summary>
        /// 更新战斗逻辑
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public void UpdateBattle(float deltaTime)
        {
            _battleManager?.Update(deltaTime);
        }

        /// <summary>
        /// 暂停战斗
        /// </summary>
        public void PauseBattle()
        {
            _battleManager?.PauseBattle();
        }

        /// <summary>
        /// 恢复战斗
        /// </summary>
        public void ResumeBattle()
        {
            _battleManager?.ResumeBattle();
        }

        /// <summary>
        /// 结束战斗
        /// </summary>
        public void EndBattle()
        {
            if (_battleManager != null && _battleManager.IsRunning)
            {
                _battleManager.PauseBattle();
                // 可以添加更多的战斗结束逻辑
            }
        }
    }
}
