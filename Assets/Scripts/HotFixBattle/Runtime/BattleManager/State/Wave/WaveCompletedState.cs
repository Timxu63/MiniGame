using cfg;

namespace HotFixBattle
{
    /// <summary>
    /// 波次已完成状态
    /// </summary>
    public class WaveCompletedState : WaveStateBase
    {
        public WaveCompletedState(BattleWaveController waveController) : base(waveController) { }

        public override void Enter()
        {
            // 触发波次完成事件
            _waveController.OnWaveCompleted();
        }

        public override void Update(float deltaTime)
        {
            // 波次已完成，不执行任何操作
            // 可以在这里通知波次控制器切换到下一个波次
        }

        public override void Exit()
        {
            // 退出状态时的清理工作
        }

        public override WaveState GetStateType()
        {
            return WaveState.Completed;
        }
    }
}
