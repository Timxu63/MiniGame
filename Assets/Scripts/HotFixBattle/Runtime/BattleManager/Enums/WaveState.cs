
namespace HotFixBattle
{
    /// <summary>
    /// 波次状态枚举
    /// </summary>
    public enum WaveState
    {
        /// <summary>
        /// 未开始
        /// </summary>
        NotStarted,

        /// <summary>
        /// 准备进入
        /// </summary>
        Entering,

        /// <summary>
        /// 进行中
        /// </summary>
        InProgress,

        /// <summary>
        /// 准备退出
        /// </summary>
        Exiting,

        /// <summary>
        /// 已完成
        /// </summary>
        Completed
    }
}
