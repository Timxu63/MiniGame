
using cfg;

namespace HotFixBattle
{
    /// <summary>
    /// 任务状态基类
    /// </summary>
    public abstract class MissionStateBase
    {
        protected BattleMissionController _missionController;

        public MissionStateBase(BattleMissionController missionController)
        {
            _missionController = missionController;
        }

        /// <summary>
        /// 进入状态
        /// </summary>
        public abstract void Enter();

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public abstract void Update(float deltaTime);

        /// <summary>
        /// 退出状态
        /// </summary>
        public abstract void Exit();

        /// <summary>
        /// 获取当前状态类型
        /// </summary>
        /// <returns>任务状态</returns>
        public abstract MissionState GetStateType();
    }
}
