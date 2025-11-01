using System.Collections.Generic;
using cfg;
using Framework.EventSystem;
using Framework.Runtime;
using Game.Logic.BattleModule.Entity;
using HotFixBattle;

namespace Game.Logic.BattleModule
{
    /// <summary>
    /// 实体生成器，用于在任务进入时创建对应的怪物
    /// </summary>
    public class EntitySpawner
    {
        private static EntitySpawner _instance;

        /// <summary>
        /// 获取实体生成器实例
        /// </summary>
        public static EntitySpawner Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EntitySpawner();
                }
                return _instance;
            }
        }

        // 角色配置缓存
        private Dictionary<int, Charactor> _charactorConfigs = new Dictionary<int, Charactor>();

        // 资源配置缓存
        private Dictionary<int, Resource> _resourceConfigs = new Dictionary<int, Resource>();

        // 私有构造函数，实现单例模式
        private EntitySpawner()
        {
        }

        /// <summary>
        /// 初始化角色和资源配置
        /// </summary>
        /// <param name="tables">表格数据</param>
        public void InitializeConfigs(Tables tables)
        {
            if (tables == null)
            {
                return;
            }

            // 缓存所有角色配置
            if (tables.TbCharactor != null)
            {
                foreach (var charactorConfig in tables.TbCharactor.DataList)
                {
                    _charactorConfigs[charactorConfig.Id] = charactorConfig;
                }
            }

            // 缓存所有资源配置
            if (tables.TbResource != null)
            {
                foreach (var resourceConfig in tables.TbResource.DataList)
                {
                    _resourceConfigs[resourceConfig.Id] = resourceConfig;
                }
            }
        }

        /// <summary>
        /// 为任务生成怪物
        /// </summary>
        /// <param name="missionData">任务数据</param>
        /// <param name="worldContext">世界上下文</param>
        /// <returns>生成的怪物ID列表</returns>
        public List<int> SpawnMonstersForMission(Chapter_Mission missionData, BattleWorldContext worldContext)
        {
            List<int> spawnedMonsterIds = new List<int>();

            if (missionData?.MonsterId == null)
            {
                return spawnedMonsterIds;
            }

            // 获取实体管理器
            var entityManager = SimpleEntityManager.Instance;

            // 为每个怪物ID生成怪物
            foreach (var monsterId in missionData.MonsterId)
            {
                var monster = SpawnMonster(monsterId, worldContext);
                if (monster != null)
                {
                    entityManager.AddEntity(monster);
                    spawnedMonsterIds.Add(monster.Id);
                }
            }

            return spawnedMonsterIds;
        }

        /// <summary>
        /// 生成单个怪物
        /// </summary>
        /// <param name="monsterId">怪物配置ID</param>
        /// <param name="worldContext">世界上下文</param>
        /// <returns>生成的怪物实体</returns>
        public IEntity SpawnMonster(int monsterId, BattleWorldContext worldContext)
        {
            // 获取角色配置
            if (!_charactorConfigs.TryGetValue(monsterId, out var charactorConfig))
            {
                // 如果缓存中没有，尝试从表格中获取
                if (worldContext?.Tables?.TbCharactor != null)
                {
                    charactorConfig = worldContext.Tables.TbCharactor.Get(monsterId);
                    if (charactorConfig != null)
                    {
                        _charactorConfigs[monsterId] = charactorConfig;
                    }
                }

                if (charactorConfig == null)
                {
                    UnityEngine.Debug.LogError($"未找到角色配置，ID: {monsterId}");
                    return null;
                }
            }

            // 获取资源路径
            string resourcePath = "";
            if (_resourceConfigs.TryGetValue(charactorConfig.ResourceID, out var resourceConfig))
            {
                resourcePath = resourceConfig.Path;
            }
            else if (worldContext?.Tables?.TbResource != null)
            {
                resourceConfig = worldContext.Tables.TbResource.Get(charactorConfig.ResourceID);
                if (resourceConfig != null)
                {
                    _resourceConfigs[charactorConfig.ResourceID] = resourceConfig;
                    resourcePath = resourceConfig.Path;
                }
            }

            // 创建怪物参数
            var monsterParams = new MonsterCreationParams
            {
                Name = charactorConfig.Name,
                MaxHealth = 100, // 默认生命值，可以从配置表获取
                Level = 1, // 默认等级，可以从配置表获取
                AttackPower = 10, // 默认攻击力，可以从配置表获取
                Defense = 5, // 默认防御力，可以从配置表获取
                MonsterType = charactorConfig.Type, // 默认怪物类型，可以从配置表获取
                DropExperience = 10, // 默认经验值，可以从配置表获取
                DropGold = 5, // 默认金币，可以从配置表获取
                SurvivalTime = charactorConfig.SurvivalTime // 默认生存时间，可以从配置表获取
            };

            // 使用工厂创建怪物
            var monster = EntityFactory.CreateEntity(eEntityType.Monster, monsterParams) as MonsterEntity;
            
            // 发送实体创建事件
            if (monster != null)
            {
                GameApp.Event.DispatchNow((int)LocalMessageName.CC_EntityCreated, new EntityCreatedEventArgs(monster));
            }

            return monster;
        }

        /// <summary>
        /// 清除所有怪物
        /// </summary>
        public void ClearAllMonsters()
        {
            var entityManager = SimpleEntityManager.Instance;
            var monsters = entityManager.GetEntitiesByType(eEntityType.Monster);

            foreach (var monster in monsters)
            {
                entityManager.RemoveEntity(monster.Id);
                
                // 发送实体销毁事件
                GameApp.Event.DispatchNow((int)LocalMessageName.CC_EntityDestroyed, new EntityDestroyedEventArgs(monster.Id));
            }
        }

        /// <summary>
        /// 清除指定ID的怪物
        /// </summary>
        /// <param name="monsterIds">怪物ID列表</param>
        public void ClearMonsters(List<int> monsterIds)
        {
            var entityManager = SimpleEntityManager.Instance;

            foreach (var monsterId in monsterIds)
            {
                entityManager.RemoveEntity(monsterId);
                
                // 发送实体销毁事件
                GameApp.Event.DispatchNow((int)LocalMessageName.CC_EntityDestroyed, new EntityDestroyedEventArgs(monsterId));
            }
        }
    }
}
