
using System;
using UnityEngine;
using Game.Logic.BattleModule.Entity;

namespace HotFixBattle.AI
{
    /// <summary>
    /// AI状态接口
    /// </summary>
    public interface IAIState
    {
        /// <summary>
        /// 状态名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 进入状态
        /// </summary>
        /// <param name="context">AI上下文</param>
        void Enter(AIContext context);

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="context">AI上下文</param>
        /// <param name="deltaTime">时间增量</param>
        void Update(AIContext context, float deltaTime);

        /// <summary>
        /// 退出状态
        /// </summary>
        /// <param name="context">AI上下文</param>
        void Exit(AIContext context);
    }

    /// <summary>
    /// AI上下文，包含AI决策所需的所有信息
    /// </summary>
    public class AIContext
    {
        /// <summary>
        /// 关联的实体
        /// </summary>
        public IEntity Entity { get; set; }

        /// <summary>
        /// 当前AI状态
        /// </summary>
        public IAIState CurrentState { get; set; }

        /// <summary>
        /// AI配置
        /// </summary>
        public cfg.AIConfig Config { get; set; }

        /// <summary>
        /// 上次攻击时间
        /// </summary>
        public float LastAttackTime { get; set; }

        /// <summary>
        /// 初始位置（用于巡逻）
        /// </summary>
        public Vector3 InitialPosition { get; set; }

        /// <summary>
        /// 目标位置（用于移动）
        /// </summary>
        public Vector3 TargetPosition { get; set; }

        /// <summary>
        /// 当前目标实体
        /// </summary>
        public IEntity Target { get; set; }

        /// <summary>
        /// 是否可以攻击
        /// </summary>
        public bool CanAttack => Time.time - LastAttackTime >= Config.AttackCooldown;
    }
}
