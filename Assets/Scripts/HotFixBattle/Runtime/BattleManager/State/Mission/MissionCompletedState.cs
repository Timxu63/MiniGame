using cfg;
using Framework.Runtime;

namespace HotFixBattle
{
    /// <summary>
    /// 任务已完成状态
    /// </summary>
    public class MissionCompletedState : MissionStateBase
    {
        public MissionCompletedState(BattleMissionController missionController) : base(missionController) { }

        public override void Enter()
        {
            // 触发任务完成事件
            _missionController.OnMissionCompleted();
            UnityEngine.Debug.Log($"[MissionCompletedState] Enter - Mission ID: {_missionController.MissionData.Id}");
        }

        public override void Update(float deltaTime)
        {
            // 任务已完成，不执行任何操作
            // 可以在这里通知任务控制器切换到下一个任务
        }

        public override void Exit()
        {
            // 退出状态时的清理工作
            UnityEngine.Debug.Log($"[MissionCompletedState] Exit - Mission ID: {_missionController.MissionData.Id}");
        }

        public override MissionState GetStateType()
        {
            return MissionState.Completed;
        }
    }
}
