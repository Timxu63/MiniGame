using System;
using System.Collections.Generic;
using cfg;
using Game.Logic.BattleModule.Entity;

namespace HotFixBattle.AI
{
    /// <summary>
    /// AI行为组加载器，用于加载和管理AI行为组
    /// </summary>
    public static partial class AIBehaviorGroupLoader
    {
        /// <summary>
        /// AI行为组字典，根据名称存储行为组
        /// </summary>
        private static readonly Dictionary<string, AIBehaviorGroup> _behaviorGroups = 
            new Dictionary<string, AIBehaviorGroup>();

        /// <summary>
        /// 初始化默认AI行为组
        /// </summary>
        static AIBehaviorGroupLoader()
        {
            InitializeDefaultBehaviorGroups();
        }

        /// <summary>
        /// 获取AI行为组
        /// </summary>
        /// <param name="groupName">行为组名称</param>
        /// <returns>AI行为组，如果不存在则返回null</returns>
        public static AIBehaviorGroup GetBehaviorGroup(string groupName)
        {
            _behaviorGroups.TryGetValue(groupName, out var group);
            return group;
        }

        /// <summary>
        /// 注册AI行为组
        /// </summary>
        /// <param name="group">AI行为组</param>
        public static void RegisterBehaviorGroup(AIBehaviorGroup group)
        {
            if (group != null && !string.IsNullOrEmpty(group.Name))
            {
                _behaviorGroups[group.Name] = group;
            }
        }

        /// <summary>
        /// 从Charactor配置中初始化AI
        /// </summary>
        /// <param name="ai">AI组件</param>
        /// <param name="charactor">角色配置</param>
        public static void InitializeAI(AIComponent ai, Charactor charactor)
        {
            // 从Charactor配置中获取行为组名称
            string behaviorGroupName = charactor.AIName;

            if (string.IsNullOrEmpty(behaviorGroupName))
            {
                // 如果没有指定行为组，则根据怪物类型使用默认行为组
                behaviorGroupName = GetDefaultBehaviorGroupName(charactor.Type);
            }

            // 获取行为组
            var behaviorGroup = GetBehaviorGroup(behaviorGroupName);

            if (behaviorGroup != null)
            {
                // 使用行为组初始化AI
                behaviorGroup.Initialize(ai, charactor);
            }
            else
            {
                // 如果找不到指定的行为组，使用默认行为组
                var defaultGroup = GetBehaviorGroup("NormalMonster");
                defaultGroup?.Initialize(ai, charactor);
            }
        }

        /// <summary>
        /// 根据怪物类型获取默认行为组名称
        /// </summary>
        /// <param name="monsterType">怪物类型</param>
        /// <returns>默认行为组名称</returns>
        private static string GetDefaultBehaviorGroupName(eEntityType monsterType)
        {
            switch (monsterType)
            {
                case eEntityType.Monster:
                    return "NormalMonster";
                case eEntityType.Elite:
                    return "EliteMonster";
                case eEntityType.Boss:
                    return "Boss";
                default:
                    return "NormalMonster";
            }
        }
    }
}