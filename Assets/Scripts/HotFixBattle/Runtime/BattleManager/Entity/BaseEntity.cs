
using System;
using Framework.EventSystem;
using Framework.Runtime;
using cfg;
using HotFixBattle;
using UnityEngine;

namespace Game.Logic.BattleModule.Entity
{
    /// <summary>
    /// 基础实体抽象类，实现IEntity接口
    /// </summary>
    public abstract class BaseEntity : IEntity
    {
        private static int _nextId = 1;

        /// <summary>
        /// 实体唯一ID
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// 实体名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 实体类型
        /// </summary>
        public eEntityType Type { get; protected set; }

        /// <summary>
        /// 是否存活
        /// </summary>
        public bool IsAlive { get; protected set; }

        /// <summary>
        /// 当前生命值
        /// </summary>
        public int CurrentHealth { get; protected set; }

        /// <summary>
        /// 最大生命值
        /// </summary>
        public int MaxHealth { get; protected set; }

        /// <summary>
        /// 角色数据
        /// </summary>
        public Charactor Charactor{ get; protected set; }

        /// <summary>
        /// 实体位置
        /// </summary>
        public Vector3 LocalPosition { get; set; }
        
        
        /// <summary>
        /// 移动速度
        /// </summary>
        public float MoveSpeed { get; set; } = 5.0f;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">实体名称</param>
        /// <param name="type">实体类型</param>
        /// <param name="maxHealth">最大生命值</param>
        protected BaseEntity(string name, eEntityType type, int maxHealth)
        {
            Id = _nextId++;
            Name = name;
            Type = type;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            IsAlive = true;
            LocalPosition = Vector3.zero; // 设置默认位置为 (0,0,0)
        }

        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="damage">伤害值</param>
        /// <returns>实际受到的伤害</returns>
        public virtual int TakeDamage(int damage)
        {
            if (!IsAlive || damage <= 0)
            {
                return 0;
            }

            int actualDamage = Math.Min(damage, CurrentHealth);
            CurrentHealth -= actualDamage;

            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                IsAlive = false;
                OnDeath();
                
                // 发送实体死亡事件
                GameApp.Event.DispatchNow((int)LocalMessageName.CC_EntityDeath, new EntityDeathEventArgs(Id));
            }

            OnTakeDamage(actualDamage);
            
            // 发送实体受伤事件
            GameApp.Event.DispatchNow((int)LocalMessageName.CC_EntityDamaged, new EntityDamagedEventArgs(Id, actualDamage));
            
            return actualDamage;
        }

        /// <summary>
        /// 治疗
        /// </summary>
        /// <param name="amount">治疗量</param>
        /// <returns>实际治疗量</returns>
        public virtual int Heal(int amount)
        {
            if (!IsAlive || amount <= 0)
            {
                return 0;
            }

            int actualHeal = Math.Min(amount, MaxHealth - CurrentHealth);
            CurrentHealth += actualHeal;

            OnHeal(actualHeal);
            
            // 发送实体治疗事件
            GameApp.Event.DispatchNow((int)LocalMessageName.CC_EntityHealed, new EntityHealedEventArgs(Id, actualHeal));
            
            return actualHeal;
        }

        /// <summary>
        /// 更新实体状态
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public virtual void Update(float deltaTime)
        {
            // 基础更新逻辑
        }

        /// <summary>
        /// 死亡事件
        /// </summary>
        protected virtual void OnDeath()
        {
            // 死亡事件处理
        }

        /// <summary>
        /// 受到伤害事件
        /// </summary>
        /// <param name="damage">实际受到的伤害</param>
        protected virtual void OnTakeDamage(int damage)
        {
            // 受伤事件处理
        }

        /// <summary>
        /// 治疗事件
        /// </summary>
        /// <param name="amount">实际治疗量</param>
        protected virtual void OnHeal(int amount)
        {
            // 治疗事件处理
        }
        
        
        /// <summary>
        /// 移动实体
        /// </summary>
        /// <param name="direction">移动方向</param>
        public void Move(Vector2 direction)
        {
            if (!IsAlive || direction == Vector2.zero)
                return;

            // 将2D方向转换为3D方向
            Vector3 moveDirection = new Vector3(direction.x, 0, direction.y).normalized;

            // 计算下一帧位置
            Vector3 nextPosition = LocalPosition + moveDirection * MoveSpeed * Time.deltaTime;

            // 检查地图边界
            if (MapManager.Instance.IsInitialized)
            {
                if (MapManager.Instance.IsInBounds(nextPosition))
                {
                    // 如果在边界内，直接移动
                    LocalPosition = nextPosition;
                }
                else
                {
                    // 如果超出边界，找到边界上的极限位置
                    Vector3 clampedPosition = nextPosition;
                    Bounds mapBounds = MapManager.Instance.MapBounds;

                    // 将位置限制在边界内
                    clampedPosition.x = Mathf.Clamp(clampedPosition.x, mapBounds.min.x, mapBounds.max.x);
                    clampedPosition.z = Mathf.Clamp(clampedPosition.z, mapBounds.min.z, mapBounds.max.z);

                    // 只有当夹紧后的位置与当前位置不同时才更新
                    if (clampedPosition != LocalPosition)
                    {
                        LocalPosition = clampedPosition;
                    }
                }

                // 发送实体移动事件
                GameApp.Event.DispatchNow((int)LocalMessageName.CC_EntityMove, new EntityMoveEventArgs(Id, LocalPosition));
            }
            else
            {
                // 如果地图未初始化，则直接移动
                LocalPosition = nextPosition;
                GameApp.Event.DispatchNow((int)LocalMessageName.CC_EntityMove, new EntityMoveEventArgs(Id, LocalPosition));
            }
        }
    }
}
