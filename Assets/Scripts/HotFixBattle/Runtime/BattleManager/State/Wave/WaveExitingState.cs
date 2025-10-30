using cfg;

namespace HotFixBattle
{
    /// <summary>
    /// 波次退出状态
    /// </summary>
    public class WaveExitingState : WaveStateBase
    {
        private float _exitDuration;
        private float _exitTimer;

        public WaveExitingState(BattleWaveController waveController) : base(waveController) 
        {
            _exitDuration = 1.0f; // 退出持续时间为1秒
        }

        public override void Enter()
        {
            _exitTimer = 0f;
            // 触发波次退出事件
            _waveController.OnWaveExit();
            UnityEngine.Debug.Log($"[WaveExitingState] Enter - WaveGroup ID: {_waveController.WaveGroupData.Id}, 退出时长: {_exitDuration}秒");
        }

        public override void Update(float deltaTime)
        {
            _exitTimer += deltaTime;

            // 检查是否有需要打断的逻辑
            if (_waveController.CheckInterrupt())
            {
                // 如果有打断逻辑，则暂停退出过程
                return;
            }

            // 退出时间结束后，切换到已完成状态
            if (_exitTimer >= _exitDuration)
            {
                UnityEngine.Debug.Log($"[WaveExitingState] Update - WaveGroup ID: {_waveController.WaveGroupData.Id}, 退出结束，切换到Completed状态");
                _waveController.ChangeState(new WaveCompletedState(_waveController));
            }
        }

        public override void Exit()
        {
            // 退出状态时的清理工作
        }

        public override WaveState GetStateType()
        {
            return WaveState.Exiting;
        }
    }
}
