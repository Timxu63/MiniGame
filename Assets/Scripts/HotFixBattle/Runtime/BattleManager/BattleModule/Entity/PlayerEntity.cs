
using System;
using cfg;

namespace Game.Logic.BattleModule.Entity
{
    /// <summary>
    /// 玩家实体类
    /// </summary>
    public class PlayerEntity : BaseEntity
    {
        /// <summary>
        /// 玩家等级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 玩家经验值
        /// </summary>
        public int Experience { get; set; }

        /// <summary>
        /// 玩家攻击力
        /// </summary>
        public int AttackPower { get; set; }

        /// <summary>
        /// 玩家防御力
        /// </summary>
        public int Defense { get; set; }

        /// <summary>
        /// 玩家技能点
        /// </summary>
        public int SkillPoints { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">玩家名称</param>
        /// <param name="maxHealth">最大生命值</param>
        /// <param name="level">玩家等级</param>
        /// <param name="attackPower">攻击力</param>
        /// <param name="defense">防御力</param>
        public PlayerEntity(string name, int maxHealth, int level = 1, int attackPower = 10, int defense = 5)
            : base(name, eEntityType.Player, maxHealth)
        {
            Level = level;
            Experience = 0;
            AttackPower = attackPower;
            Defense = defense;
            SkillPoints = 0;
        }

        /// <summary>
        /// 获得经验值
        /// </summary>
        /// <param name="exp">经验值</param>
        public void GainExperience(int exp)
        {
            Experience += exp;
            // 简单的升级逻辑，每100点经验升一级
            int levelsGained = Experience / 100;
            if (levelsGained > 0)
            {
                LevelUp(levelsGained);
                Experience %= 100;
            }
        }

        /// <summary>
        /// 升级
        /// </summary>
        /// <param name="levels">升级数</param>
        private void LevelUp(int levels)
        {
            Level += levels;
            // 升级增加属性
            MaxHealth += levels * 10;
            CurrentHealth += levels * 10;
            AttackPower += levels * 2;
            Defense += levels;
            SkillPoints += levels;
        }

        /// <summary>
        /// 重写受到伤害方法，考虑防御力
        /// </summary>
        /// <param name="damage">原始伤害值</param>
        /// <returns>实际受到的伤害</returns>
        public override int TakeDamage(int damage)
        {
            // 计算实际伤害，考虑防御力
            int actualDamage = Math.Max(1, damage - Defense);
            return base.TakeDamage(actualDamage);
        }

        /// <summary>
        /// 重写死亡事件
        /// </summary>
        protected override void OnDeath()
        {
            base.OnDeath();
            // 玩家死亡的特殊处理
            // 例如：掉落经验值、返回复活点等
        }
    }
}
