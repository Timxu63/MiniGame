using Game.Logic.BattleModule.Entity;

namespace HotFixBattle.AI
{
    /// <summary>
    /// AI行为选择器，依次执行子行为，直到有一个成功或全部失败
    /// </summary>
    public class AISelector : AIBehaviorComposite
    {
        private int _currentChild = 0;

        public override string Name => "Selector";

        public override AIBehaviorResult Execute(AIComponent ai, float deltaTime)
        {
            while (_currentChild < Children.Count)
            {
                AIBehaviorResult result = Children[_currentChild].Execute(ai, deltaTime);

                if (result == AIBehaviorResult.Success)
                {
                    _currentChild = 0;
                    return AIBehaviorResult.Success;
                }
                else if (result == AIBehaviorResult.Running)
                {
                    return AIBehaviorResult.Running;
                }

                _currentChild++;
            }

            _currentChild = 0;
            return AIBehaviorResult.Failure;
        }
    }
}
