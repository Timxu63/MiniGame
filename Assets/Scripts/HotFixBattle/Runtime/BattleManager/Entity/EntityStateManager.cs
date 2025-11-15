using System;
using System.Collections.Generic;
using Game.Logic.BattleModule.Entity;

namespace HotFixBattle
{
    /// <summary>
    /// 实体状态管理器
    /// </summary>
    public class EntityStateManager
    {
        // 当前实体状态（使用位掩码）
        private EntityState _currentState = EntityState.Normal;

        // 状态效果字典，存储每个状态的持续时间
        private Dictionary<EntityState, float> _stateDurations = new Dictionary<EntityState, float>();

        // 状态效果字典，存储每个状态的强度
        private Dictionary<EntityState, float> _stateIntensities = new Dictionary<EntityState, float>();

        // 状态回调字典，存储状态开始、更新和结束时的回调
        private Dictionary<EntityState, Action<float>> _stateStartCallbacks = new Dictionary<EntityState, Action<float>>();
        private Dictionary<EntityState, Action<float>> _stateUpdateCallbacks = new Dictionary<EntityState, Action<float>>();
        private Dictionary<EntityState, Action> _stateEndCallbacks = new Dictionary<EntityState, Action>();

        // 实体引用
        private readonly BaseEntity _entity;

        /// <summary>
        /// 当前状态
        /// </summary>
        public EntityState CurrentState => _currentState;

        /// <summary>
        /// 是否死亡
        /// </summary>
        public bool IsDead => HasState(EntityState.Dead);

        /// <summary>
        /// 是否冰冻
        /// </summary>
        public bool IsFrozen => HasState(EntityState.Frozen);

        /// <summary>
        /// 是否眩晕
        /// </summary>
        public bool IsStunned => HasState(EntityState.Stunned);

        /// <summary>
        /// 是否无敌
        /// </summary>
        public bool IsInvincible => HasState(EntityState.Invincible);

        /// <summary>
        /// 是否可以移动
        /// </summary>
        public bool CanMove => !IsDead && !IsFrozen && !IsStunned;

        /// <summary>
        /// 是否可以攻击
        /// </summary>
        public bool CanAttack => !IsDead && !IsFrozen && !IsStunned && !HasState(EntityState.Silenced);

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="entity">实体引用</param>
        public EntityStateManager(BaseEntity entity)
        {
            _entity = entity;

            // 初始化状态回调
            InitializeStateCallbacks();
        }

        /// <summary>
        /// 初始化状态回调
        /// </summary>
        private void InitializeStateCallbacks()
        {
            // 冰冻状态回调
            _stateStartCallbacks[EntityState.Frozen] = (intensity) => {
                // 冰冻开始时的逻辑，例如播放冰冻动画
                UnityEngine.Debug.Log($"[{_entity.Name}] 被冰冻了，强度: {intensity}");
            };

            _stateUpdateCallbacks[EntityState.Frozen] = (intensity) => {
                // 冰冻持续期间的逻辑，例如持续伤害
            };

            _stateEndCallbacks[EntityState.Frozen] = () => {
                // 冰冻结束时的逻辑，例如停止冰冻动画
                UnityEngine.Debug.Log($"[{_entity.Name}] 冰冻状态结束");
            };

            // 中毒状态回调
            _stateStartCallbacks[EntityState.Poisoned] = (intensity) => {
                UnityEngine.Debug.Log($"[{_entity.Name}] 中毒了，强度: {intensity}");
            };

            _stateUpdateCallbacks[EntityState.Poisoned] = (intensity) => {
                // 中毒持续伤害
                if (_entity.IsAlive)
                {
                    int damage = (int)(intensity * UnityEngine.Time.deltaTime);
                    _entity.TakeDamage(damage);
                }
            };

            _stateEndCallbacks[EntityState.Poisoned] = () => {
                UnityEngine.Debug.Log($"[{_entity.Name}] 中毒状态结束");
            };

            // 燃烧状态回调
            _stateStartCallbacks[EntityState.Burning] = (intensity) => {
                UnityEngine.Debug.Log($"[{_entity.Name}] 燃烧了，强度: {intensity}");
            };

            _stateUpdateCallbacks[EntityState.Burning] = (intensity) => {
                // 燃烧持续伤害
                if (_entity.IsAlive)
                {
                    int damage = (int)(intensity * UnityEngine.Time.deltaTime);
                    _entity.TakeDamage(damage);
                }
            };

            _stateEndCallbacks[EntityState.Burning] = () => {
                UnityEngine.Debug.Log($"[{_entity.Name}] 燃烧状态结束");
            };

            // 流血状态回调
            _stateStartCallbacks[EntityState.Bleeding] = (intensity) => {
                UnityEngine.Debug.Log($"[{_entity.Name}] 流血了，强度: {intensity}");
            };

            _stateUpdateCallbacks[EntityState.Bleeding] = (intensity) => {
                // 流血持续伤害
                if (_entity.IsAlive)
                {
                    int damage = (int)(intensity * UnityEngine.Time.deltaTime);
                    _entity.TakeDamage(damage);
                }
            };

            _stateEndCallbacks[EntityState.Bleeding] = () => {
                UnityEngine.Debug.Log($"[{_entity.Name}] 流血状态结束");
            };

            // 眩晕状态回调
            _stateStartCallbacks[EntityState.Stunned] = (intensity) => {
                UnityEngine.Debug.Log($"[{_entity.Name}] 眩晕了，强度: {intensity}");
            };

            _stateEndCallbacks[EntityState.Stunned] = () => {
                UnityEngine.Debug.Log($"[{_entity.Name}] 眩晕状态结束");
            };

            // 死亡状态回调
            _stateStartCallbacks[EntityState.Dead] = (intensity) => {
                UnityEngine.Debug.Log($"[{_entity.Name}] 死亡了");
                // 移除所有其他状态
                RemoveAllStatesExcept(EntityState.Dead);
            };
        }

        /// <summary>
        /// 更新状态管理器
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public void Update(float deltaTime)
        {
            // 创建一个列表来存储需要移除的状态
            List<EntityState> statesToRemove = new List<EntityState>();

            // 遍历所有状态
            foreach (var kvp in _stateDurations)
            {
                EntityState state = kvp.Key;
                float remainingTime = kvp.Value - deltaTime;

                // 更新剩余时间
                _stateDurations[state] = remainingTime;

                // 调用状态更新回调
                if (_stateUpdateCallbacks.ContainsKey(state))
                {
                    float intensity = _stateIntensities.ContainsKey(state) ? _stateIntensities[state] : 1.0f;
                    _stateUpdateCallbacks[state]?.Invoke(intensity);
                }

                // 检查状态是否结束
                if (remainingTime <= 0)
                {
                    statesToRemove.Add(state);
                }
            }

            // 移除已结束的状态
            foreach (var state in statesToRemove)
            {
                RemoveState(state);
            }
        }

        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="state">要添加的状态</param>
        /// <param name="duration">持续时间（秒），0表示永久</param>
        /// <param name="intensity">状态强度</param>
        public void AddState(EntityState state, float duration = 0f, float intensity = 1.0f)
        {
            // 如果实体已死亡，只能添加死亡状态
            if (IsDead && state != EntityState.Dead)
            {
                return;
            }

            // 如果已经是该状态，更新持续时间
            if (HasState(state))
            {
                // 如果新的持续时间更长，则更新
                if (duration > 0 && (!_stateDurations.ContainsKey(state) || _stateDurations[state] < duration))
                {
                    _stateDurations[state] = duration;
                }

                // 更新强度
                _stateIntensities[state] = intensity;
                return;
            }

            // 添加状态
            _currentState |= state;

            // 设置持续时间和强度
            if (duration > 0)
            {
                _stateDurations[state] = duration;
            }

            _stateIntensities[state] = intensity;

            // 调用状态开始回调
            if (_stateStartCallbacks.ContainsKey(state))
            {
                _stateStartCallbacks[state]?.Invoke(intensity);
            }
        }

        /// <summary>
        /// 移除状态
        /// </summary>
        /// <param name="state">要移除的状态</param>
        public void RemoveState(EntityState state)
        {
            // 如果没有该状态，直接返回
            if (!HasState(state))
            {
                return;
            }

            // 移除状态
            _currentState &= ~state;

            // 移除持续时间和强度
            _stateDurations.Remove(state);
            _stateIntensities.Remove(state);

            // 调用状态结束回调
            if (_stateEndCallbacks.ContainsKey(state))
            {
                _stateEndCallbacks[state]?.Invoke();
            }
        }

        /// <summary>
        /// 检查是否有指定状态
        /// </summary>
        /// <param name="state">要检查的状态</param>
        /// <returns>是否有该状态</returns>
        public bool HasState(EntityState state)
        {
            return (_currentState & state) == state;
        }

        /// <summary>
        /// 移除所有状态
        /// </summary>
        public void RemoveAllStates()
        {
            // 创建一个列表来存储当前所有状态
            List<EntityState> currentStates = new List<EntityState>();

            // 遍历所有可能的状态
            foreach (EntityState state in Enum.GetValues(typeof(EntityState)))
            {
                if (state != EntityState.None && HasState(state))
                {
                    currentStates.Add(state);
                }
            }

            // 移除所有状态
            foreach (var state in currentStates)
            {
                RemoveState(state);
            }
        }

        /// <summary>
        /// 移除除指定状态外的所有状态
        /// </summary>
        /// <param name="stateToKeep">要保留的状态</param>
        public void RemoveAllStatesExcept(EntityState stateToKeep)
        {
            // 创建一个列表来存储当前所有状态
            List<EntityState> currentStates = new List<EntityState>();

            // 遍历所有可能的状态
            foreach (EntityState state in Enum.GetValues(typeof(EntityState)))
            {
                if (state != EntityState.None && state != stateToKeep && HasState(state))
                {
                    currentStates.Add(state);
                }
            }

            // 移除所有状态
            foreach (var state in currentStates)
            {
                RemoveState(state);
            }
        }

        /// <summary>
        /// 获取状态剩余时间
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>剩余时间（秒），-1表示无时间限制</returns>
        public float GetStateRemainingTime(EntityState state)
        {
            if (!HasState(state))
            {
                return 0f;
            }

            return _stateDurations.ContainsKey(state) ? _stateDurations[state] : -1f;
        }

        /// <summary>
        /// 获取状态强度
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>状态强度</returns>
        public float GetStateIntensity(EntityState state)
        {
            if (!HasState(state))
            {
                return 0f;
            }

            return _stateIntensities.ContainsKey(state) ? _stateIntensities[state] : 1.0f;
        }
    }
}
