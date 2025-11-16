
using System;
using System.Collections.Generic;
using Game.Logic.BattleModule.Entity;

namespace Game.Logic.BattleModule.Component
{
    /// <summary>
    /// 组件管理器，用于管理实体的所有组件
    /// </summary>
    public class ComponentManager
    {
        /// <summary>
        /// 组件字典，键为组件类型，值为组件实例
        /// </summary>
        private Dictionary<Type, EntityComponent> _components = new Dictionary<Type, EntityComponent>();

        /// <summary>
        /// 组件所属的实体
        /// </summary>
        private readonly BaseEntity _owner;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="owner">组件所属的实体</param>
        public ComponentManager(BaseEntity owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>添加的组件实例</returns>
        public T AddComponent<T>() where T : EntityComponent, new()
        {
            Type componentType = typeof(T);

            // 如果已存在该类型的组件，则返回现有组件
            if (_components.ContainsKey(componentType))
            {
                return (T)_components[componentType];
            }

            // 创建新组件
            T component = (T)Activator.CreateInstance(componentType, _owner);
            _components[componentType] = component;

            // 初始化组件
            component.Initialize();

            return component;
        }

        /// <summary>
        /// 添加已有组件实例
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">组件实例</param>
        /// <returns>添加的组件实例</returns>
        public T AddComponentInstance<T>(T component) where T : EntityComponent
        {
            if (component == null)
            {
                return null;
            }

            Type componentType = typeof(T);

            // 如果已存在该类型的组件，则返回现有组件
            if (_components.ContainsKey(componentType))
            {
                return (T)_components[componentType];
            }

            // 添加组件
            _components[componentType] = component;

            // 初始化组件
            component.Initialize();

            return component;
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>组件实例，如果不存在则返回null</returns>
        public T GetComponent<T>() where T : EntityComponent
        {
            Type componentType = typeof(T);
            if (_components.ContainsKey(componentType))
            {
                return (T)_components[componentType];
            }

            return null;
        }

        /// <summary>
        /// 移除组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>是否成功移除组件</returns>
        public bool RemoveComponent<T>() where T : EntityComponent
        {
            Type componentType = typeof(T);
            if (_components.ContainsKey(componentType))
            {
                // 销毁组件
                _components[componentType].Destroy();

                // 从字典中移除
                _components.Remove(componentType);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检查是否存在指定类型的组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>是否存在该组件</returns>
        public bool HasComponent<T>() where T : EntityComponent
        {
            Type componentType = typeof(T);
            return _components.ContainsKey(componentType);
        }

        /// <summary>
        /// 更新所有组件
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public void UpdateAll(float deltaTime)
        {
            foreach (var component in _components.Values)
            {
                if (component.IsEnabled)
                {
                    component.Update(deltaTime);
                }
            }
        }

        /// <summary>
        /// 销毁所有组件
        /// </summary>
        public void DestroyAll()
        {
            foreach (var component in _components.Values)
            {
                component.Destroy();
            }

            _components.Clear();
        }
    }
}
