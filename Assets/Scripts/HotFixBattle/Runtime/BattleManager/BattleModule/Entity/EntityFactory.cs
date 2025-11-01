
using System;
using System.Collections.Generic;
using cfg;

namespace Game.Logic.BattleModule.Entity
{
    /// <summary>
    /// 实体工厂类，使用工厂模式创建不同类型的实体
    /// </summary>
    public class EntityFactory
    {
        // 存储实体创建函数的字典
        private static Dictionary<eEntityType, Func<EntityCreationParams, IEntity>> _entityCreators = 
            new Dictionary<eEntityType, Func<EntityCreationParams, IEntity>>
        {
            { eEntityType.Player, CreatePlayer },
            { eEntityType.Monster, CreateMonster },
            { eEntityType.NPC, CreateNPC },
            { eEntityType.Object, CreateObject }
        };

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="params">创建参数</param>
        /// <returns>创建的实体</returns>
        public static IEntity CreateEntity(eEntityType type, EntityCreationParams @params)
        {
            if (_entityCreators.ContainsKey(type))
            {
                return _entityCreators[type](@params);
            }

            throw new ArgumentException($"未知的实体类型: {type}");
        }

        /// <summary>
        /// 注册新的实体创建函数
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="creator">创建函数</param>
        public static void RegisterEntityCreator(eEntityType type, Func<EntityCreationParams, IEntity> creator)
        {
            _entityCreators[type] = creator;
        }

        /// <summary>
        /// 创建玩家实体
        /// </summary>
        /// <param name="params">创建参数</param>
        /// <returns>玩家实体</returns>
        private static IEntity CreatePlayer(EntityCreationParams @params)
        {
            if (@params is PlayerCreationParams playerParams)
            {
                return new PlayerEntity(
                    playerParams.Name,
                    playerParams.MaxHealth,
                    playerParams.Level,
                    playerParams.AttackPower,
                    playerParams.Defense
                );
            }

            throw new ArgumentException("创建玩家实体需要PlayerCreationParams参数");
        }

        /// <summary>
        /// 创建怪物实体
        /// </summary>
        /// <param name="params">创建参数</param>
        /// <returns>怪物实体</returns>
        private static IEntity CreateMonster(EntityCreationParams @params)
        {
            if (@params is MonsterCreationParams monsterParams)
            {
                return new MonsterEntity(
                    monsterParams.Name,
                    monsterParams.MaxHealth,
                    monsterParams.Level,
                    monsterParams.AttackPower,
                    monsterParams.Defense,
                    monsterParams.MonsterType,
                    monsterParams.DropExperience,
                    monsterParams.DropGold,
                    monsterParams.SurvivalTime
                );
            }

            throw new ArgumentException("创建怪物实体需要MonsterCreationParams参数");
        }

        /// <summary>
        /// 创建NPC实体
        /// </summary>
        /// <param name="params">创建参数</param>
        /// <returns>NPC实体</returns>
        private static IEntity CreateNPC(EntityCreationParams @params)
        {
            // 简单实现，实际项目中可能需要更复杂的NPC类
            if (@params is NPCCreationParams npcParams)
            {
                return new NPCEntity(npcParams.Name, npcParams.MaxHealth);
            }

            throw new ArgumentException("创建NPC实体需要NPCCreationParams参数");
        }

        /// <summary>
        /// 创建物体实体
        /// </summary>
        /// <param name="params">创建参数</param>
        /// <returns>物体实体</returns>
        private static IEntity CreateObject(EntityCreationParams @params)
        {
            // 简单实现，实际项目中可能需要更复杂的Object类
            if (@params is ObjectCreationParams objectParams)
            {
                return new ObjectEntity(objectParams.Name, objectParams.MaxHealth);
            }

            throw new ArgumentException("创建物体实体需要ObjectCreationParams参数");
        }
    }

    /// <summary>
    /// 实体创建参数基类
    /// </summary>
    public abstract class EntityCreationParams
    {
        public string Name { get; set; }
        public int MaxHealth { get; set; }
    }

    /// <summary>
    /// 玩家创建参数
    /// </summary>
    public class PlayerCreationParams : EntityCreationParams
    {
        public int Level { get; set; } = 1;
        public int AttackPower { get; set; } = 10;
        public int Defense { get; set; } = 5;
    }

    /// <summary>
    /// 怪物创建参数
    /// </summary>
    public class MonsterCreationParams : EntityCreationParams
    {
        public int Level { get; set; } = 1;
        public int AttackPower { get; set; } = 5;
        public int Defense { get; set; } = 2;
        public eEntityType MonsterType { get; set; } = eEntityType.Monster;
        public int DropExperience { get; set; } = 10;
        public int DropGold { get; set; } = 5;
        public float SurvivalTime { get; set; } = 10f;
    }

    /// <summary>
    /// NPC创建参数
    /// </summary>
    public class NPCCreationParams : EntityCreationParams
    {
        // NPC特有参数
    }

    /// <summary>
    /// 物体创建参数
    /// </summary>
    public class ObjectCreationParams : EntityCreationParams
    {
        // 物体特有参数
    }
}
