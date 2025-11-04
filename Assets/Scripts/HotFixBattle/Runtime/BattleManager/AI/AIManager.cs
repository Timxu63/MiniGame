
using System.Collections.Generic;
using cfg;
using Framework;
using Framework.Runtime;
using Game.Logic.BattleModule.Entity;
using UnityEngine;

namespace HotFixBattle.AI
{
    /// <summary>
    /// AI状态工厂，用于创建不同类型的AI状态
    /// </summary>
    public static class AIStateFactory
    {
        /// <summary>
        /// 根据AI类型创建初始状态
        /// </summary>
        /// <param name="aiType">AI类型</param>
        /// <returns>AI状态实例</returns>
        public static IAIState CreateState(eAIType aiType)
        {
            switch (aiType)
            {
                case eAIType.Static:
                    return new StaticState();
                case eAIType.Random:
                    return new RandomMoveState();
                case eAIType.Patrol:
                    return new PatrolState();
                case eAIType.Chase:
                    return new ChaseState();
                case eAIType.Aggressive:
                    return new ChaseState(); // 攻击型AI初始状态为追逐
                case eAIType.Coward:
                    return new RandomMoveState(); // 胆小型AI初始状态为随机移动
                case eAIType.Guardian:
                    return new GuardianState();
                default:
                    return new StaticState();
            }
        }
    }

    /// <summary>
    /// AI管理器，负责管理所有实体的AI逻辑
    /// </summary>
    public class AIManager : Singleton<AIManager>
    {
        // 实体ID到AI上下文的映射
        private readonly Dictionary<int, AIContext> _entityAIs = new Dictionary<int, AIContext>();
        
        private BattleWorldContext _worldContext;
        /// <summary>
        /// 初始化AI管理器
        /// </summary>
        public void Initialize(BattleWorldContext worldContext)
        {
            _worldContext = worldContext;
            RegisterEntityEvents();
        }

        /// <summary>
        /// 注册实体创建事件监听
        /// </summary>
        public void RegisterEntityEvents()
        {
            // 注册事件监听
            GameApp.Event.RegisterEvent((int)LocalMessageName.CC_EntityCreated, OnEntityCreated);
            GameApp.Event.RegisterEvent((int)LocalMessageName.CC_EntityDestroyed, OnEntityDestroyed);
        }

        /// <summary>
        /// 清理AI管理器
        /// </summary>
        public void Cleanup()
        {
            // 移除事件监听
            GameApp.Event.UnRegisterEvent((int)LocalMessageName.CC_EntityCreated, OnEntityCreated);
            GameApp.Event.UnRegisterEvent((int)LocalMessageName.CC_EntityDestroyed, OnEntityDestroyed);

            // 清理所有AI
            _entityAIs.Clear();
        }

        /// <summary>
        /// 更新所有AI
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public void Update(float deltaTime)
        {
            // 复制一份实体AI列表，避免在迭代过程中修改集合
            var entityAIsCopy = new Dictionary<int, AIContext>(_entityAIs);

            foreach (var kvp in entityAIsCopy)
            {
                var entityId = kvp.Key;
                var context = kvp.Value;

                // 检查实体是否还存在
                if (context.Entity == null || !context.Entity.IsAlive)
                {
                    _entityAIs.Remove(entityId);
                    continue;
                }

                // 更新AI状态
                context.CurrentState?.Update(context, deltaTime);

                // 特殊AI逻辑处理
                HandleSpecialAILogic(context);
            }
        }

        /// <summary>
        /// 处理特殊AI逻辑
        /// </summary>
        /// <param name="context">AI上下文</param>
        private void HandleSpecialAILogic(AIContext context)
        {
            // 胆小型AI：生命值低时逃跑
            if (context.Config.AIType == eAIType.Coward)
            {
                float healthPercentage = (float)context.Entity.CurrentHealth / context.Entity.MaxHealth;
                if (healthPercentage < context.Config.FleeThreshold && !(context.CurrentState is FleeState))
                {
                    // 寻找最近的玩家作为逃跑目标
                    var nearestPlayer = FindNearestPlayer(context.Entity.Position);
                    if (nearestPlayer != null)
                    {
                        context.Target = nearestPlayer;
                        context.CurrentState = new FleeState();
                        context.CurrentState.Enter(context);
                    }
                }
            }
            // 攻击型AI：没有目标时寻找目标
            else if (context.Config.AIType == eAIType.Aggressive && context.Target == null)
            {
                var nearestPlayer = FindNearestPlayer(context.Entity.Position, context.Config.DetectionRange);
                if (nearestPlayer != null)
                {
                    context.Target = nearestPlayer;
                    if (!(context.CurrentState is ChaseState) && !(context.CurrentState is AttackState))
                    {
                        context.CurrentState = new ChaseState();
                        context.CurrentState.Enter(context);
                    }
                }
            }
        }

        /// <summary>
        /// 查找最近的玩家
        /// </summary>
        /// <param name="position">搜索位置</param>
        /// <param name="range">搜索范围，0表示无限范围</param>
        /// <returns>最近的玩家实体</returns>
        private IEntity FindNearestPlayer(Vector3 position, float range = 0)
        {
            IEntity nearestPlayer = null;
            float nearestDistance = float.MaxValue;

            // 这里应该通过实体管理器获取所有玩家实体
            // 暂时使用简化的实现
            foreach (var kvp in _entityAIs)
            {
                var entity = kvp.Value.Entity;
                if (entity.Type == eEntityType.Player)
                {
                    float distance = Vector3.Distance(position, entity.Position);
                    if (range == 0 || distance <= range)
                    {
                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestPlayer = entity;
                        }
                    }
                }
            }

            return nearestPlayer;
        }

        /// <summary>
        /// 实体创建事件处理
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="eventArgs">事件参数</param>
        private void OnEntityCreated(int type, Framework.EventSystem.BaseEventArgs eventArgs)
        {
            if (eventArgs is EntityCreatedEventArgs args && args.Entity != null)
            {
                // 只为怪物、精英和Boss创建AI
                if (args.Entity.Type == eEntityType.Monster || 
                    args.Entity.Type == eEntityType.Elite || 
                    args.Entity.Type == eEntityType.Boss)
                {
                    CreateAI(args.Entity);
                }
            }
        }

        /// <summary>
        /// 实体销毁事件处理
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="eventArgs">事件参数</param>
        private void OnEntityDestroyed(int type, Framework.EventSystem.BaseEventArgs eventArgs)
        {
            if (eventArgs is EntityDestroyedEventArgs args)
            {
                RemoveAI(args.EntityId);
            }
        }

        /// <summary>
        /// 为实体创建AI
        /// </summary>
        /// <param name="entity">实体对象</param>
        private void CreateAI(IEntity entity)
        {
            // 获取AI配置
            cfg.AIConfig aiConfig = null;

            // 尝试从Chapter表获取AI参数
            // 这里假设可以通过某种方式获取实体所属的章节ID
            // 实际实现可能需要根据项目结构调整
            try
            {
                // 从怪物实体获取章节ID
                int chapterId = 1; // 默认章节ID
                if (entity is MonsterEntity monsterEntity)
                {
                    chapterId = monsterEntity.ChapterId;
                }

                // 获取章节配置
                var chapterConfig = _worldContext.Tables.TbChapter.Get(chapterId);
                if (chapterConfig != null && !string.IsNullOrEmpty(chapterConfig.AIParam))
                {
                    // 从JSON字符串创建AI配置
                    aiConfig = new cfg.AIConfig(chapterConfig.AIParam);
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"[AIManager] 获取AI配置失败: {e.Message}");
            }

            // 如果无法获取AI配置，使用默认配置
            if (aiConfig == null)
            {
                // 使用默认的JSON配置
                string defaultAIParam = @"{
                    aiType: 2,
                    moveSpeed: 2.0,
                    detectionRange: 5.0,
                    attackRange: 1.0,
                    attackCooldown: 1.0,
                    patrolRadius: 3.0,
                    fleeThreshold: 0.3
                }";
                aiConfig = new cfg.AIConfig(defaultAIParam);
            }

            // 创建AI上下文
            var context = new AIContext
            {
                Entity = entity,
                Config = aiConfig,
                InitialPosition = entity.Position,
                CurrentState = AIStateFactory.CreateState(aiConfig.AIType)
            };

            // 进入初始状态
            context.CurrentState.Enter(context);

            // 保存AI上下文
            _entityAIs[entity.Id] = context;
        }

        /// <summary>
        /// 移除实体的AI
        /// </summary>
        /// <param name="entityId">实体ID</param>
        private void RemoveAI(int entityId)
        {
            if (_entityAIs.TryGetValue(entityId, out var context))
            {
                // 退出当前状态
                context.CurrentState?.Exit(context);

                // 移除AI
                _entityAIs.Remove(entityId);
            }
        }
    }
}
