using System.Collections;
using cfg;
using Framework.Runtime;

namespace HotFixBattle
{
    /// <summary>
    /// 波次进入状态
    /// </summary>
    public class WaveEnteringState : WaveStateBase
    {
        private float _enterDuration;
        private float _enterTimer;

        public WaveEnteringState(BattleWaveController waveController) : base(waveController) 
        {
            _enterDuration = 1.0f; // 进入持续时间为1秒
            BattleWaveChangeEventArgs eventArgs = new BattleWaveChangeEventArgs();
            eventArgs.MissionId = waveController.WaveGroupData.Id;
            GameApp.Event.DispatchNow((int)LocalMessageName.CC_BattleWaveChange, eventArgs);
        }

        public override void Enter()
        {
            _enterTimer = 0f;
            // 触发波次进入事件
            _waveController.OnWaveEnter();
            UnityEngine.Debug.Log($"[WaveEnteringState] Enter - WaveGroup ID: {_waveController.WaveGroupData.Id}, 进入时长: {_enterDuration}秒");
        }

        public override void Update(float deltaTime)
        {
            _enterTimer += deltaTime;

            // 检查是否有需要打断的逻辑
            if (_waveController.CheckInterrupt())
            {
                // 如果有打断逻辑，则暂停进入过程
                return;
            }

            // 进入时间结束后，切换到进行中状态
            if (_enterTimer >= _enterDuration)
            {
                UnityEngine.Debug.Log($"[WaveEnteringState] Update - WaveGroup ID: {_waveController.WaveGroupData.Id}, 进入结束，切换到InProgress状态");
                _waveController.ChangeState(new WaveInProgressState(_waveController));
            }
        }

        public override void Exit()
        {
            // 退出状态时的清理工作
            UnityEngine.Debug.Log($"[WaveEnteringState] Exit - WaveGroup ID: {_waveController.WaveGroupData.Id}");
        }

        public override WaveState GetStateType()
        {
            return WaveState.Entering;
        }
    }
}
