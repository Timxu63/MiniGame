
using System;
using System.Collections.Generic;
using cfg;
using Framework.Runtime;
using HotFixBattle;
using UnityEngine;

namespace Game.Logic.BattleModule.Entity
{
    /// <summary>
    /// 实体工厂类，使用工厂模式创建不同类型的实体
    /// </summary>
    public class EntityFactory
    {
        // 存储实体创建函数的字典
        private static Dictionary<eEntityType, Func<EntityCreationParams, BaseEntity>> _entityCreators = 
            new Dictionary<eEntityType, Func<EntityCreationParams, BaseEntity>>
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
        public static BaseEntity CreateEntity(eEntityType type, EntityCreationParams @params)
        {
            if (_entityCreators.ContainsKey(type))
            {
                var entity = _entityCreators[type](@params);
                // 设置实体位置
                entity.LocalPosition = @params.Position;
                // 自动将创建的实体添加到实体管理器
                SimpleEntityManager.Instance.AddEntity(entity);
                // 发送实体创建事件
                if (entity != null)
                {
                    GameApp.Event.DispatchNow((int)LocalMessageName.CC_EntityCreated, new EntityCreatedEventArgs(entity));
                }

                return entity;
            }

            throw new ArgumentException($"未知的实体类型: {type}");
        }

        /// <summary>
        /// 注册新的实体创建函数
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="creator">创建函数</param>
        public static void RegisterEntityCreator(eEntityType type, Func<EntityCreationParams, BaseEntity> creator)
        {
            _entityCreators[type] = creator;
        }

        /// <summary>
        /// 创建玩家实体
        /// </summary>
        /// <param name="params">创建参数</param>
        /// <returns>玩家实体</returns>
        private static BaseEntity CreatePlayer(EntityCreationParams @params)
        {
            if (@params is PlayerCreationParams playerParams)
            {
                return new PlayerEntity(
                    playerParams.Name,
                    playerParams.MaxHealth,
                    playerParams.CharactorConfig,
                    playerParams.Level
                );
            }

            throw new ArgumentException("创建玩家实体需要PlayerCreationParams参数");
        }

        /// <summary>
        /// 创建怪物实体
        /// </summary>
        /// <param name="params">创建参数</param>
        /// <returns>怪物实体</returns>
        private static BaseEntity CreateMonster(EntityCreationParams @params)
        {
            if (@params is MonsterCreationParams monsterParams)
            {
                return new MonsterEntity(
                    monsterParams.Name,
                    monsterParams.MaxHealth,
                    monsterParams.CharactorConfig,
                    monsterParams.Level,
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
        private static BaseEntity CreateNPC(EntityCreationParams @params)
        {
            // 简单实现，实际项目中可能需要更复杂的NPC类
            if (@params is NPCCreationParams npcParams)
            {
                return new NPCEntity(npcParams.Name, npcParams.MaxHealth, npcParams.CharactorConfig);
            }

            throw new ArgumentException("创建NPC实体需要NPCCreationParams参数");
        }

        /// <summary>
        /// 创建物体实体
        /// </summary>
        /// <param name="params">创建参数</param>
        /// <returns>物体实体</returns>
        private static BaseEntity CreateObject(EntityCreationParams @params)
        {
            // 简单实现，实际项目中可能需要更复杂的Object类
            if (@params is ObjectCreationParams objectParams)
            {
                return new ObjectEntity(objectParams.Name, objectParams.MaxHealth, objectParams.CharactorConfig);
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
        public Charactor CharactorConfig;
        public Vector3 Position { get; set; } = Vector3.zero; // 默认位置为 (0,0,0)
    }

    /// <summary>
    /// 玩家创建参数
    /// </summary>
    public class PlayerCreationParams : EntityCreationParams
    {
        public int Level { get; set; } = 1;
    }

    /// <summary>
    /// 怪物创建参数
    /// </summary>
    public class MonsterCreationParams : EntityCreationParams
    {
        public int Level { get; set; } = 1;
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
