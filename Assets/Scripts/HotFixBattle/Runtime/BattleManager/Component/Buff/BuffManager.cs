
using System;
using System.Collections.Generic;
using Game.Logic.BattleModule.Component;
using Game.Logic.BattleModule.Entity;

namespace HotFixBattle
{
    /// <summary>
    /// Buff管理器组件
    /// </summary>
    public class BuffManager : EntityComponent
    {
        /// <summary>
        /// Buff字典，键为Buff类型，值为Buff实例列表
        /// </summary>
        private Dictionary<Type, List<Buff>> _buffsByType = new Dictionary<Type, List<Buff>>();

        /// <summary>
        /// 所有Buff的列表
        /// </summary>
        private List<Buff> _allBuffs = new List<Buff>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="owner">组件所属的实体</param>
        public BuffManager(BaseEntity owner) : base(owner)
        {
        }

        /// <summary>
        /// 添加Buff
        /// </summary>
        /// <typeparam name="T">Buff类型</typeparam>
        /// <param name="duration">持续时间</param>
        /// <param name="intensity">强度</param>
        /// <returns>添加的Buff实例</returns>
        public T AddBuff<T>(float duration = 0f, float intensity = 1.0f) where T : Buff
        {
            Type buffType = typeof(T);

            // 创建Buff实例
            T buff = (T)Activator.CreateInstance(buffType, Owner, duration, intensity);

            // 添加到字典
            if (!_buffsByType.ContainsKey(buffType))
            {
                _buffsByType[buffType] = new List<Buff>();
            }
            _buffsByType[buffType].Add(buff);

            // 添加到总列表
            _allBuffs.Add(buff);

            // 激活Buff
            buff.Activate();

            return buff;
        }

        /// <summary>
        /// 移除指定类型的所有Buff
        /// </summary>
        /// <typeparam name="T">Buff类型</typeparam>
        /// <returns>是否成功移除Buff</returns>
        public bool RemoveBuff<T>() where T : Buff
        {
            Type buffType = typeof(T);

            if (!_buffsByType.ContainsKey(buffType))
            {
                return false;
            }

            // 获取该类型的所有Buff
            List<Buff> buffsToRemove = _buffsByType[buffType];

            // 停用所有Buff
            foreach (var buff in buffsToRemove)
            {
                buff.Deactivate();
                _allBuffs.Remove(buff);
            }

            // 从字典中移除
            _buffsByType.Remove(buffType);

            return true;
        }

        /// <summary>
        /// 移除指定ID的Buff
        /// </summary>
        /// <param name="buffId">Buff ID</param>
        /// <returns>是否成功移除Buff</returns>
        public bool RemoveBuff(int buffId)
        {
            // 查找Buff
            Buff buffToRemove = null;
            foreach (var buff in _allBuffs)
            {
                if (buff.Id == buffId)
                {
                    buffToRemove = buff;
                    break;
                }
            }

            if (buffToRemove == null)
            {
                return false;
            }

            // 停用Buff
            buffToRemove.Deactivate();

            // 从字典中移除
            Type buffType = buffToRemove.GetType();
            if (_buffsByType.ContainsKey(buffType))
            {
                _buffsByType[buffType].Remove(buffToRemove);

                // 如果该类型没有其他Buff，则从字典中移除该类型
                if (_buffsByType[buffType].Count == 0)
                {
                    _buffsByType.Remove(buffType);
                }
            }

            // 从总列表中移除
            _allBuffs.Remove(buffToRemove);

            return true;
        }

        /// <summary>
        /// 检查是否存在指定类型的Buff
        /// </summary>
        /// <typeparam name="T">Buff类型</typeparam>
        /// <returns>是否存在该类型的Buff</returns>
        public bool HasBuff<T>() where T : Buff
        {
            Type buffType = typeof(T);
            return _buffsByType.ContainsKey(buffType) && _buffsByType[buffType].Count > 0;
        }

        /// <summary>
        /// 获取指定类型的第一个Buff
        /// </summary>
        /// <typeparam name="T">Buff类型</typeparam>
        /// <returns>Buff实例，如果不存在则返回null</returns>
        public T GetBuff<T>() where T : Buff
        {
            Type buffType = typeof(T);

            if (!_buffsByType.ContainsKey(buffType) || _buffsByType[buffType].Count == 0)
            {
                return null;
            }

            return (T)_buffsByType[buffType][0];
        }

        /// <summary>
        /// 获取指定类型的所有Buff
        /// </summary>
        /// <typeparam name="T">Buff类型</typeparam>
        /// <returns>Buff实例列表</returns>
        public List<T> GetBuffs<T>() where T : Buff
        {
            Type buffType = typeof(T);
            List<T> result = new List<T>();

            if (_buffsByType.ContainsKey(buffType))
            {
                foreach (var buff in _buffsByType[buffType])
                {
                    result.Add((T)buff);
                }
            }

            return result;
        }

        /// <summary>
        /// 获取所有Buff
        /// </summary>
        /// <returns>所有Buff的列表</returns>
        public List<Buff> GetAllBuffs()
        {
            return new List<Buff>(_allBuffs);
        }

        /// <summary>
        /// 更新所有Buff
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public override void Update(float deltaTime)
        {
            if (!IsEnabled) return;

            // 创建一个列表来存储需要移除的Buff
            List<Buff> buffsToRemove = new List<Buff>();

            // 更新所有Buff
            foreach (var buff in _allBuffs)
            {
                buff.Update(deltaTime);

                // 如果Buff已停用且时间耗尽，则标记为需要移除
                if (!buff.IsActive && buff.Duration > 0 && buff.RemainingTime <= 0)
                {
                    buffsToRemove.Add(buff);
                }
            }

            // 移除已结束的Buff
            foreach (var buff in buffsToRemove)
            {
                RemoveBuff(buff.Id);
            }
        }

        /// <summary>
        /// 清除所有Buff
        /// </summary>
        public void ClearAllBuffs()
        {
            // 停用所有Buff
            foreach (var buff in _allBuffs)
            {
                buff.Deactivate();
            }

            // 清空字典和列表
            _buffsByType.Clear();
            _allBuffs.Clear();
        }
    }
}
