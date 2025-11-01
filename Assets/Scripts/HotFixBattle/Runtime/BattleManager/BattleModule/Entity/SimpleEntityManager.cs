using System;
using System.Collections.Generic;
using System.Linq;
using cfg;

namespace Game.Logic.BattleModule.Entity
{
    /// <summary>
    /// 简单实体管理器，用于管理游戏中的所有实体
    /// </summary>
    public class SimpleEntityManager
    {
        private static SimpleEntityManager _instance;
        private readonly Dictionary<int, IEntity> _entities;
        private readonly Dictionary<eEntityType, List<IEntity>> _entitiesByType;
        private int _nextId = 1;

        /// <summary>
        /// 获取实体管理器实例
        /// </summary>
        public static SimpleEntityManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SimpleEntityManager();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 私有构造函数，实现单例模式
        /// </summary>
        private SimpleEntityManager()
        {
            _entities = new Dictionary<int, IEntity>();
            _entitiesByType = new Dictionary<eEntityType, List<IEntity>>();

            // 初始化各类型实体列表
            foreach (eEntityType type in Enum.GetValues(typeof(eEntityType)))
            {
                _entitiesByType[type] = new List<IEntity>();
            }
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">要添加的实体</param>
        public void AddEntity(IEntity entity)
        {
            if (entity == null)
            {
                return;
            }

            _entities[entity.Id] = entity;
            _entitiesByType[entity.Type].Add(entity);
        }

        /// <summary>
        /// 移除实体
        /// </summary>
        /// <param name="entityId">实体ID</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveEntity(int entityId)
        {
            if (!_entities.TryGetValue(entityId, out var entity))
            {
                return false;
            }

            _entities.Remove(entityId);
            _entitiesByType[entity.Type].Remove(entity);
            return true;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="entityId">实体ID</param>
        /// <returns>实体对象，如果不存在则返回null</returns>
        public IEntity GetEntity(int entityId)
        {
            _entities.TryGetValue(entityId, out var entity);
            return entity;
        }

        /// <summary>
        /// 获取指定类型的所有实体
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns>实体列表</returns>
        public List<IEntity> GetEntitiesByType(eEntityType type)
        {
            return new List<IEntity>(_entitiesByType[type]);
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <returns>所有实体的列表</returns>
        public List<IEntity> GetAllEntities()
        {
            return new List<IEntity>(_entities.Values);
        }

        /// <summary>
        /// 更新所有实体
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public void UpdateAllEntities(float deltaTime)
        {
            foreach (var entity in _entities.Values.ToList())
            {
                entity.Update(deltaTime);
            }
        }

        /// <summary>
        /// 清除所有实体
        /// </summary>
        public void ClearAllEntities()
        {
            _entities.Clear();

            foreach (var typeList in _entitiesByType.Values)
            {
                typeList.Clear();
            }
        }
    }
}
