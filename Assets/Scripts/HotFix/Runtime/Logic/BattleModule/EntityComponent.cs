using UnityEngine;
using Game.Logic.BattleModule.Entity;

namespace HotFixBattle
{
    /// <summary>
    /// 实体组件，用于在视图和实体之间建立关联
    /// </summary>
    public class EntityComponent : MonoBehaviour
    {
        /// <summary>
        /// 关联的实体对象
        /// </summary>
        public IEntity Entity { get; set; }

        /// <summary>
        /// 获取实体组件
        /// </summary>
        /// <param name="gameObject">游戏对象</param>
        /// <returns>实体组件</returns>
        public static EntityComponent GetEntityComponent(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return null;
            }

            return gameObject.GetComponent<EntityComponent>();
        }

        /// <summary>
        /// 获取游戏对象的关联实体
        /// </summary>
        /// <param name="gameObject">游戏对象</param>
        /// <returns>关联的实体对象</returns>
        public static IEntity GetEntity(GameObject gameObject)
        {
            var component = GetEntityComponent(gameObject);
            return component?.Entity;
        }
    }
}
