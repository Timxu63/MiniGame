using Game.Logic.BattleModule.Entity;
using HotFixBattle;

namespace Game.Logic.BattleModule.Component.Weapon.Impl
{
    /// <summary>
    /// 火焰伤害词条
    /// </summary>
    public class FireDamageAffix : WeaponAffix
    {
        /// <summary>
        /// 火焰伤害加成百分比
        /// </summary>
        private float _fireDamageBonus;

        /// <summary>
        /// 燃烧概率
        /// </summary>
        private float _burnChance;

        /// <summary>
        /// 燃烧持续时间
        /// </summary>
        private float _burnDuration;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="level">词条等级</param>
        public FireDamageAffix(int level = 1) : base(3001, "火焰伤害", "攻击时有几率造成额外火焰伤害并点燃目标", level)
        {
            // 根据等级设置属性
            switch (level)
            {
                case 1:
                    _fireDamageBonus = 0.15f; // 15%额外火焰伤害
                    _burnChance = 0.2f; // 20%点燃概率
                    _burnDuration = 3f; // 3秒燃烧时间
                    break;
                case 2:
                    _fireDamageBonus = 0.25f; // 25%额外火焰伤害
                    _burnChance = 0.3f; // 30%点燃概率
                    _burnDuration = 4f; // 4秒燃烧时间
                    break;
                case 3:
                    _fireDamageBonus = 0.35f; // 35%额外火焰伤害
                    _burnChance = 0.4f; // 40%点燃概率
                    _burnDuration = 5f; // 5秒燃烧时间
                    break;
                default:
                    _fireDamageBonus = 0.15f;
                    _burnChance = 0.2f;
                    _burnDuration = 3f;
                    break;
            }
        }

        /// <summary>
        /// 修改伤害
        /// </summary>
        /// <param name="originalDamage">原始伤害</param>
        /// <param name="target">目标实体</param>
        /// <returns>修改后的伤害</returns>
        public override int ModifyDamage(int originalDamage, BaseEntity target)
        {
            // 增加火焰伤害
            return originalDamage + (int)(originalDamage * _fireDamageBonus);
        }

        /// <summary>
        /// 攻击命中时调用
        /// </summary>
        /// <param name="target">目标实体</param>
        /// <param name="damage">造成的伤害</param>
        public override void OnAttackHit(BaseEntity target, int damage)
        {
            // 检查是否触发燃烧
            if (UnityEngine.Random.value < _burnChance)
            {
                // 获取目标的Buff管理器
                var buffManager = target.ComponentManager.GetComponent<BuffManager>();
                if (buffManager != null)
                {
                    // 添加燃烧Buff
                    buffManager.AddBuff<BurningBuff>(_burnDuration, 1.0f);
                }
            }
        }
    }

    
}
