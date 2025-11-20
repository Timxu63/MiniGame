using System;
using System.Collections.Generic;
using Game.Logic.BattleModule.Entity;

namespace HotFixBattle.AI
{
    /// <summary>
    /// AI行为组合器，用于组合多个行为
    /// </summary>
    public abstract class AIBehaviorComposite : AIBehaviorBase
    {
        /// <summary>
        /// 子行为列表
        /// </summary>
        protected List<IAIBehavior> Children { get; } = new List<IAIBehavior>();

        /// <summary>
        /// 添加子行为
        /// </summary>
        /// <param name="behavior">子行为</param>
        public void AddChild(IAIBehavior behavior)
        {
            if (behavior != null)
            {
                Children.Add(behavior);
            }
        }

        /// <summary>
        /// 清除所有子行为
        /// </summary>
        public void ClearChildren()
        {
            Children.Clear();
        }
    }
}
