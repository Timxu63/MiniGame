namespace HotFixBattle.AI
{
    /// <summary>
    /// AI状态枚举
    /// </summary>
    public enum AIStateType
    {
        Patrol,     //巡逻
        Chase,      //追逐
        Attack,     //攻击
        Flee,       //逃跑
    }

    /// <summary>
    /// AI行为枚举
    /// </summary>
    public enum AIBehaviorType
    {
        MoveToTarget,           //移动到目标
        RandomMove,             //随机移动
        Wait,                   //等待
        Attack,                 //攻击
        AIFindTarget,           //寻找目标
        // AggressiveMoveToTarget, //主动移动到目标
        // SmartMoveToTarget,      //智能移动到目标
        // AggressiveChase,        //主动追逐
        // FlickerMove,            //闪烁移动
        // PersistentChase,        //持续追逐
        // KitingAttack,           //风筝攻击
        // GroupAttack,            //群体攻击
        // PowerAttack             //强力攻击
    }

    /// <summary>
    /// AI决策规则枚举
    /// </summary>
    public enum AIDecisionRuleType
    {
        LowHealthFlee,
        AttackTargetInRange,
        MoveToTarget,
        PatrolWhenNoTarget,
        DefaultIdle,
        // AggressiveChase,
        // PersistentChase,
        // KitingAttack,
        // GroupAttack,
        // AggressiveMoveToTarget,
        // SmartMoveToTarget,
        // FlickerMove,
        // PowerAttack
    }
}