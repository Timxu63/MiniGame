
using UnityEngine;
using cfg;
using Game.Logic.BattleModule.Entity;
using Framework.EventSystem;
using Framework.Runtime;

namespace HotFixBattle.AI
{
    /// <summary>
    /// 静止状态
    /// </summary>
    public class StaticState : IAIState
    {
        public string Name => "Static";

        public void Enter(AIContext context)
        {
            // 静止状态不需要特殊处理
        }

        public void Update(AIContext context, float deltaTime)
        {
            // 静止状态不移动
        }

        public void Exit(AIContext context)
        {
            // 静止状态不需要特殊处理
        }
    }

    /// <summary>
    /// 随机移动状态
    /// </summary>
    public class RandomMoveState : IAIState
    {
        public string Name => "RandomMove";

        private float _nextChangeDirectionTime;
        private Vector3 _randomDirection;

        public void Enter(AIContext context)
        {
            ChangeDirection(context);
        }

        public void Update(AIContext context, float deltaTime)
        {
            if (Time.time >= _nextChangeDirectionTime)
            {
                ChangeDirection(context);
            }

            // 发送移动事件
            Vector3 currentPosition = context.Entity.GetScreenPosition();
            Vector3 newPosition = currentPosition + _randomDirection * context.Config.MoveSpeed * deltaTime;
            // 更新实体位置
            context.Entity.SetPosition(newPosition);
            GameApp.Event.DispatchNow((int)LocalMessageName.CC_EntityMove, new EntityMoveEventArgs(context.Entity.Id, newPosition));
        }

        public void Exit(AIContext context)
        {
            // 随机移动状态不需要特殊处理
        }

        private void ChangeDirection(AIContext context)
        {
            _randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
            _nextChangeDirectionTime = Time.time + Random.Range(1f, 3f);
        }
    }

    /// <summary>
    /// 巡逻状态
    /// </summary>
    public class PatrolState : IAIState
    {
        public string Name => "Patrol";

        private Vector3 _patrolCenter;
        private float _nextPatrolPointTime;
        private Vector3 _currentPatrolPoint;

        public void Enter(AIContext context)
        {
            _patrolCenter = context.InitialPosition;
            SetNewPatrolPoint(context);
        }

        public void Update(AIContext context, float deltaTime)
        {
            // 检查是否到达巡逻点
            Vector3 currentPosition = context.Entity.GetScreenPosition();
            if (Vector3.Distance(currentPosition, _currentPatrolPoint) < 0.5f || 
                Time.time >= _nextPatrolPointTime)
            {
                SetNewPatrolPoint(context);
            }

            // 向巡逻点移动
            Vector3 direction = (_currentPatrolPoint - currentPosition).normalized;
            Vector3 newPosition = currentPosition + direction * context.Config.MoveSpeed * deltaTime;
            // 更新实体位置
            context.Entity.SetPosition(newPosition);
            GameApp.Event.DispatchNow((int)LocalMessageName.CC_EntityMove, new EntityMoveEventArgs(context.Entity.Id, newPosition));
        }

        public void Exit(AIContext context)
        {
            // 巡逻状态不需要特殊处理
        }

        private void SetNewPatrolPoint(AIContext context)
        {
            float angle = Random.Range(0f, 2f * Mathf.PI);
            float radius = Random.Range(0f, context.Config.PatrolRadius);
            _currentPatrolPoint = _patrolCenter + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            _nextPatrolPointTime = Time.time + Random.Range(5f, 10f);
        }
    }

    /// <summary>
    /// 追逐状态
    /// </summary>
    public class ChaseState : IAIState
    {
        public string Name => "Chase";

        public void Enter(AIContext context)
        {
            // 追逐状态不需要特殊处理
        }

        public void Update(AIContext context, float deltaTime)
        {
            if (context.Target == null)
                return;

            // 向目标移动
            Vector3 currentPosition = context.Entity.GetScreenPosition();
            Vector3 targetPosition = context.Target.GetScreenPosition();
            Vector3 direction = (targetPosition - currentPosition).normalized;
            Vector3 newPosition = currentPosition + direction * context.Config.MoveSpeed * deltaTime;
            // 更新实体位置
            context.Entity.SetPosition(newPosition);
            GameApp.Event.DispatchNow((int)LocalMessageName.CC_EntityMove, new EntityMoveEventArgs(context.Entity.Id, newPosition));

            // 检查是否在攻击范围内
            if (Vector3.Distance(currentPosition, targetPosition) <= context.Config.AttackRange)
            {
                // 切换到攻击状态
                context.CurrentState = new AttackState();
                context.CurrentState.Enter(context);
            }
        }

        public void Exit(AIContext context)
        {
            // 追逐状态不需要特殊处理
        }
    }

    /// <summary>
    /// 攻击状态
    /// </summary>
    public class AttackState : IAIState
    {
        public string Name => "Attack";

        public void Enter(AIContext context)
        {
            // 攻击状态不需要特殊处理
        }

        public void Update(AIContext context, float deltaTime)
        {
            if (context.Target == null)
                return;

            // 检查目标是否还在攻击范围内
            Vector3 currentPosition = context.Entity.GetScreenPosition();
            Vector3 targetPosition = context.Target.GetScreenPosition();
            if (Vector3.Distance(currentPosition, targetPosition) > context.Config.AttackRange)
            {
                // 切换回追逐状态
                context.CurrentState = new ChaseState();
                context.CurrentState.Enter(context);
                return;
            }

            // 如果可以攻击，则执行攻击
            if (context.CanAttack)
            {
                context.LastAttackTime = Time.time;
                // 发送攻击事件
                GameApp.Event.DispatchNow((int)LocalMessageName.CC_EntityAttack, new EntityAttackEventArgs(context.Entity.Id, context.Target.Id));
            }
        }

        public void Exit(AIContext context)
        {
            // 攻击状态不需要特殊处理
        }
    }

    /// <summary>
    /// 逃跑状态
    /// </summary>
    public class FleeState : IAIState
    {
        public string Name => "Flee";

        public void Enter(AIContext context)
        {
            // 逃跑状态不需要特殊处理
        }

        public void Update(AIContext context, float deltaTime)
        {
            if (context.Target == null)
                return;

            // 远离目标移动
            Vector3 currentPosition = context.Entity.GetScreenPosition();
            Vector3 targetPosition = context.Target.GetScreenPosition();
            Vector3 direction = (currentPosition - targetPosition).normalized;
            Vector3 newPosition = currentPosition + direction * context.Config.MoveSpeed * deltaTime;
            // 更新实体位置
            context.Entity.SetPosition(newPosition);
            GameApp.Event.DispatchNow((int)LocalMessageName.CC_EntityMove, new EntityMoveEventArgs(context.Entity.Id, newPosition));

            // 如果距离目标足够远，切换到随机移动状态
            if (Vector3.Distance(currentPosition, targetPosition) > context.Config.DetectionRange * 1.5f)
            {
                context.CurrentState = new RandomMoveState();
                context.CurrentState.Enter(context);
            }
        }

        public void Exit(AIContext context)
        {
            // 逃跑状态不需要特殊处理
        }
    }

    /// <summary>
    /// 守卫状态
    /// </summary>
    public class GuardianState : IAIState
    {
        public string Name => "Guardian";

        private PatrolState _patrolState;
        private ChaseState _chaseState;
        private AttackState _attackState;

        public void Enter(AIContext context)
        {
            _patrolState = new PatrolState();
            _chaseState = new ChaseState();
            _attackState = new AttackState();

            _patrolState.Enter(context);
        }

        public void Update(AIContext context, float deltaTime)
        {
            // 检查是否有目标
            if (context.Target == null)
            {
                // 没有目标，执行巡逻
                _patrolState.Update(context, deltaTime);
                return;
            }

            // 检查目标是否在检测范围内
            Vector3 currentPosition = context.Entity.GetScreenPosition();
            Vector3 targetPosition = context.Target.GetScreenPosition();
            float distanceToTarget = Vector3.Distance(currentPosition, targetPosition);
            if (distanceToTarget <= context.Config.DetectionRange)
            {
                // 目标在检测范围内
                if (distanceToTarget <= context.Config.AttackRange)
                {
                    // 在攻击范围内，执行攻击
                    _attackState.Update(context, deltaTime);
                }
                else
                {
                    // 在检测范围内但不在攻击范围内，执行追逐
                    _chaseState.Update(context, deltaTime);
                }
            }
            else
            {
                // 目标超出检测范围，继续巡逻
                _patrolState.Update(context, deltaTime);
                context.Target = null;
            }
        }

        public void Exit(AIContext context)
        {
            // 守卫状态不需要特殊处理
        }
    }
}
