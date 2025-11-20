using System;
using cfg;
using Game.Logic.BattleModule.Component.Weapon;
using Game.Logic.BattleModule.Entity;

namespace HotFixBattle.AI
{
    /// <summary>
    /// 随机移动行为
    /// </summary>
    public class AIRandomMove : AIBehaviorBase
    {
        private readonly float _duration;
        private float _timer;
        private UnityEngine.Vector2 _direction;

        public override string Name => "RandomMove";

        public AIRandomMove(float duration = 1.0f)
        {
            _duration = duration;
        }

        public override AIBehaviorResult Execute(AIComponent ai, float deltaTime)
        {
            _timer += deltaTime;

            // 如果计时器超过持续时间，重新计算方向
            if (_timer >= _duration)
            {
                _timer = 0f;
                // 生成随机方向
                float angle = UnityEngine.Random.Range(0f, 2f * UnityEngine.Mathf.PI);
                _direction = new UnityEngine.Vector2(UnityEngine.Mathf.Cos(angle), UnityEngine.Mathf.Sin(angle));
            }

            // 移动
            ai.Owner.Move(_direction);

            return AIBehaviorResult.Running;
        }
    }
}