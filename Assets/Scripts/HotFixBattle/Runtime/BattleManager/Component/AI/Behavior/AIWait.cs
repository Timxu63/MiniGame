using System;
using cfg;
using Game.Logic.BattleModule.Component.Weapon;
using Game.Logic.BattleModule.Entity;

namespace Game.Logic.BattleModule.Component.AI
{
    /// <summary>
    /// 等待行为
    /// </summary>
    public class AIWait : AIBehaviorBase
    {
        private readonly float _duration;
        private float _timer;

        public override string Name => "Wait";

        public AIWait(float duration)
        {
            _duration = duration;
        }

        public override AIBehaviorResult Execute(AIComponent ai, float deltaTime)
        {
            _timer += deltaTime;

            if (_timer >= _duration)
            {
                _timer = 0f;
                return AIBehaviorResult.Success;
            }

            return AIBehaviorResult.Running;
        }
    }
}