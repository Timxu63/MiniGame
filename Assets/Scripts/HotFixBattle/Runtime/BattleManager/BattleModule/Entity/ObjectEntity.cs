
using System;
using cfg;

namespace Game.Logic.BattleModule.Entity
{
    /// <summary>
    /// 物体实体类
    /// </summary>
    public class ObjectEntity : BaseEntity
    {
        /// <summary>
        /// 物体类型
        /// </summary>
        public ObjectType ObjectType { get; set; }

        /// <summary>
        /// 是否可交互
        /// </summary>
        public bool IsInteractable { get; set; }

        /// <summary>
        /// 是否可破坏
        /// </summary>
        public bool IsDestructible { get; set; }

        /// <summary>
        /// 交互所需条件
        /// </summary>
        public string InteractionRequirement { get; set; }

        /// <summary>
        /// 破坏后掉落物ID
        /// </summary>
        public int[] DropItemIds { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">物体名称</param>
        /// <param name="maxHealth">最大生命值</param>
        public ObjectEntity(string name, int maxHealth)
            : base(name, eEntityType.Object, maxHealth)
        {
            ObjectType = ObjectType.Static;
            IsInteractable = false;
            IsDestructible = false;
            InteractionRequirement = null;
            DropItemIds = new int[0];
        }

        /// <summary>
        /// 交互
        /// </summary>
        /// <param name="interactor">交互者</param>
        /// <returns>交互结果</returns>
        public virtual string Interact(IEntity interactor)
        {
            if (!IsInteractable)
            {
                return $"这个{ObjectType}无法交互";
            }

            // 检查交互条件
            if (!string.IsNullOrEmpty(InteractionRequirement))
            {
                // 这里可以添加更复杂的条件检查逻辑
                // 例如：检查玩家等级、物品等
                return $"你需要{InteractionRequirement}才能与这个{ObjectType}交互";
            }

            // 根据物体类型执行不同的交互逻辑
            switch (ObjectType)
            {
                case ObjectType.Chest:
                    return "你打开了宝箱，获得了丰厚的奖励！";

                case ObjectType.Door:
                    return "你打开了门，发现了新的道路！";

                case ObjectType.Lever:
                    return "你拉下了拉杆，机关启动了！";

                default:
                    return $"你与{ObjectType}进行了交互";
            }
        }

        /// <summary>
        /// 重写受到伤害方法，只有可破坏的物体才能受到伤害
        /// </summary>
        /// <param name="damage">原始伤害值</param>
        /// <returns>实际受到的伤害</returns>
        public override int TakeDamage(int damage)
        {
            if (!IsDestructible)
            {
                return 0; // 不可破坏的物体不会受到伤害
            }

            return base.TakeDamage(damage);
        }

        /// <summary>
        /// 重写死亡事件，处理物体被破坏后的逻辑
        /// </summary>
        protected override void OnDeath()
        {
            base.OnDeath();

            // 物体被破坏后的处理
            // 例如：掉落物品、触发机关等
        }
    }

    /// <summary>
    /// 物体类型枚举
    /// </summary>
    public enum ObjectType
    {
        Static,  // 静态装饰物
        Chest,   // 宝箱
        Door,    // 门
        Lever,   // 拉杆
        Barrel,  // 木桶
        Crystal, // 水晶
        Statue   // 雕像
    }
}
