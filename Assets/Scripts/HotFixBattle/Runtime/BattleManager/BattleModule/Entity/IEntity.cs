
using System;
using cfg;

namespace Game.Logic.BattleModule.Entity
{
    /// <summary>
    /// 实体接口，定义所有游戏实体的基本行为
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// 实体唯一ID
        /// </summary>
        int Id { get; }

        /// <summary>
        /// 实体名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 实体类型
        /// </summary>
        eEntityType Type { get; }

        /// <summary>
        /// 是否存活
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// 当前生命值
        /// </summary>
        int CurrentHealth { get; }

        /// <summary>
        /// 最大生命值
        /// </summary>
        int MaxHealth { get; }

        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="damage">伤害值</param>
        /// <returns>实际受到的伤害</returns>
        int TakeDamage(int damage);

        /// <summary>
        /// 治疗
        /// </summary>
        /// <param name="amount">治疗量</param>
        /// <returns>实际治疗量</returns>
        int Heal(int amount);

        /// <summary>
        /// 更新实体状态
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        void Update(float deltaTime);
    }
}
