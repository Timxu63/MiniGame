
using cfg;

namespace HotFixBattle
{
    /// <summary>
    /// 波次状态基类
    /// </summary>
    public abstract class WaveStateBase
    {
        protected BattleWaveController _waveController;

        public WaveStateBase(BattleWaveController waveController)
        {
            _waveController = waveController;
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
        /// <returns>波次状态</returns>
        public abstract WaveState GetStateType();
    }
}
