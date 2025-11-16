
using Game.Logic.BattleModule.Entity;

namespace Game.Logic.BattleModule.Component.Weapon.Impl
{
    /// <summary>
    /// 剑武器实现
    /// </summary>
    public class SwordWeapon : Weapon
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="owner">武器所属实体</param>
        /// <param name="id">武器ID</param>
        /// <param name="name">武器名称</param>
        /// <param name="description">武器描述</param>
        /// <param name="iconId">武器图标ID</param>
        /// <param name="baseDamage">基础攻击力</param>
        /// <param name="attackSpeed">攻击速度</param>
        /// <param name="attackRange">攻击范围</param>
        public SwordWeapon(BaseEntity owner, int id, string name, string description, int iconId, 
                          int baseDamage, float attackSpeed, float attackRange) 
            : base(owner, id, name, description, iconId, WeaponType.Sword, baseDamage, attackSpeed, attackRange)
        {
        }

        /// <summary>
        /// 创建一把普通的剑
        /// </summary>
        /// <param name="owner">武器所属实体</param>
        /// <returns>剑武器实例</returns>
        public static SwordWeapon CreateNormalSword(BaseEntity owner)
        {
            return new SwordWeapon(owner, 1001, "普通铁剑", "一把普通的铁剑", 2001, 10, 1.2f, 1.5f);
        }

        /// <summary>
        /// 创建一把精良的剑
        /// </summary>
        /// <param name="owner">武器所属实体</param>
        /// <returns>剑武器实例</returns>
        public static SwordWeapon CreateFineSword(BaseEntity owner)
        {
            return new SwordWeapon(owner, 1002, "精良钢剑", "一把精良的钢剑", 2002, 15, 1.5f, 1.5f);
        }
    }
}
