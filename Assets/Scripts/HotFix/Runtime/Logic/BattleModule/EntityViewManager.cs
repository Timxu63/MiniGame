using System.Collections.Generic;
using cfg;
using Framework;
using UnityEngine;
using Framework.EventSystem;
using Framework.Runtime;
using Game.Logic.BattleModule.Entity;
using HotFix;
using HotFixBattle.AI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace HotFixBattle
{
    public class EntityViewData
    {
        public GameObject View;
        public Animator Animator;
        public EntityComponent EntityComponent;
        //实体没有创建结束时事件缓存
        public List<BaseEventArgs> cacheEvents = new List<BaseEventArgs>();
    }

    /// <summary>
    /// 实体视图管理器，负责处理实体的视图层逻辑
    /// </summary>
    public class EntityViewManager : Singleton<EntityViewManager>
    {
        // 实体ID到视图数据的映射
        private readonly Dictionary<int, EntityViewData> _entityViewDatas = new Dictionary<int, EntityViewData>();

        /// <summary>
        /// 初始化实体视图管理器
        /// </summary>
        public void Initialize()
        {                                          
            // 注册事件监听
            GameApp.Event.RegisterEvent((int)LocalMessageName.CC_EntityCreated, OnEntityCreated);
            GameApp.Event.RegisterEvent((int)LocalMessageName.CC_EntityDestroyed, OnEntityDestroyed);
            GameApp.Event.RegisterEvent((int)LocalMessageName.CC_EntityDamaged, OnEntityDamaged);
            GameApp.Event.RegisterEvent((int)LocalMessageName.CC_EntityHealed, OnEntityHealed);
            GameApp.Event.RegisterEvent((int)LocalMessageName.CC_EntityDeath, OnEntityDeath);
            GameApp.Event.RegisterEvent((int)LocalMessageName.CC_EntityMove, OnEntityMove);
            GameApp.Event.RegisterEvent((int)LocalMessageName.CC_EntityAttack, OnEntityAttack);
        }

        /// <summary>
        /// 清理实体视图管理器
        /// </summary>
        public void Cleanup()
        {
            // 移除事件监听
            GameApp.Event.UnRegisterEvent((int)LocalMessageName.CC_EntityCreated, OnEntityCreated);
            GameApp.Event.UnRegisterEvent((int)LocalMessageName.CC_EntityDestroyed, OnEntityDestroyed);
            GameApp.Event.UnRegisterEvent((int)LocalMessageName.CC_EntityDamaged, OnEntityDamaged);
            GameApp.Event.UnRegisterEvent((int)LocalMessageName.CC_EntityHealed, OnEntityHealed);
            GameApp.Event.UnRegisterEvent((int)LocalMessageName.CC_EntityDeath, OnEntityDeath);
            GameApp.Event.UnRegisterEvent((int)LocalMessageName.CC_EntityMove, OnEntityMove);
            GameApp.Event.UnRegisterEvent((int)LocalMessageName.CC_EntityAttack, OnEntityAttack);

            // 清理所有视图
            foreach (var viewData in _entityViewDatas.Values)
            {
                if (viewData.View != null)
                {
                    Object.Destroy(viewData.View);
                }
            }

            _entityViewDatas.Clear();
        }


        /// <summary>
        /// 实体创建事件处理
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="eventArgs">事件参数</param>
        private void OnEntityCreated(int type, BaseEventArgs eventArgs)
        {
            if (eventArgs is EntityCreatedEventArgs args && args.Entity != null)
            {
                CreateEntityView(args.Entity);
            }
        }

        /// <summary>
        /// 实体销毁事件处理
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="eventArgs">事件参数</param>
        private void OnEntityDestroyed(int type, BaseEventArgs eventArgs)
        {
            if (eventArgs is EntityDestroyedEventArgs args)
            {
                DestroyEntityView(args.EntityId);
            }
        }

        /// <summary>
        /// 实体受伤事件处理
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="eventArgs">事件参数</param>
        private void OnEntityDamaged(int type, BaseEventArgs eventArgs)
        {
            if (eventArgs is EntityDamagedEventArgs args)
            {
                OnEntityDamaged(args.EntityId, args.Damage);
            }
        }

        /// <summary>
        /// 实体治疗事件处理
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="eventArgs">事件参数</param>
        private void OnEntityHealed(int type, BaseEventArgs eventArgs)
        {
            if (eventArgs is EntityHealedEventArgs args)
            {
                OnEntityHealed(args.EntityId, args.Amount);
            }
        }

        /// <summary>
        /// 实体死亡事件处理
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="eventArgs">事件参数</param>
        private void OnEntityDeath(int type, BaseEventArgs eventArgs)
        {
            if (eventArgs is EntityDeathEventArgs args)
            {
                OnEntityDeath(args.EntityId);
            }
        }

        /// <summary>
        /// 创建实体视图
        /// </summary>
        /// <param name="entity">实体对象</param>
        private void CreateEntityView(IEntity entity)
        {
            if (entity == null)
            {
                return;
            }

            // 根据实体类型创建不同的视图
            GameObject entityView = null;

            switch (entity.Type)
            {
                case eEntityType.Player:
                     CreatePlayerView(entity);
                    break;
                case eEntityType.Monster:
                case eEntityType.Elite:
                case eEntityType.Boss:
                    CreateMonsterView(entity);
                    break;
                case eEntityType.NPC:
                    CreateNPCView(entity);
                    break;
                case eEntityType.Object:
                    CreateObjectView(entity);
                    break;
            }
        }

        /// <summary>
        /// 销毁实体视图
        /// </summary>
        /// <param name="entityId">实体ID</param>
        private void DestroyEntityView(int entityId)
        {
            if (_entityViewDatas.TryGetValue(entityId, out var viewData))
            {
                if (viewData.View != null)
                {
                    Object.Destroy(viewData.View);
                }

                _entityViewDatas.Remove(entityId);

                Debug.Log($"[EntityViewManager] 销毁实体视图: ID {entityId}");
            }
        }

        /// <summary>
        /// 实体受伤处理
        /// </summary>
        /// <param name="entityId">实体ID</param>
        /// <param name="damage">伤害值</param>
        private void OnEntityDamaged(int entityId, int damage)
        {
            if (_entityViewDatas.TryGetValue(entityId, out var viewData))
            {
                // 播放受伤动画
                if (viewData.Animator != null)
                {
                    viewData.Animator.SetTrigger("Damaged");
                }

                // 显示伤害数字
                ShowDamageNumber(viewData.View.transform.position, damage);

                Debug.Log($"[EntityViewManager] 实体受伤: ID {entityId}, 伤害 {damage}");
            }
        }

        /// <summary>
        /// 实体治疗处理
        /// </summary>
        /// <param name="entityId">实体ID</param>
        /// <param name="amount">治疗量</param>
        private void OnEntityHealed(int entityId, int amount)
        {
            if (_entityViewDatas.TryGetValue(entityId, out var viewData))
            {
                // 显示治疗数字
                ShowHealNumber(viewData.View.transform.position, amount);

                Debug.Log($"[EntityViewManager] 实体治疗: ID {entityId}, 治疗 {amount}");
            }
        }

        /// <summary>
        /// 实体死亡处理
        /// </summary>
        /// <param name="entityId">实体ID</param>
        private void OnEntityDeath(int entityId)
        {
            if (_entityViewDatas.TryGetValue(entityId, out var viewData))
            {
                // 播放死亡动画
                if (viewData.Animator != null)
                {
                    viewData.Animator.SetTrigger("Death");
                }

                // 延迟销毁视图
                Object.Destroy(viewData.View, 2.0f);

                Debug.Log($"[EntityViewManager] 实体死亡: ID {entityId}");
            }
        }

        /// <summary>
        /// 创建玩家视图
        /// </summary>
        /// <param name="entity">玩家实体</param>
        /// <returns>玩家视图游戏对象</returns>
        private void CreatePlayerView(IEntity entity)
        {
            
            // 获取角色配置
            var charactorConfig = GameTableProxy.Tables.TbCharactor.Get(entity.Id);
            if (charactorConfig == null)
            {
                Debug.LogError($"[EntityViewManager] 未找到角色配置，ID: {entity.Id}");
                return;
            }

            // 获取资源配置
            var resourceConfig = GameTableProxy.Tables.TbResource.Get(charactorConfig.ResourceID);
            if (resourceConfig == null)
            {
                Debug.LogError($"[EntityViewManager] 未找到资源配置，ID: {charactorConfig.ResourceID}");;
                return;
            }

            // 通过Addressable加载资源
            GameObject playerView = null;
            var loadOperation = Addressables.LoadAssetAsync<GameObject>(resourceConfig.Path);
            loadOperation.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    playerView = GameObject.Instantiate(op.Result);
                    playerView.name = $"Player_{entity.Name}_{entity.Id}";
                    
                    // 处理实体视图的通用逻辑
                    ProcessEntityView(entity, playerView);
                }
                else
                {
                    Debug.LogError($"[EntityViewManager] 加载资源失败: {resourceConfig.Path}");
                }
            };
        }

        /// <summary>
        /// 创建怪物视图
        /// </summary>
        /// <param name="entity">怪物实体</param>
        /// <returns>怪物视图游戏对象</returns>
        private void CreateMonsterView(IEntity entity)
        {
            // 获取角色配置
            var charactorConfig = entity.Charactor;
            if (charactorConfig == null)
            {
                Debug.LogError($"[EntityViewManager] 未找到角色配置，ID: {entity.Charactor.Id}");
                return;
            }

            // 获取资源配置
            var resourceConfig = GameTableProxy.Tables.TbResource.Get(charactorConfig.ResourceID);
            if (resourceConfig == null)
            {
                Debug.LogError($"[EntityViewManager] 未找到资源配置，ID: {charactorConfig.ResourceID}");
                return;
            }

            // 通过Addressable加载资源
            GameObject monsterView = null;
            var loadOperation = Addressables.LoadAssetAsync<GameObject>(resourceConfig.Path);
            loadOperation.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    monsterView = GameObject.Instantiate(op.Result);
                    monsterView.name = $"Monster_{entity.Name}_{entity.Id}";
                    
                    // 处理实体视图的通用逻辑
                    ProcessEntityView(entity, monsterView);
                }
                else
                {
                    Debug.LogError($"[EntityViewManager] 加载资源失败: {resourceConfig.Path}");
                }
            };
        }

        /// <summary>
        /// 创建NPC视图
        /// </summary>
        /// <param name="entity">NPC实体</param>
        /// <returns>NPC视图游戏对象</returns>
        private void CreateNPCView(IEntity entity)
        {
            // 获取角色配置
            var charactorConfig = GameTableProxy.Tables.TbCharactor.Get(entity.Id);
            if (charactorConfig == null)
            {
                Debug.LogError($"[EntityViewManager] 未找到角色配置，ID: {entity.Id}");
                return;
            }

            // 获取资源配置
            var resourceConfig = GameTableProxy.Tables.TbResource.Get(charactorConfig.ResourceID);
            if (resourceConfig == null)
            {
                Debug.LogError($"[EntityViewManager] 未找到资源配置，ID: {charactorConfig.ResourceID}");
                return;
            }

            // 通过Addressable加载资源
            GameObject npcView = null;
            var loadOperation = Addressables.LoadAssetAsync<GameObject>(resourceConfig.Path);
            loadOperation.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    npcView = GameObject.Instantiate(op.Result);
                    npcView.name = $"NPC_{entity.Name}_{entity.Id}";
                    
                    // 这些通用逻辑已经移到CreateEntityView方法中
                }
                else
                {
                    Debug.LogError($"[EntityViewManager] 加载资源失败: {resourceConfig.Path}");
                }
            };
        }

        /// <summary>
        /// 创建物体视图
        /// </summary>
        /// <param name="entity">物体实体</param>
        /// <returns>物体视图游戏对象</returns>
        private void CreateObjectView(IEntity entity)
        {
            var charactorConfig = GameTableProxy.Tables.TbCharactor.Get(entity.Id);
            if (charactorConfig == null)
            {
                Debug.LogError($"[EntityViewManager] 未找到角色配置，ID: {entity.Id}");
                return;
            }

            // 获取资源配置
            var resourceConfig = GameTableProxy.Tables.TbResource.Get(charactorConfig.ResourceID);
            if (resourceConfig == null)
            {
                Debug.LogError($"[EntityViewManager] 未找到资源配置，ID: {charactorConfig.ResourceID}");
                return;
            }

            // 通过Addressable加载资源
            GameObject npcView = null;
            var loadOperation = Addressables.LoadAssetAsync<GameObject>(resourceConfig.Path);
            loadOperation.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    npcView = GameObject.Instantiate(op.Result);
                    npcView.name = $"NPC_{entity.Name}_{entity.Id}";
                    
                    // 这些通用逻辑已经移到CreateEntityView方法中
                }
                else
                {
                    Debug.LogError($"[EntityViewManager] 加载资源失败: {resourceConfig.Path}");
                }
            };

        }

        /// <summary>
        /// 处理实体视图创建后的通用逻辑
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="entityView">实体视图游戏对象</param>
        private void ProcessEntityView(IEntity entity, GameObject entityView)
        {
            // 检查实体是否已经死亡
            if (!entity.IsAlive)
            {
                Debug.LogWarning($"[EntityViewManager] 尝试为已死亡的实体创建视图: {entity.Name} (ID: {entity.Id})");
                
                // 如果实体已死亡，直接销毁视图
                Object.Destroy(entityView);
                return;
            }
            
            // 创建视图数据
            var viewData = new EntityViewData
            {
                View = entityView,
                Animator = entityView.GetComponent<Animator>(),
                EntityComponent = entityView.GetComponent<EntityComponent>()
            };
            
            // 确保有EntityComponent
            if (viewData.EntityComponent == null)
            {
                viewData.EntityComponent = entityView.AddComponent<EntityComponent>();
                viewData.EntityComponent.Entity = entity;
            }
            
            // 设置为EntityRoot的子对象
            entityView.transform.SetParent(GameNode.Instance.EntityRoot.transform);
            entityView.transform.localPosition = Vector3.zero;
            // 保存视图数据
            _entityViewDatas[entity.Id] = viewData;
            
            Debug.Log($"[EntityViewManager] 创建实体视图: {entity.Name} (ID: {entity.Id}, Type: {entity.Type})");
        }

        /// <summary>
        /// 显示伤害数字
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="damage">伤害值</param>
        private void ShowDamageNumber(Vector3 position, int damage)
        {
            // 这里应该创建伤害数字UI并显示在指定位置
            // 暂时只在控制台输出
            Debug.Log($"[EntityViewManager] 显示伤害数字: {damage} 在位置 {position}");
        }

        /// <summary>
        /// 显示治疗数字
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="amount">治疗量</param>
        private void ShowHealNumber(Vector3 position, int amount)
        {
            // 这里应该创建治疗数字UI并显示在指定位置
            // 暂时只在控制台输出
            Debug.Log($"[EntityViewManager] 显示治疗数字: {amount} 在位置 {position}");
        }

        /// <summary>
        /// 实体移动事件处理
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="eventArgs">事件参数</param>
        private void OnEntityMove(int type, BaseEventArgs eventArgs)
        {
            if (eventArgs is EntityMoveEventArgs args)
            {
                if (_entityViewDatas.TryGetValue(args.EntityId, out var viewData))
                {
                    // 更新实体视图位置
                    viewData.View.transform.position = args.Position;

                    // 更新实体位置
                    if (viewData.EntityComponent != null && viewData.EntityComponent.Entity != null)
                    {
                        viewData.EntityComponent.Entity.SetPosition(args.Position);
                    }
                }
            }
        }

        /// <summary>
        /// 实体攻击事件处理
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="eventArgs">事件参数</param>
        private void OnEntityAttack(int type, BaseEventArgs eventArgs)
        {
            if (eventArgs is EntityAttackEventArgs args)
            {
                // 播放攻击动画
                if (_entityViewDatas.TryGetValue(args.AttackerId, out var attackerView))
                {
                    if (attackerView.Animator != null)
                    {
                        attackerView.Animator.SetTrigger("Attack");
                    }
                }

                // 播放受击动画
                if (_entityViewDatas.TryGetValue(args.TargetId, out var targetView))
                {
                    if (targetView.Animator != null)
                    {
                        targetView.Animator.SetTrigger("Hit");
                    }
                }

                Debug.Log($"[EntityViewManager] 实体攻击: 攻击者 {args.AttackerId} 目标 {args.TargetId}");
            }
        }
    }
}
