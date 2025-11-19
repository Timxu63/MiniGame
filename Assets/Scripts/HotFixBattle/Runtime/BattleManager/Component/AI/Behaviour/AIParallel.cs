using Game.Logic.BattleModule.Entity;

namespace Game.Logic.BattleModule.Component.AI
{
    /// <summary>
    /// AI并行行为，同时执行所有子行为
    /// </summary>
    public class AIParallel : AIBehaviorComposite
    {
        public enum Policy
        {
            RequireOne,    // 只需要一个成功
            RequireAll     // 需要全部成功
        }

        private readonly Policy _policy;

        public override string Name => "Parallel";

        public AIParallel(Policy policy = Policy.RequireOne)
        {
            _policy = policy;
        }

        public override AIBehaviorResult Execute(AIComponent ai, float deltaTime)
        {
            int successCount = 0;
            int failureCount = 0;
            int runningCount = 0;

            foreach (var child in Children)
            {
                AIBehaviorResult result = child.Execute(ai, deltaTime);

                switch (result)
                {
                    case AIBehaviorResult.Success:
                        successCount++;
                        break;
                    case AIBehaviorResult.Failure:
                        failureCount++;
                        break;
                    case AIBehaviorResult.Running:
                        runningCount++;
                        break;
                }
            }

            if (_policy == Policy.RequireOne && successCount > 0)
            {
                return AIBehaviorResult.Success;
            }
            else if (_policy == Policy.RequireAll && failureCount == 0 && runningCount == 0)
            {
                return AIBehaviorResult.Success;
            }
            else if (runningCount > 0)
            {
                return AIBehaviorResult.Running;
            }

            return AIBehaviorResult.Failure;
        }
    }
}
