using cfg;

namespace HotFixBattle
{
    /// <summary>
    /// 任务未开始状态
    /// </summary>
    public class MissionNotStartedState : MissionStateBase
    {
        public MissionNotStartedState(BattleMissionController missionController) : base(missionController) { }

        public override void Enter()
        {
            // 任务未开始，不执行任何操作
            UnityEngine.Debug.Log($"[MissionNotStartedState] Enter - Mission ID: {_missionController.MissionData.Id}");
        }

        public override void Update(float deltaTime)
        {
            // 检查是否可以开始任务
            if (_missionController.CanStartMission())
            {
                UnityEngine.Debug.Log($"[MissionNotStartedState] Update - Mission ID: {_missionController.MissionData.Id}, 准备切换到Warning状态");
                _missionController.ChangeState(new MissionWarningState(_missionController));
            }
        }

        public override void Exit()
        {
            // 退出状态时的清理工作
            UnityEngine.Debug.Log($"[MissionNotStartedState] Exit - Mission ID: {_missionController.MissionData.Id}");
        }

        public override MissionState GetStateType()
        {
            return MissionState.NotStarted;
        }
    }
}
