using cfg;

namespace HotFixBattle
{
    /// <summary>
    /// 波次进行中状态
    /// </summary>
    public class WaveInProgressState : WaveStateBase
    {
        public WaveInProgressState(BattleWaveController waveController) : base(waveController) { }

        public override void Enter()
        {
            // 开始执行波次中的任务
            _waveController.StartMissions();
            UnityEngine.Debug.Log($"[WaveInProgressState] Enter - WaveGroup ID: {_waveController.WaveGroupData.Id}, 任务数量: {_waveController.WaveGroupData.WaveIds.Length}");
        }

        public override void Update(float deltaTime)
        {
            // 更新当前任务
            _waveController.UpdateCurrentMission(deltaTime);

            // 检查是否有需要打断的逻辑
            if (_waveController.CheckInterrupt())
            {
                // 如果有打断逻辑，则暂停执行
                return;
            }

            // 检查是否所有任务都已完成
            if (_waveController.AreAllMissionsCompleted())
            {
                UnityEngine.Debug.Log($"[WaveInProgressState] Update - WaveGroup ID: {_waveController.WaveGroupData.Id}, 所有任务完成，切换到Exiting状态");
                _waveController.ChangeState(new WaveExitingState(_waveController));
            }
        }

        public override void Exit()
        {
            // 退出状态时的清理工作
            UnityEngine.Debug.Log($"[WaveInProgressState] Exit - WaveGroup ID: {_waveController.WaveGroupData.Id}");
        }

        public override WaveState GetStateType()
        {
            return WaveState.InProgress;
        }
    }
}
