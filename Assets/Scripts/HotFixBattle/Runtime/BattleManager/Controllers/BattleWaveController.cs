
using System.Collections.Generic;
using cfg;
using HotFix;

namespace HotFixBattle
{
    /// <summary>
    /// 战斗波次控制器
    /// </summary>
    public class BattleWaveController
    {
        private Chapter_WaveGroup _waveGroupData;
        private BattleWorldContext _worldContext;
        private IChapterHandler _chapterHandler;

        private WaveStateBase _currentState;
        private List<BattleMissionController> _missionControllers;
        private int _currentMissionIndex;
        private WaveState _currentStateType;

        private bool _isInterrupted;

        public Chapter_WaveGroup WaveGroupData => _waveGroupData;
        public WaveState CurrentStateType => _currentStateType;
        public bool IsCompleted => _currentStateType == WaveState.Completed;
        public bool IsInterrupted => _isInterrupted;

        public BattleWaveController(Chapter_WaveGroup waveGroupData, BattleWorldContext worldContext, IChapterHandler chapterHandler)
        {
            _waveGroupData = waveGroupData;
            _worldContext = worldContext;
            _chapterHandler = chapterHandler;
            _missionControllers = new List<BattleMissionController>();
            _currentMissionIndex = 0;
            _isInterrupted = false;

            // 初始化任务控制器
            InitializeMissionControllers();

            // 设置初始状态
            _currentState = new WaveNotStartedState(this);
            _currentStateType = WaveState.NotStarted;
            _currentState.Enter();
        }

        /// <summary>
        /// 初始化任务控制器
        /// </summary>
        private void InitializeMissionControllers()
        {
            foreach (var waveId in _waveGroupData.WaveIds)
            {
                var missionData = _worldContext.Tables.TbChapterMission.Get(waveId);
                if (missionData != null)
                {
                    var missionController = new BattleMissionController(missionData, _worldContext);
                    _missionControllers.Add(missionController);
                }
            }
        }

        /// <summary>
        /// 更新波次
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public void Update(float deltaTime)
        {
            _currentState.Update(deltaTime);
        }

        /// <summary>
        /// 改变状态
        /// </summary>
        /// <param name="newState">新状态</param>
        public void ChangeState(WaveStateBase newState)
        {
            _currentState.Exit();
            _currentState = newState;
            _currentStateType = newState.GetStateType();
            _currentState.Enter();
        }

        /// <summary>
        /// 检查是否可以进入波次
        /// </summary>
        /// <returns>是否可以进入</returns>
        public bool CanEnterWave()
        {
            // 可以添加额外的条件检查
            return !_isInterrupted;
        }

        /// <summary>
        /// 开始执行任务
        /// </summary>
        public void StartMissions()
        {
            if (_missionControllers.Count > 0)
            {
                _currentMissionIndex = 0;
                _missionControllers[0].StartMission();
            }
        }

        /// <summary>
        /// 更新当前任务
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public void UpdateCurrentMission(float deltaTime)
        {
            if (_currentMissionIndex < _missionControllers.Count)
            {
                var currentMission = _missionControllers[_currentMissionIndex];
                currentMission.Update(deltaTime);

                // 如果当前任务已完成，切换到下一个任务
                if (currentMission.IsCompleted)
                {
                    _currentMissionIndex++;
                    if (_currentMissionIndex < _missionControllers.Count)
                    {
                        _missionControllers[_currentMissionIndex].StartMission();
                    }
                }
            }
        }

        /// <summary>
        /// 检查是否所有任务都已完成
        /// </summary>
        /// <returns>是否所有任务都已完成</returns>
        public bool AreAllMissionsCompleted()
        {
            foreach (var mission in _missionControllers)
            {
                if (!mission.IsCompleted)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 检查是否有需要打断的逻辑
        /// </summary>
        /// <returns>是否有需要打断的逻辑</returns>
        public bool CheckInterrupt()
        {
            // 这里可以实现具体的打断逻辑
            return _isInterrupted;
        }

        /// <summary>
        /// 设置打断状态
        /// </summary>
        /// <param name="interrupted">是否打断</param>
        public void SetInterrupted(bool interrupted)
        {
            _isInterrupted = interrupted;
        }

        /// <summary>
        /// 波次进入事件
        /// </summary>
        public void OnWaveEnter()
        {
            _chapterHandler.OnWaveGroupStart(_waveGroupData);
            // 可以添加额外的波次进入逻辑
        }

        /// <summary>
        /// 波次退出事件
        /// </summary>
        public void OnWaveExit()
        {
            // 可以添加额外的波次退出逻辑
        }

        /// <summary>
        /// 波次完成事件
        /// </summary>
        public void OnWaveCompleted()
        {
            _chapterHandler.OnWaveGroupEnd(_waveGroupData);
            // 可以添加额外的波次完成逻辑
        }
    }
}
