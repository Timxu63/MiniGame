
using System;
using cfg;

namespace Game.Logic.BattleModule.Entity
{
    /// <summary>
    /// 怪物实体类
    /// </summary>
    public class MonsterEntity : BaseEntity
    {
        /// <summary>
        /// 怪物等级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 怪物攻击力
        /// </summary>
        public int AttackPower { get; set; }

        /// <summary>
        /// 怪物防御力
        /// </summary>
        public int Defense { get; set; }

        /// <summary>
        /// 怪物类型
        /// </summary>
        public eEntityType MonsterType { get; set; }

        /// <summary>
        /// 掉落经验值
        /// </summary>
        public int DropExperience { get; set; }

        /// <summary>
        /// 掉落金币
        /// </summary>
        public int DropGold { get; set; }

        /// <summary>
        /// AI状态
        /// </summary>
        public MonsterAIState AIState { get; set; }

        /// <summary>
        /// 生存时间计时器（秒）
        /// </summary>
        private float _survivalTimer = 0f;

        /// <summary>
        /// 最大生存时间（秒）
        /// </summary>
        private float _maxSurvivalTime;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">怪物名称</param>
        /// <param name="maxHealth">最大生命值</param>
        /// <param name="level">怪物等级</param>
        /// <param name="attackPower">攻击力</param>
        /// <param name="defense">防御力</param>
        /// <param name="monsterType">怪物类型</param>
        /// <param name="dropExperience">掉落经验值</param>
        /// <param name="dropGold">掉落金币</param>
        /// <param name="survivalTime">生存时间（秒）</param>
        public MonsterEntity(string name, int maxHealth, int level = 1, int attackPower = 5, 
            int defense = 2, eEntityType monsterType = eEntityType.Monster, 
            int dropExperience = 10, int dropGold = 5, float survivalTime = 10f)
            : base(name, eEntityType.Monster, maxHealth)
        {
            Level = level;
            AttackPower = attackPower;
            Defense = defense;
            MonsterType = monsterType;
            DropExperience = dropExperience;
            DropGold = dropGold;
            AIState = MonsterAIState.Idle;
            _maxSurvivalTime = survivalTime;
        }

        /// <summary>
        /// 重写受到伤害方法
        /// </summary>
        /// <param name="damage">原始伤害值</param>
        /// <returns>实际受到的伤害</returns>
        public override int TakeDamage(int damage)
        {
            // 计算实际伤害，考虑防御力
            int actualDamage = Math.Max(1, damage - Defense);
            
            return base.TakeDamage(actualDamage);
        }

        /// <summary>
        /// 更新怪物AI状态
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // 更新生存时间计时器
            UpdateSurvivalTimer(deltaTime);

            // 简单的AI状态更新逻辑
            switch (AIState)
            {
                case MonsterAIState.Idle:
                    // 闲置状态，可以添加巡逻逻辑
                    break;

                case MonsterAIState.Patrol:
                    // 巡逻状态
                    break;

                case MonsterAIState.Chase:
                    // 追击状态
                    break;

                case MonsterAIState.Attack:
                    // 攻击状态
                    break;

                case MonsterAIState.Flee:
                    // 逃跑状态
                    break;
            }
        }

        /// <summary>
        /// 更新生存时间计时器
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        private void UpdateSurvivalTimer(float deltaTime)
        {
            // 增加生存时间计时器
            _survivalTimer += deltaTime;
            
            // 如果生存时间超过最大值，怪物自动死亡
            if (_maxSurvivalTime != 0 && _survivalTimer >= _maxSurvivalTime)
            {
                TakeDamage(CurrentHealth); // 造成等于当前生命值的伤害，导致死亡
            }
        }

        /// <summary>
        /// 重写死亡事件
        /// </summary>
        protected override void OnDeath()
        {
            base.OnDeath();
            // 怪物死亡的特殊处理
            // 例如：掉落物品、触发任务等
        }
    }

    /// <summary>
    /// 怪物AI状态枚举
    /// </summary>
    public enum MonsterAIState
    {
        Idle,      // 闲置
        Patrol,    // 巡逻
        Chase,     // 追击
        Attack,    // 攻击
        Flee       // 逃跑
    }
}
