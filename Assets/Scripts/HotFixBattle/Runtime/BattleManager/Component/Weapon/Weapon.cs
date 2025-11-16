
using System;
using System.Collections.Generic;
using Game.Logic.BattleModule.Entity;
using UnityEngine;

namespace Game.Logic.BattleModule.Component.Weapon
{
    /// <summary>
    /// 武器基类
    /// </summary>
    public abstract class Weapon
    {
        /// <summary>
        /// 武器ID
        /// </summary>
        public int Id { get; protected set; }

        /// <summary>
        /// 武器名称
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// 武器描述
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// 武器图标ID
        /// </summary>
        public int IconId { get; protected set; }

        /// <summary>
        /// 武器类型
        /// </summary>
        public WeaponType Type { get; protected set; }

        /// <summary>
        /// 基础攻击力
        /// </summary>
        public int BaseDamage { get; protected set; }

        /// <summary>
        /// 攻击速度（每秒攻击次数）
        /// </summary>
        public float AttackSpeed { get; set; }

        /// <summary>
        /// 攻击范围
        /// </summary>
        public float AttackRange { get; protected set; }

        /// <summary>
        /// 武器词条列表
        /// </summary>
        public List<WeaponAffix> Affixes { get; private set; }

        /// <summary>
        /// 上次攻击时间
        /// </summary>
        private float _lastAttackTime = 0f;

        /// <summary>
        /// 武器所属实体
        /// </summary>
        public BaseEntity Owner { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="owner">武器所属实体</param>
        /// <param name="id">武器ID</param>
        /// <param name="name">武器名称</param>
        /// <param name="description">武器描述</param>
        /// <param name="iconId">武器图标ID</param>
        /// <param name="type">武器类型</param>
        /// <param name="baseDamage">基础攻击力</param>
        /// <param name="attackSpeed">攻击速度</param>
        /// <param name="attackRange">攻击范围</param>
        protected Weapon(BaseEntity owner, int id, string name, string description, int iconId, 
                         WeaponType type, int baseDamage, float attackSpeed, float attackRange)
        {
            Owner = owner;
            Id = id;
            Name = name;
            Description = description;
            IconId = iconId;
            Type = type;
            BaseDamage = baseDamage;
            AttackSpeed = attackSpeed;
            AttackRange = attackRange;
            Affixes = new List<WeaponAffix>();
        }

        /// <summary>
        /// 添加词条
        /// </summary>
        /// <param name="affix">词条</param>
        public void AddAffix(WeaponAffix affix)
        {
            if (affix != null && !Affixes.Contains(affix))
            {
                Affixes.Add(affix);
                affix.OnAdded(this);
            }
        }

        /// <summary>
        /// 移除词条
        /// </summary>
        /// <param name="affix">词条</param>
        public void RemoveAffix(WeaponAffix affix)
        {
            if (affix != null && Affixes.Contains(affix))
            {
                Affixes.Remove(affix);
                affix.OnRemoved(this);
            }
        }

        /// <summary>
        /// 尝试攻击
        /// </summary>
        /// <param name="target">目标实体</param>
        /// <returns>是否成功攻击</returns>
        public virtual bool TryAttack(BaseEntity target)
        {
            // 检查攻击冷却
            if (Time.time - _lastAttackTime < 1.0f / AttackSpeed)
            {
                return false;
            }

            // 检查攻击范围
            float distance = Vector3.Distance(Owner.LocalPosition, target.LocalPosition);
            if (distance > AttackRange)
            {
                return false;
            }

            // 执行攻击
            PerformAttack(target);

            // 更新上次攻击时间
            _lastAttackTime = Time.time;

            return true;
        }

        /// <summary>
        /// 执行攻击
        /// </summary>
        /// <param name="target">目标实体</param>
        protected virtual void PerformAttack(BaseEntity target)
        {
            // 计算基础伤害
            int damage = CalculateDamage();

            // 应用词条效果
            foreach (var affix in Affixes)
            {
                damage = affix.ModifyDamage(damage, target);
            }

            // 造成伤害
            target.TakeDamage(damage);

            // 应用词条的攻击后效果
            foreach (var affix in Affixes)
            {
                affix.OnAttackHit(target, damage);
            }
        }

        /// <summary>
        /// 计算伤害
        /// </summary>
        /// <returns>伤害值</returns>
        protected virtual int CalculateDamage()
        {
            // 基础伤害
            int damage = BaseDamage;

            // 如果是怪物实体，加上怪物的攻击力
            if (Owner is MonsterEntity monster)
            {
                damage += monster.AttackPower;
            }

            return damage;
        }
    }

    /// <summary>
    /// 武器类型枚举
    /// </summary>
    public enum WeaponType
    {
        None = 0,
        Sword = 1,      // 剑
        Bow = 2,        // 弓
        Staff = 3,      // 法杖
        Dagger = 4,     // 匕首
        Hammer = 5,     // 锤
        Spear = 6,      // 枪
        Wand = 7,       // 魔杖
        Axe = 8         // 斧
    }
}
