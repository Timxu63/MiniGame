using System;

namespace HotFixBattle
{
    /// <summary>
    /// 实体状态扩展方法
    /// </summary>
    public static class EntityStateExtensions
    {
        /// <summary>
        /// 添加冰冻状态
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="duration">持续时间（秒）</param>
        /// <param name="intensity">强度</param>
        public static void Freeze(this Game.Logic.BattleModule.Entity.BaseEntity entity, float duration = 5f, float intensity = 1.0f)
        {
            entity.StateManager.AddState(EntityState.Frozen, duration, intensity);
        }

        /// <summary>
        /// 添加中毒状态
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="duration">持续时间（秒）</param>
        /// <param name="intensity">强度</param>
        public static void Poison(this Game.Logic.BattleModule.Entity.BaseEntity entity, float duration = 10f, float intensity = 1.0f)
        {
            entity.StateManager.AddState(EntityState.Poisoned, duration, intensity);
        }

        /// <summary>
        /// 添加燃烧状态
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="duration">持续时间（秒）</param>
        /// <param name="intensity">强度</param>
        public static void Burn(this Game.Logic.BattleModule.Entity.BaseEntity entity, float duration = 8f, float intensity = 1.0f)
        {
            entity.StateManager.AddState(EntityState.Burning, duration, intensity);
        }

        /// <summary>
        /// 添加流血状态
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="duration">持续时间（秒）</param>
        /// <param name="intensity">强度</param>
        public static void Bleed(this Game.Logic.BattleModule.Entity.BaseEntity entity, float duration = 6f, float intensity = 1.0f)
        {
            entity.StateManager.AddState(EntityState.Bleeding, duration, intensity);
        }

        /// <summary>
        /// 添加眩晕状态
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="duration">持续时间（秒）</param>
        /// <param name="intensity">强度</param>
        public static void Stun(this Game.Logic.BattleModule.Entity.BaseEntity entity, float duration = 3f, float intensity = 1.0f)
        {
            entity.StateManager.AddState(EntityState.Stunned, duration, intensity);
        }

        /// <summary>
        /// 添加沉默状态
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="duration">持续时间（秒）</param>
        /// <param name="intensity">强度</param>
        public static void Silence(this Game.Logic.BattleModule.Entity.BaseEntity entity, float duration = 5f, float intensity = 1.0f)
        {
            entity.StateManager.AddState(EntityState.Silenced, duration, intensity);
        }

        /// <summary>
        /// 添加无敌状态
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="duration">持续时间（秒）</param>
        /// <param name="intensity">强度</param>
        public static void MakeInvincible(this Game.Logic.BattleModule.Entity.BaseEntity entity, float duration = 5f, float intensity = 1.0f)
        {
            entity.StateManager.AddState(EntityState.Invincible, duration, intensity);
        }

        /// <summary>
        /// 添加隐身状态
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="duration">持续时间（秒）</param>
        /// <param name="intensity">强度</param>
        public static void MakeInvisible(this Game.Logic.BattleModule.Entity.BaseEntity entity, float duration = 5f, float intensity = 1.0f)
        {
            entity.StateManager.AddState(EntityState.Invisible, duration, intensity);
        }

        /// <summary>
        /// 移除指定状态
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="state">要移除的状态</param>
        public static void RemoveState(this Game.Logic.BattleModule.Entity.BaseEntity entity, EntityState state)
        {
            entity.StateManager.RemoveState(state);
        }

        /// <summary>
        /// 检查是否有指定状态
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="state">要检查的状态</param>
        /// <returns>是否有该状态</returns>
        public static bool HasState(this Game.Logic.BattleModule.Entity.BaseEntity entity, EntityState state)
        {
            return entity.StateManager.HasState(state);
        }
    }
}
