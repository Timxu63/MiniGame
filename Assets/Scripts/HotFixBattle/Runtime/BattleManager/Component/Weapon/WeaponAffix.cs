using Game.Logic.BattleModule.Entity;

namespace Game.Logic.BattleModule.Component.Weapon
{
    /// <summary>
    /// 武器词条基类
    /// </summary>
    public abstract class WeaponAffix
    {
        /// <summary>
        /// 词条ID
        /// </summary>
        public int Id { get; protected set; }

        /// <summary>
        /// 词条名称
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// 词条描述
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// 词条等级
        /// </summary>
        public int Level { get; protected set; }

        /// <summary>
        /// 词条所属武器
        /// </summary>
        protected Weapon _weapon;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">词条ID</param>
        /// <param name="name">词条名称</param>
        /// <param name="description">词条描述</param>
        /// <param name="level">词条等级</param>
        protected WeaponAffix(int id, string name, string description, int level = 1)
        {
            Id = id;
            Name = name;
            Description = description;
            Level = level;
        }

        /// <summary>
        /// 词条添加到武器时调用
        /// </summary>
        /// <param name="weapon">词条所属的武器</param>
        public virtual void OnAdded(Weapon weapon)
        {
            _weapon = weapon;
        }

        /// <summary>
        /// 词条从武器移除时调用
        /// </summary>
        /// <param name="weapon">词条所属的武器</param>
        public virtual void OnRemoved(Weapon weapon)
        {
            _weapon = null;
        }

        /// <summary>
        /// 修改伤害
        /// </summary>
        /// <param name="originalDamage">原始伤害</param>
        /// <param name="target">目标实体</param>
        /// <returns>修改后的伤害</returns>
        public virtual int ModifyDamage(int originalDamage, BaseEntity target)
        {
            return originalDamage;
        }

        /// <summary>
        /// 攻击命中时调用
        /// </summary>
        /// <param name="target">目标实体</param>
        /// <param name="damage">造成的伤害</param>
        public virtual void OnAttackHit(BaseEntity target, int damage)
        {
            // 子类可重写此方法实现特殊效果
        }
    }
}
