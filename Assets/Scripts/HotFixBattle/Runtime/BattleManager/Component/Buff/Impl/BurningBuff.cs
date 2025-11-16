using Aliyun.OSS;
using Game.Logic.BattleModule.Entity;

namespace HotFixBattle
{
    /// <summary>
    /// 燃烧Buff
    /// </summary>
    public class BurningBuff : Buff
    {
        /// <summary>
        /// 每秒伤害
        /// </summary>
        private int _damagePerSecond;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="owner">Buff所属实体</param>
        /// <param name="duration">持续时间</param>
        /// <param name="intensity">强度</param>
        public BurningBuff(BaseEntity owner, float duration, float intensity) 
            : base(owner, "燃烧", "持续受到火焰伤害", 3001, duration, intensity)
        {
            // 每秒伤害基于强度
            _damagePerSecond = (int)(5 * intensity); // 基础5点每秒，根据强度调整
        }

        /// <summary>
        /// Buff激活时的逻辑
        /// </summary>
        protected override void OnActivate()
        {
            // 添加燃烧状态
            Owner.StateManager.AddState(HotFixBattle.EntityState.Burning, Duration, Intensity);
        }

        /// <summary>
        /// Buff更新时的逻辑
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        protected override void OnUpdate(float deltaTime)
        {
            // 燃烧持续伤害已在EntityStateManager中处理
        }

        /// <summary>
        /// Buff停用时的逻辑
        /// </summary>
        protected override void OnDeactivate()
        {
            // 移除燃烧状态
            Owner.StateManager.RemoveState(HotFixBattle.EntityState.Burning);
        }
    }
}