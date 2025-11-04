
using UnityEngine;

namespace HotFixBattle.AI
{
    /// <summary>
    /// AI扩展方法
    /// </summary>
    public static class AIExtensions
    {
        /// <summary>
        /// 获取实体在屏幕上的位置
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>屏幕位置</returns>
        public static Vector3 GetScreenPosition(this Game.Logic.BattleModule.Entity.IEntity entity)
        {
            // 如果实体位置为Vector3.zero，返回屏幕最上方的点
            if (entity.Position == Vector3.zero)
            {
                // 获取屏幕高度
                float screenHeight = Camera.main.orthographicSize * 2;
                // 返回屏幕最上方的点
                return new Vector3(0, screenHeight / 2, 0);
            }

            // 否则返回实体当前位置
            return entity.Position;
        }

        /// <summary>
        /// 设置实体位置
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="position">新位置</param>
        public static void SetPosition(this Game.Logic.BattleModule.Entity.IEntity entity, Vector3 position)
        {
            entity.Position = position;
        }
    }
}
