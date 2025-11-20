using System;
using System.Collections.Generic;

namespace HotFixBattle.AI
{
    /// <summary>
    /// AI行为组加载器 - 初始化部分
    /// </summary>
    public static partial class AIBehaviorGroupLoader
    {
        /// <summary>
        /// 初始化默认AI行为组
        /// </summary>
        private static void InitializeDefaultBehaviorGroups()
        {
            // 使用各行为组配置类创建行为组
            _behaviorGroups["NormalMonster"] = NormalMonsterBehaviorGroup.Create();
            _behaviorGroups["EliteMonster"] = EliteMonsterBehaviorGroup.Create();
            _behaviorGroups["Boss"] = BossBehaviorGroup.Create();
            _behaviorGroups["FastMonster"] = FastMonsterBehaviorGroup.Create();
            _behaviorGroups["PatientMonster"] = PatientMonsterBehaviorGroup.Create();
            _behaviorGroups["KitingMonster"] = KitingMonsterBehaviorGroup.Create();
            _behaviorGroups["GroupMonster"] = GroupMonsterBehaviorGroup.Create();
        }

        // 各个行为组的初始化方法已移至单独的配置类中
    }
}