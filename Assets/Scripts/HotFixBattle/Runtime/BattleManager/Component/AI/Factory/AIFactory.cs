
using System;
using System.Collections.Generic;
using Game.Logic.BattleModule.Entity;

namespace Game.Logic.BattleModule.Component.AI
{
    /// <summary>
    /// AI工厂类，用于创建和管理AI组件
    /// </summary>
    public static class AIFactory
    {
        /// <summary>
        /// AI组件创建器委托
        /// </summary>
        /// <param name="owner">AI所属的实体</param>
        /// <returns>创建的AI组件</returns>
        public delegate AIComponent AICreatorDelegate(BaseEntity owner);

        /// <summary>
        /// AI组件创建器字典
        /// </summary>
        private static Dictionary<string, AICreatorDelegate> _creators = new Dictionary<string, AICreatorDelegate>();

        /// <summary>
        /// 静态构造函数，注册默认的AI创建器
        /// </summary>
        static AIFactory()
        {
            // 注册默认的AI创建器
            RegisterCreator("Monster", owner => new MonsterAI(owner as MonsterEntity));
            RegisterCreator("Default", owner => new MonsterAI(owner as MonsterEntity));
        }

        /// <summary>
        /// 注册AI组件创建器
        /// </summary>
        /// <param name="aiType">AI类型名称</param>
        /// <param name="creator">创建器委托</param>
        public static void RegisterCreator(string aiType, AICreatorDelegate creator)
        {
            if (!string.IsNullOrEmpty(aiType) && creator != null)
            {
                _creators[aiType] = creator;
            }
        }

        /// <summary>
        /// 创建AI组件
        /// </summary>
        /// <param name="aiType">AI类型名称</param>
        /// <param name="owner">AI所属的实体</param>
        /// <returns>创建的AI组件，如果创建失败则返回null</returns>
        public static AIComponent CreateAI(string aiType, BaseEntity owner)
        {
            if (string.IsNullOrEmpty(aiType) || owner == null)
            {
                return null;
            }

            if (_creators.ContainsKey(aiType))
            {
                try
                {
                    return _creators[aiType](owner);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"创建AI组件失败: {ex.Message}");
                    return null;
                }
            }

            // 如果找不到指定类型的创建器，尝试使用默认创建器
            if (_creators.ContainsKey("Default"))
            {
                try
                {
                    return _creators["Default"](owner);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"创建默认AI组件失败: {ex.Message}");
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// 为实体添加AI组件
        /// </summary>
        /// <param name="aiType">AI类型名称</param>
        /// <param name="owner">AI所属的实体</param>
        /// <returns>添加的AI组件，如果添加失败则返回null</returns>
        public static AIComponent AddAIToEntity(string aiType, BaseEntity owner)
        {
            if (owner == null)
            {
                return null;
            }

            // 检查是否已有AI组件
            var existingAI = owner.ComponentManager.GetComponent<AIComponent>();
            if (existingAI != null)
            {
                return existingAI;
            }

            // 创建新的AI组件
            var ai = CreateAI(aiType, owner);
            if (ai != null)
            {
                // 使用新的AddComponentInstance方法直接添加创建的AI实例
                return owner.ComponentManager.AddComponentInstance(ai);
            }

            return null;
        }

        /// <summary>
        /// 从实体移除AI组件
        /// </summary>
        /// <param name="owner">AI所属的实体</param>
        /// <returns>是否成功移除</returns>
        public static bool RemoveAIFromEntity(BaseEntity owner)
        {
            if (owner == null)
            {
                return false;
            }

            return owner.ComponentManager.RemoveComponent<AIComponent>();
        }

        /// <summary>
        /// 检查实体是否有AI组件
        /// </summary>
        /// <param name="owner">要检查的实体</param>
        /// <returns>如果实体有AI组件返回true，否则返回false</returns>
        public static bool EntityHasAI(BaseEntity owner)
        {
            if (owner == null)
            {
                return false;
            }

            return owner.ComponentManager.HasComponent<AIComponent>();
        }

        /// <summary>
        /// 获取实体的AI组件
        /// </summary>
        /// <param name="owner">要获取AI组件的实体</param>
        /// <returns>AI组件，如果实体没有AI组件则返回null</returns>
        public static AIComponent GetEntityAI(BaseEntity owner)
        {
            if (owner == null)
            {
                return null;
            }

            return owner.ComponentManager.GetComponent<AIComponent>();
        }
    }
}
