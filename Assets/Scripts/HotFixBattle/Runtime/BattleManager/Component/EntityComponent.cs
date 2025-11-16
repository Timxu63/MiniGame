
using System;
using Game.Logic.BattleModule.Entity;

namespace Game.Logic.BattleModule.Component
{
    /// <summary>
    /// 实体组件基类
    /// </summary>
    public abstract class EntityComponent
    {
        /// <summary>
        /// 组件所属的实体
        /// </summary>
        public BaseEntity Owner { get; private set; }

        /// <summary>
        /// 组件是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="owner">组件所属的实体</param>
        protected EntityComponent(BaseEntity owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        public virtual void Initialize()
        {
            // 子类可重写此方法进行初始化
        }

        /// <summary>
        /// 更新组件
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public virtual void Update(float deltaTime)
        {
            if (!IsEnabled) return;

            // 子类可重写此方法进行更新
        }

        /// <summary>
        /// 销毁组件
        /// </summary>
        public virtual void Destroy()
        {
            // 子类可重写此方法进行清理
        }
    }
}
