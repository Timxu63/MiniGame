using cfg;

namespace HotFixBattle
{
    /// <summary>
    /// 任务进行中状态
    /// </summary>
    public class MissionInProgressState : MissionStateBase
    {
        private float _missionTimer;
        private float _missionDuration;

        public MissionInProgressState(BattleMissionController missionController) : base(missionController) 
        {
            _missionDuration = missionController.GetMissionDuration();
        }

        public override void Enter()
        {
            
            _missionTimer = 0f;
            // 触发任务开始事件
            _missionController.OnMissionStart();
            // 生成怪物
            _missionController.SpawnMonsters();
            UnityEngine.Debug.Log($"[MissionInProgressState] Enter - Mission ID: {_missionController.MissionData.Id}, 任务时长: {_missionDuration}秒, 怪物数量: {_missionController.MissionData.MonsterId.Length}");
        }

        public override void Update(float deltaTime)
        {
            _missionTimer += deltaTime;

            // 更新任务逻辑
            _missionController.UpdateMissionLogic(deltaTime);

            // 检查任务是否完成（时间到或者所有怪物被消灭）
            if (_missionTimer >= _missionDuration || _missionController.AreAllMonstersDefeated())
            {
                string reason = _missionTimer >= _missionDuration ? "时间结束" : "怪物全部消灭";
                UnityEngine.Debug.Log($"[MissionInProgressState] Update - Mission ID: {_missionController.MissionData.Id}, 任务完成，原因: {reason}");
                _missionController.ChangeState(new MissionCompletedState(_missionController));
            }
        }

        public override void Exit()
        {
            // 退出状态时的清理工作
            _missionController.CleanupMission();
            UnityEngine.Debug.Log($"[MissionInProgressState] Exit - Mission ID: {_missionController.MissionData.Id}");
        }

        public override MissionState GetStateType()
        {
            return MissionState.InProgress;
        }
    }
}
