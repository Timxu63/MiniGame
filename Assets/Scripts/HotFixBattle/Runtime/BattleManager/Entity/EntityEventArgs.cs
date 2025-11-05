using Framework.EventSystem;
using Game.Logic.BattleModule.Entity;

namespace HotFixBattle
{
    /// <summary>
    /// 实体创建事件参数
    /// </summary>
    public class EntityCreatedEventArgs : BaseEventArgs
    {
        public IEntity Entity { get; }

        public EntityCreatedEventArgs(IEntity entity)
        {
            Entity = entity;
        }

        public override void Clear()
        {
            
        }
    }

    /// <summary>
    /// 实体销毁事件参数
    /// </summary>
    public class EntityDestroyedEventArgs : BaseEventArgs
    {
        public int EntityId { get; }

        public EntityDestroyedEventArgs(int entityId)
        {
            EntityId = entityId;
        }

        public override void Clear()
        {
            
        }
    }

    /// <summary>
    /// 实体受伤事件参数
    /// </summary>
    public class EntityDamagedEventArgs : BaseEventArgs
    {
        public int EntityId { get; }
        public int Damage { get; }

        public EntityDamagedEventArgs(int entityId, int damage)
        {
            EntityId = entityId;
            Damage = damage;
        }

        public override void Clear()
        {
            
        }
    }

    /// <summary>
    /// 实体治疗事件参数
    /// </summary>
    public class EntityHealedEventArgs : BaseEventArgs
    {
        public int EntityId { get; }
        public int Amount { get; }

        public EntityHealedEventArgs(int entityId, int amount)
        {
            EntityId = entityId;
            Amount = amount;
        }

        public override void Clear()
        {
            
        }
    }

    /// <summary>
    /// 实体死亡事件参数
    /// </summary>
    public class EntityDeathEventArgs : BaseEventArgs
    {
        public int EntityId { get; }

        public EntityDeathEventArgs(int entityId)
        {
            EntityId = entityId;
        }

        public override void Clear()
        {
            
        }
    }
}
