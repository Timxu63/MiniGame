
using System.Collections.Generic;
using cfg;
using Game.Logic.BattleModule;
using Game.Logic.BattleModule.Entity;

namespace HotFixBattle
{
    /// <summary>
    /// 战斗任务控制器
    /// </summary>
    public class BattleMissionController
    {
        private Chapter_Mission _missionData;
        private BattleWorldContext _worldContext;

        private MissionStateBase _currentState;
        private List<int> _spawnedMonsters;
        private float _timer;
        private MissionState _currentStateType;

        public Chapter_Mission MissionData => _missionData;
        public MissionState CurrentStateType => _currentStateType;
        public bool IsCompleted => _currentStateType == MissionState.Completed;
        public float MissionDuration => _missionData.Time;

        public BattleMissionController(Chapter_Mission missionData, BattleWorldContext worldContext)
        {
            _missionData = missionData;
            _worldContext = worldContext;
            _spawnedMonsters = new List<int>();
            _timer = 0f;

            // 设置初始状态
            _currentState = new MissionNotStartedState(this);
            _currentStateType = MissionState.NotStarted;
            _currentState.Enter();
        }

        /// <summary>
        /// 更新任务
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public void Update(float deltaTime)
        {
            _currentState.Update(deltaTime);
        }

        /// <summary>
        /// 改变状态
        /// </summary>
        /// <param name="newState">新状态</param>
        public void ChangeState(MissionStateBase newState)
        {
            _currentState.Exit();
            _currentState = newState;
            _currentStateType = newState.GetStateType();
            _currentState.Enter();
        }

        /// <summary>
        /// 开始任务
        /// </summary>
        public void StartMission()
        {
            if (CurrentStateType == MissionState.NotStarted)
            {
                ChangeState(new MissionWarningState(this));
            }
        }

        /// <summary>
        /// 检查是否可以开始任务
        /// </summary>
        /// <returns>是否可以开始</returns>
        public bool CanStartMission()
        {
            // 可以添加额外的条件检查
            return true;
        }

        /// <summary>
        /// 获取任务持续时间
        /// </summary>
        /// <returns>任务持续时间</returns>
        public float GetMissionDuration()
        {
            return _missionData.Time;
        }

        /// <summary>
        /// 生成怪物
        /// </summary>
        public void SpawnMonsters()
        {
            // 使用实体生成器生成怪物
            var spawnedMonsterIds = EntitySpawner.Instance.SpawnMonstersForMission(_missionData, _worldContext);
            _spawnedMonsters.AddRange(spawnedMonsterIds);

            UnityEngine.Debug.Log($"[BattleMissionController] SpawnMonsters - Mission ID: {_missionData.Id}, 生成了 {spawnedMonsterIds.Count} 个怪物");
        }

        /// <summary>
        /// 更新任务逻辑
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public void UpdateMissionLogic(float deltaTime)
        {
            _timer += deltaTime;
            // 这里可以添加任务特定的更新逻辑
        }

        /// <summary>
        /// 检查是否所有怪物都被消灭
        /// </summary>
        /// <returns>是否所有怪物都被消灭</returns>
        public bool AreAllMonstersDefeated()
        {
            // 使用实体管理器检查所有生成的怪物是否都被消灭
            var entityManager = SimpleEntityManager.Instance;

            foreach (var monsterId in _spawnedMonsters)
            {
                var monster = entityManager.GetEntity(monsterId);
                if (monster != null && monster.IsAlive)
                {
                    return false; // 如果有存活的怪物，返回false
                }
            }

            return true; // 所有怪物都被消灭
        }

        /// <summary>
        /// 清理任务
        /// </summary>
        public void CleanupMission()
        {
            // 使用实体生成器清理怪物
            EntitySpawner.Instance.ClearMonsters(_spawnedMonsters);
            _spawnedMonsters.Clear();

            UnityEngine.Debug.Log($"[BattleMissionController] CleanupMission - Mission ID: {_missionData.Id}, 清理了 {_spawnedMonsters.Count} 个怪物");
        }

        /// <summary>
        /// 任务警告事件
        /// </summary>
        public void OnMissionWarning()
        {
            // 触发任务警告事件，比如显示UI提示等
            // EventSystem.Instance.TriggerEvent(new MissionWarningEvent(_missionData));
        }

        /// <summary>
        /// 任务开始事件
        /// </summary>
        public void OnMissionStart()
        {
            // 触发任务开始事件
            // EventSystem.Instance.TriggerEvent(new MissionStartEvent(_missionData));
        }

        /// <summary>
        /// 任务完成事件
        /// </summary>
        public void OnMissionCompleted()
        {
            // 触发任务完成事件
            // EventSystem.Instance.TriggerEvent(new MissionCompletedEvent(_missionData));
        }
    }
}
