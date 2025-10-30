using cfg;

namespace HotFixBattle
{
    /// <summary>
    /// 波次未开始状态
    /// </summary>
    public class WaveNotStartedState : WaveStateBase
    {
        public WaveNotStartedState(BattleWaveController waveController) : base(waveController) { }

        public override void Enter()
        {
            // 波次未开始，不执行任何操作
            UnityEngine.Debug.Log($"[WaveNotStartedState] Enter - WaveGroup ID: {_waveController.WaveGroupData.Id}");
        }

        public override void Update(float deltaTime)
        {
            // 检查是否可以进入波次
            if (_waveController.CanEnterWave())
            {
                UnityEngine.Debug.Log($"[WaveNotStartedState] Update - WaveGroup ID: {_waveController.WaveGroupData.Id}, 准备切换到Entering状态");
                _waveController.ChangeState(new WaveEnteringState(_waveController));
            }
        }

        public override void Exit()
        {
            // 退出状态时的清理工作
            UnityEngine.Debug.Log($"[WaveNotStartedState] Exit - WaveGroup ID: {_waveController.WaveGroupData.Id}");
        }

        public override WaveState GetStateType()
        {
            return WaveState.NotStarted;
        }
    }
}
