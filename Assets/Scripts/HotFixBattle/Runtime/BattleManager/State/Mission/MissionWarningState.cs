using cfg;

namespace HotFixBattle
{
    /// <summary>
    /// 任务警告状态
    /// </summary>
    public class MissionWarningState : MissionStateBase
    {
        private float _warningDuration;
        private float _warningTimer;

        public MissionWarningState(BattleMissionController missionController) : base(missionController) 
        {
            _warningDuration = 2.0f; // 警告持续时间为2秒
        }

        public override void Enter()
        {
            _warningTimer = 0f;
            // 触发任务警告事件
            _missionController.OnMissionWarning();
            UnityEngine.Debug.Log($"[MissionWarningState] Enter - Mission ID: {_missionController.MissionData.Id}, 警告时长: {_warningDuration}秒");
        }

        public override void Update(float deltaTime)
        {
            _warningTimer += deltaTime;

            // 警告时间结束后，切换到进行中状态
            if (_warningTimer >= _warningDuration)
            {
                UnityEngine.Debug.Log($"[MissionWarningState] Update - Mission ID: {_missionController.MissionData.Id}, 警告结束，切换到InProgress状态");
                _missionController.ChangeState(new MissionInProgressState(_missionController));
            }
        }

        public override void Exit()
        {
            // 退出状态时的清理工作
            UnityEngine.Debug.Log($"[MissionWarningState] Exit - Mission ID: {_missionController.MissionData.Id}");
        }

        public override MissionState GetStateType()
        {
            return MissionState.Warning;
        }
    }
}
