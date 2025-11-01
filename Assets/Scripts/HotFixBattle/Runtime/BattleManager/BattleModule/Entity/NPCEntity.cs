
using System;
using cfg;

namespace Game.Logic.BattleModule.Entity
{
    /// <summary>
    /// NPC实体类
    /// </summary>
    public class NPCEntity : BaseEntity
    {
        /// <summary>
        /// 对话内容
        /// </summary>
        public string[] Dialogues { get; set; }

        /// <summary>
        /// 商店物品ID列表（如果是商人）
        /// </summary>
        public int[] ShopItemIds { get; set; }

        /// <summary>
        /// 是否是商人
        /// </summary>
        public bool IsMerchant { get; set; }

        /// <summary>
        /// NPC功能类型
        /// </summary>
        public NPCFunctionType FunctionType { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">NPC名称</param>
        /// <param name="maxHealth">最大生命值</param>
        public NPCEntity(string name, int maxHealth)
            : base(name, eEntityType.NPC, maxHealth)
        {
            Dialogues = new string[0];
            ShopItemIds = new int[0];
            IsMerchant = false;
            FunctionType = NPCFunctionType.Talker;
        }

        /// <summary>
        /// 获取随机对话
        /// </summary>
        /// <returns>对话内容</returns>
        public string GetRandomDialogue()
        {
            if (Dialogues == null || Dialogues.Length == 0)
            {
                return "你好，旅行者！";
            }

            Random random = new Random();
            int index = random.Next(Dialogues.Length);
            return Dialogues[index];
        }

        /// <summary>
        /// 重写死亡事件，NPC通常不会死亡
        /// </summary>
        protected override void OnDeath()
        {
            // NPC通常不会死亡，而是进入不可交互状态
            // 可以在这里添加特殊处理逻辑
        }
    }

    /// <summary>
    /// NPC功能类型枚举
    /// </summary>
    public enum NPCFunctionType
    {
        Talker,      // 对话NPC
        Merchant,    // 商人
        QuestGiver,  // 任务发布者
        Blacksmith,  // 铁匠
        Healer,      // 治疗师
        Trainer      // 训练师
    }
}
