using System.Collections.Generic;
using cfg;
using Framework;
using UnityEngine;
using Framework.EventSystem;
using Framework.Runtime;
using Game.Logic.BattleModule.Entity;
using HotFix;

namespace HotFixBattle
{
    /// <summary>
    /// 实体视图管理器，负责处理实体的视图层逻辑
    /// </summary>
    public class EntityViewManager : Singleton<EntityViewManager>
    {
        // 实体ID到视图的映射
        private readonly Dictionary<int, GameObject> _entityViews = new Dictionary<int, GameObject>();

        // 实体ID到动画组件的映射
        private readonly Dictionary<int, Animator> _entityAnimators = new Dictionary<int, Animator>();

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

            // 清理所有视图
            foreach (var view in _entityViews.Values)
            {
                if (view != null)
                {
                    Object.Destroy(view);
                }
            }

            _entityViews.Clear();
            _entityAnimators.Clear();
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
                    entityView = CreatePlayerView(entity);
                    break;
                case eEntityType.Monster:
                    entityView = CreateMonsterView(entity);
                    break;
                case eEntityType.NPC:
                    entityView = CreateNPCView(entity);
                    break;
                case eEntityType.Object:
                    entityView = CreateObjectView(entity);
                    break;
            }

            if (entityView != null)
            {
                // 设置为EntityRoot的子对象
                entityView.transform.SetParent(GameNode.Instance.EntityRoot.transform);

                // 保存视图引用
                _entityViews[entity.Id] = entityView;

                // 获取动画组件
                var animator = entityView.GetComponent<Animator>();
                if (animator != null)
                {
                    _entityAnimators[entity.Id] = animator;
                }

                // 添加实体组件引用
                var entityComponent = entityView.AddComponent<EntityComponent>();
                entityComponent.Entity = entity;

                Debug.Log($"[EntityViewManager] 创建实体视图: {entity.Name} (ID: {entity.Id}, Type: {entity.Type})");
            }
        }

        /// <summary>
        /// 销毁实体视图
        /// </summary>
        /// <param name="entityId">实体ID</param>
        private void DestroyEntityView(int entityId)
        {
            if (_entityViews.TryGetValue(entityId, out var view))
            {
                if (view != null)
                {
                    Object.Destroy(view);
                }

                _entityViews.Remove(entityId);
                _entityAnimators.Remove(entityId);

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
            if (_entityViews.TryGetValue(entityId, out var view))
            {
                // 播放受伤动画
                if (_entityAnimators.TryGetValue(entityId, out var animator))
                {
                    animator.SetTrigger("Damaged");
                }

                // 显示伤害数字
                ShowDamageNumber(view.transform.position, damage);

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
            if (_entityViews.TryGetValue(entityId, out var view))
            {
                // 显示治疗数字
                ShowHealNumber(view.transform.position, amount);

                Debug.Log($"[EntityViewManager] 实体治疗: ID {entityId}, 治疗 {amount}");
            }
        }

        /// <summary>
        /// 实体死亡处理
        /// </summary>
        /// <param name="entityId">实体ID</param>
        private void OnEntityDeath(int entityId)
        {
            if (_entityViews.TryGetValue(entityId, out var view))
            {
                // 播放死亡动画
                if (_entityAnimators.TryGetValue(entityId, out var animator))
                {
                    animator.SetTrigger("Death");
                }

                // 延迟销毁视图
                Object.Destroy(view, 2.0f);

                Debug.Log($"[EntityViewManager] 实体死亡: ID {entityId}");
            }
        }

        /// <summary>
        /// 创建玩家视图
        /// </summary>
        /// <param name="entity">玩家实体</param>
        /// <returns>玩家视图游戏对象</returns>
        private GameObject CreatePlayerView(IEntity entity)
        {
            
            // 这里应该从资源管理器加载玩家预制体
            // 暂时创建一个简单的立方体作为占位
            var playerView = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerView.name = $"Player_{entity.Name}_{entity.Id}";


            // 添加碰撞体
            var collider = playerView.GetComponent<CapsuleCollider>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }

            // 添加动画组件
            var animator = playerView.AddComponent<Animator>();

            // 设置材质颜色
            var renderer = playerView.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.blue;
            }

            return playerView;
        }

        /// <summary>
        /// 创建怪物视图
        /// </summary>
        /// <param name="entity">怪物实体</param>
        /// <returns>怪物视图游戏对象</returns>
        private GameObject CreateMonsterView(IEntity entity)
        {
            // 这里应该从资源管理器加载怪物预制体
            // 暂时创建一个简单的立方体作为占位
            var monsterView = GameObject.CreatePrimitive(PrimitiveType.Cube);
            monsterView.name = $"Monster_{entity.Name}_{entity.Id}";
         

            // 添加碰撞体
            var collider = monsterView.GetComponent<BoxCollider>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }

            // 添加动画组件
            var animator = monsterView.AddComponent<Animator>();

            // 设置材质颜色
            var renderer = monsterView.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.red;
            }

            return monsterView;
        }

        /// <summary>
        /// 创建NPC视图
        /// </summary>
        /// <param name="entity">NPC实体</param>
        /// <returns>NPC视图游戏对象</returns>
        private GameObject CreateNPCView(IEntity entity)
        {
            // 这里应该从资源管理器加载NPC预制体
            // 暂时创建一个简单的圆柱体作为占位
            var npcView = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            npcView.name = $"NPC_{entity.Name}_{entity.Id}";

            // 添加碰撞体
            var collider = npcView.GetComponent<CapsuleCollider>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }

            // 添加动画组件
            var animator = npcView.AddComponent<Animator>();

            // 设置材质颜色
            var renderer = npcView.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.green;
            }

            return npcView;
        }

        /// <summary>
        /// 创建物体视图
        /// </summary>
        /// <param name="entity">物体实体</param>
        /// <returns>物体视图游戏对象</returns>
        private GameObject CreateObjectView(IEntity entity)
        {
            // 这里应该从资源管理器加载物体预制体
            // 暂时创建一个简单的球体作为占位
            var objectView = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            objectView.name = $"Object_{entity.Name}_{entity.Id}";


            // 添加碰撞体
            var collider = objectView.GetComponent<SphereCollider>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }

            // 设置材质颜色
            var renderer = objectView.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.yellow;
            }

            return objectView;
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
    }
}
