using System.Collections.Generic;
using cfg;
using Framework.Logic.Modules;
using Framework.Runtime;
using HotFix;

namespace HotFixBattle
{
    public abstract class BattleManagerBase
    {
        protected BattleWorldContext _worldContext;
        protected BattleState _currentState;
        protected Chapter _currentChapter;
        protected IChapterHandler _chapterHandler;
        protected BattleWaveController _currentWaveController;
        protected List<BattleWaveController> _completedWaveControllers;

        public bool IsRunning => _currentState == BattleState.Running;
        public BattleState CurrentState => _currentState;
        public bool IsBattleCompleted => _currentState == BattleState.Ended;

        public virtual void Initialize(BattleWorldContext worldContext)
        {
            _worldContext = worldContext;
            _currentState = BattleState.Initializing;
            _completedWaveControllers = new List<BattleWaveController>();

            // 初始化章节处理器工厂
            ChapterHandlerFactory.InitializeDefaultHandlers();
        }

        /// <summary>
        /// 开始战斗
        /// </summary>
        /// <param name="chapterId">章节ID</param>
        public virtual void StartBattle(int chapterId)
        {
            _currentChapter = _worldContext.Tables.TbChapter.Get(chapterId);
            if (_currentChapter == null)
            {
                return;
            }

            // 根据章节类型创建对应的处理器
            _chapterHandler = ChapterHandlerFactory.CreateHandler(_currentChapter.Type);
            _chapterHandler.Initialize(_currentChapter, _worldContext);


            // 设置战斗状态为运行中
            _currentState = BattleState.Running;
            
            // 开始第一个波次
            StartNextWaveGroup();
        }

        /// <summary>
        /// 更新战斗逻辑
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public virtual void Update(float deltaTime)
        {
            if (!IsRunning)
            {
                return;
            }

            // 更新当前波次
            _currentWaveController?.Update(deltaTime);

            // 检查当前波次是否完成
            if (_currentWaveController != null && _currentWaveController.IsCompleted)
            {
                _completedWaveControllers.Add(_currentWaveController);
                _currentWaveController = null;

                // 开始下一个波次
                if (!StartNextWaveGroup())
                {
                    // 没有更多波次，战斗结束
                    EndBattle();
                }
            }
        }

        /// <summary>
        /// 开始下一个波次
        /// </summary>
        /// <returns>是否成功开始下一个波次</returns>
        protected virtual bool StartNextWaveGroup()
        {
            if (_chapterHandler == null || !_chapterHandler.HasMoreWaveGroups())
            {
                return false;
            }

            var waveGroupData = _chapterHandler.GetNextWaveGroup();
            if (waveGroupData == null)
            {
                return false;
            }

            _currentWaveController = new BattleWaveController(waveGroupData, _worldContext, _chapterHandler);
            return true;
        }

        /// <summary>
        /// 结束战斗
        /// </summary>
        protected virtual async void EndBattle()
        {
            _currentState = BattleState.Ended;

            // 触发战斗结束事件
            // EventSystem.Instance.TriggerEvent(new BattleEndEvent());

            // 所有wave结束后，从gamestate状态切换到mainstate状态
            
            GameApp.State.ActiveState((int)StateName.MainState);
        }

        /// <summary>
        /// 暂停战斗
        /// </summary>
        public virtual void PauseBattle()
        {
            if (_currentState == BattleState.Running)
            {
                _currentState = BattleState.Paused;

                // 暂停当前波次
                _currentWaveController?.SetInterrupted(true);
            }
        }

        /// <summary>
        /// 恢复战斗
        /// </summary>
        public virtual void ResumeBattle()
        {
            if (_currentState == BattleState.Paused)
            {
                _currentState = BattleState.Running;
                // 恢复当前波次
                _currentWaveController?.SetInterrupted(false);
            }
        }
    }
}
