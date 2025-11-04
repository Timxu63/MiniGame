
using Framework.EventSystem;
using UnityEngine;

namespace HotFixBattle
{
    /// <summary>
    /// 实体移动事件参数
    /// </summary>
    public class EntityMoveEventArgs : BaseEventArgs
    {
        public int EntityId { get; }
        public Vector3 Position { get; }

        public EntityMoveEventArgs(int entityId, Vector3 position)
        {
            EntityId = entityId;
            Position = position;
        }

        public override void Clear()
        {
            // 清理资源
        }
    }

    /// <summary>
    /// 实体攻击事件参数
    /// </summary>
    public class EntityAttackEventArgs : BaseEventArgs
    {
        public int AttackerId { get; }
        public int TargetId { get; }

        public EntityAttackEventArgs(int attackerId, int targetId)
        {
            AttackerId = attackerId;
            TargetId = targetId;
        }

        public override void Clear()
        {
            // 清理资源
        }
    }
}
