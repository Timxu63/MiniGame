
using System;
using Game.Logic.BattleModule.Entity;

namespace HotFixBattle
{
    /// <summary>
    /// Buff基类
    /// </summary>
    public abstract class Buff
    {
        /// <summary>
        /// Buff的唯一ID
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Buff名称
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Buff描述
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// Buff图标ID
        /// </summary>
        public int IconId { get; protected set; }

        /// <summary>
        /// Buff持续时间（秒），0表示永久
        /// </summary>
        public float Duration { get; protected set; }

        /// <summary>
        /// Buff剩余时间（秒）
        /// </summary>
        public float RemainingTime { get; set; }

        /// <summary>
        /// Buff强度
        /// </summary>
        public float Intensity { get; protected set; }

        /// <summary>
        /// Buff是否生效
        /// </summary>
        public bool IsActive { get; protected set; }

        /// <summary>
        /// Buff所属实体
        /// </summary>
        public BaseEntity Owner { get; private set; }

        /// <summary>
        /// 静态ID计数器
        /// </summary>
        private static int _idCounter = 1;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="owner">Buff所属实体</param>
        /// <param name="name">Buff名称</param>
        /// <param name="description">Buff描述</param>
        /// <param name="iconId">Buff图标ID</param>
        /// <param name="duration">Buff持续时间（秒）</param>
        /// <param name="intensity">Buff强度</param>
        protected Buff(BaseEntity owner, string name, string description, int iconId, float duration, float intensity = 1.0f)
        {
            Id = _idCounter++;
            Owner = owner;
            Name = name;
            Description = description;
            IconId = iconId;
            Duration = duration;
            RemainingTime = duration;
            Intensity = intensity;
            IsActive = false;
        }

        /// <summary>
        /// 激活Buff
        /// </summary>
        public virtual void Activate()
        {
            if (IsActive) return;

            IsActive = true;
            OnActivate();
        }

        /// <summary>
        /// 更新Buff
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public virtual void Update(float deltaTime)
        {
            if (!IsActive) return;

            // 更新剩余时间
            if (Duration > 0)
            {
                RemainingTime -= deltaTime;

                // 如果时间耗尽，停用Buff
                if (RemainingTime <= 0)
                {
                    Deactivate();
                    return;
                }
            }

            // 调用更新逻辑
            OnUpdate(deltaTime);
        }

        /// <summary>
        /// 停用Buff
        /// </summary>
        public virtual void Deactivate()
        {
            if (!IsActive) return;

            IsActive = false;
            OnDeactivate();
        }

        /// <summary>
        /// 刷新Buff持续时间
        /// </summary>
        /// <param name="newDuration">新的持续时间</param>
        public virtual void Refresh(float newDuration = -1)
        {
            if (newDuration > 0)
            {
                Duration = newDuration;
            }

            RemainingTime = Duration;

            if (!IsActive)
            {
                Activate();
            }
        }

        /// <summary>
        /// Buff激活时的逻辑
        /// </summary>
        protected abstract void OnActivate();

        /// <summary>
        /// Buff更新时的逻辑
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        protected abstract void OnUpdate(float deltaTime);

        /// <summary>
        /// Buff停用时的逻辑
        /// </summary>
        protected abstract void OnDeactivate();
    }
}
