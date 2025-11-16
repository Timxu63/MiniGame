using Game.Logic.BattleModule.Entity;

namespace HotFixBattle
{
    /// <summary>
    /// 伤害加成Buff
    /// </summary>
    public class DamageBuff : Buff
    {
        /// <summary>
        /// 伤害增加百分比
        /// </summary>
        private float _damageBonus;

        /// <summary>
        /// 原始攻击力
        /// </summary>
        private int _originalAttackPower;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="owner">Buff所属实体</param>
        /// <param name="duration">持续时间</param>
        /// <param name="intensity">强度</param>
        public DamageBuff(BaseEntity owner, float duration, float intensity) 
            : base(owner, "伤害提升", "提升攻击伤害", 1002, duration, intensity)
        {
            // 伤害增加百分比基于强度
            _damageBonus = 0.3f * intensity; // 基础30%，根据强度调整
        }

        /// <summary>
        /// Buff激活时的逻辑
        /// </summary>
        protected override void OnActivate()
        {
            // 如果是怪物实体
            if (Owner is MonsterEntity monster)
            {
                // 保存原始攻击力
                _originalAttackPower = monster.AttackPower;

                // 增加攻击力
                monster.AttackPower = (int)(_originalAttackPower * (1 + _damageBonus));
            }
            // 如果是玩家实体
            else if (Owner is PlayerEntity player)
            {
                // 保存原始攻击力
                _originalAttackPower = player.AttackPower;

                // 增加攻击力
                player.AttackPower = (int)(_originalAttackPower * (1 + _damageBonus));
            }
        }

        /// <summary>
        /// Buff更新时的逻辑
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        protected override void OnUpdate(float deltaTime)
        {
            // 伤害Buff不需要持续更新
        }

        /// <summary>
        /// Buff停用时的逻辑
        /// </summary>
        protected override void OnDeactivate()
        {
            // 如果是怪物实体
            if (Owner is MonsterEntity monster)
            {
                // 恢复原始攻击力
                monster.AttackPower = _originalAttackPower;
            }
            // 如果是玩家实体
            else if (Owner is PlayerEntity player)
            {
                // 恢复原始攻击力
                player.AttackPower = _originalAttackPower;
            }
        }
    }
}
