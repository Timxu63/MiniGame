using Game.Logic.BattleModule.Entity;

namespace HotFixBattle.AI
{
    /// <summary>
    /// AI行为序列，依次执行子行为，直到有一个失败或全部成功
    /// </summary>
    public class AISequence : AIBehaviorComposite
    {
        private int _currentChild = 0;

        public override string Name => "Sequence";

        public override AIBehaviorResult Execute(AIComponent ai, float deltaTime)
        {
            while (_currentChild < Children.Count)
            {
                AIBehaviorResult result = Children[_currentChild].Execute(ai, deltaTime);

                if (result == AIBehaviorResult.Failure)
                {
                    _currentChild = 0;
                    return AIBehaviorResult.Failure;
                }
                else if (result == AIBehaviorResult.Running)
                {
                    return AIBehaviorResult.Running;
                }

                _currentChild++;
            }

            _currentChild = 0;
            return AIBehaviorResult.Success;
        }
    }
}
