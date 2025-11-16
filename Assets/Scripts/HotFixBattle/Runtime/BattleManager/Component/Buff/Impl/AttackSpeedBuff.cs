using Game.Logic.BattleModule.Component.Weapon;
using Game.Logic.BattleModule.Entity;

namespace HotFixBattle
{
    /// <summary>
    /// 攻击速度Buff
    /// </summary>
    public class AttackSpeedBuff : Buff
    {
        /// <summary>
        /// 攻击速度增加百分比
        /// </summary>
        private float _attackSpeedBonus;

        /// <summary>
        /// 原始攻击速度
        /// </summary>
        private float _originalAttackSpeed;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="owner">Buff所属实体</param>
        /// <param name="duration">持续时间</param>
        /// <param name="intensity">强度</param>
        public AttackSpeedBuff(BaseEntity owner, float duration, float intensity) 
            : base(owner, "攻击速度提升", "提升攻击速度", 1001, duration, intensity)
        {
            // 攻击速度增加百分比基于强度
            _attackSpeedBonus = 0.2f * intensity; // 基础20%，根据强度调整
        }

        /// <summary>
        /// Buff激活时的逻辑
        /// </summary>
        protected override void OnActivate()
        {
            // 获取武器组件
            var weaponComponent = Owner.ComponentManager.GetComponent<WeaponComponent>();
            if (weaponComponent != null && weaponComponent.CurrentWeapon != null)
            {
                // 保存原始攻击速度
                _originalAttackSpeed = weaponComponent.CurrentWeapon.AttackSpeed;

                // 增加攻击速度
                weaponComponent.CurrentWeapon.AttackSpeed *= (1 + _attackSpeedBonus);
            }
        }

        /// <summary>
        /// Buff更新时的逻辑
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        protected override void OnUpdate(float deltaTime)
        {
            // 攻击速度Buff不需要持续更新
        }

        /// <summary>
        /// Buff停用时的逻辑
        /// </summary>
        protected override void OnDeactivate()
        {
            // 获取武器组件
            var weaponComponent = Owner.ComponentManager.GetComponent<WeaponComponent>();
            if (weaponComponent != null && weaponComponent.CurrentWeapon != null)
            {
                // 恢复原始攻击速度
                weaponComponent.CurrentWeapon.AttackSpeed = _originalAttackSpeed;
            }
        }
    }
}
