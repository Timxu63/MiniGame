

using System;
using UnityEngine;
using Luban;

namespace cfg
{
    /// <summary>
    /// AI配置类，通过解析Chapter表的AIParam字段获取AI参数
    /// </summary>
    public sealed partial class AIConfig : Luban.BeanBase
    {
        public AIConfig(ByteBuf _buf)
        {
            // AIConfig不从二进制数据创建，而是从JSON字符串创建
            // 这个构造函数保留是为了兼容性
        }

        /// <summary>
        /// 从JSON字符串创建AI配置
        /// </summary>
        /// <param name="jsonString">JSON字符串</param>
        public AIConfig(string jsonString)
        {
            try
            {
                // 解析JSON字符串
                var jsonData = JsonUtility.FromJson<AIParamData>(jsonString);

                // 将解析的数据赋值给属性
                AIType = jsonData.aiType;
                MoveSpeed = jsonData.moveSpeed;
                DetectionRange = jsonData.detectionRange;
                AttackRange = jsonData.attackRange;
                AttackCooldown = jsonData.attackCooldown;
                PatrolRadius = jsonData.patrolRadius;
                FleeThreshold = jsonData.fleeThreshold;
            }
            catch (Exception e)
            {
                Debug.LogError($"[AIConfig] 解析AI参数失败: {e.Message}");

                // 使用默认值
                AIType = eAIType.Static;
                MoveSpeed = 2.0f;
                DetectionRange = 5.0f;
                AttackRange = 1.0f;
                AttackCooldown = 1.0f;
                PatrolRadius = 3.0f;
                FleeThreshold = 0.3f;
            }
        }

        /// <summary>
        /// AI类型
        /// </summary>
        public eAIType AIType { get; private set; }

        /// <summary>
        /// 移动速度
        /// </summary>
        public float MoveSpeed { get; private set; }

        /// <summary>
        /// 检测范围
        /// </summary>
        public float DetectionRange { get; private set; }

        /// <summary>
        /// 攻击范围
        /// </summary>
        public float AttackRange { get; private set; }

        /// <summary>
        /// 攻击冷却时间
        /// </summary>
        public float AttackCooldown { get; private set; }

        /// <summary>
        /// 巡逻半径
        /// </summary>
        public float PatrolRadius { get; private set; }

        /// <summary>
        /// 逃跑阈值（生命值百分比）
        /// </summary>
        public float FleeThreshold { get; private set; }

        public const int __ID__ = -123456789; // 实际ID应由Luban生成
        public override int GetTypeId() => __ID__;

        public void ResolveRef(Tables tables)
        {
        }

        public override string ToString()
        {
            return "{ "
            + "aiType:" + AIType + ","
            + "moveSpeed:" + MoveSpeed + ","
            + "detectionRange:" + DetectionRange + ","
            + "attackRange:" + AttackRange + ","
            + "attackCooldown:" + AttackCooldown + ","
            + "patrolRadius:" + PatrolRadius + ","
            + "fleeThreshold:" + FleeThreshold + ","
            + "}";
        }
    }

    /// <summary>
    /// AI参数数据结构，用于JSON反序列化
    /// </summary>
    [System.Serializable]
    public class AIParamData
    {
        public eAIType aiType;
        public float moveSpeed;
        public float detectionRange;
        public float attackRange;
        public float attackCooldown;
        public float patrolRadius;
        public float fleeThreshold;
    }

    /// <summary>
    /// AI类型枚举
    /// </summary>
    public enum eAIType
    {
        /// <summary>
        /// 静止不动
        /// </summary>
        Static,

        /// <summary>
        /// 随机移动
        /// </summary>
        Random,

        /// <summary>
        /// 巡逻
        /// </summary>
        Patrol,

        /// <summary>
        /// 追踪玩家
        /// </summary>
        Chase,

        /// <summary>
        /// 攻击型
        /// </summary>
        Aggressive,

        /// <summary>
        /// 胆小型（生命值低时会逃跑）
        /// </summary>
        Coward,

        /// <summary>
        /// 守卫型（在一定范围内巡逻，发现敌人会追击）
        /// </summary>
        Guardian
    }
}

