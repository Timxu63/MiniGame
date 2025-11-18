using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Framework.Runtime;
using Framework.EventSystem;
using cfg;
using Framework.Logic.Modules;
using UnityEngine.SceneManagement;

namespace HotFixBattle
{
    /// <summary>
    /// 战斗资源预加载管理器（基础版）
    /// 负责预加载战斗场景所需的各种资源
    /// </summary>
    public class BattleResourcePreloaderBasic
    {
        // 预加载进度事件
        public event Action<float> OnProgressChanged;

        // 预加载完成事件
        public event Action OnPreloadCompleted;

        // 预加载状态
        private bool _isPreloading = false;

        // 预加载进度
        private float _progress = 0f;

        // 资源加载任务列表
        private List<Func<Task>> _preloadTasks = new List<Func<Task>>();

        // 当前章节ID
        private int _currentChapterId = -1;
        private BattleWorldContext _worldContext;

        // 私有构造函数，实现单例模式
        public BattleResourcePreloaderBasic(BattleWorldContext worldContext)
        {
            _worldContext = worldContext;
        }

        /// <summary>
        /// 设置当前章节ID并更新预加载任务
        /// </summary>
        /// <param name="chapterId">章节ID</param>
        public void SetCurrentChapter(int chapterId)
        {
            if (_currentChapterId != chapterId)
            {
                _currentChapterId = chapterId;
                UpdatePreloadTasksForChapter(chapterId);
            }
        }

        /// <summary>
        /// 根据章节ID更新预加载任务
        /// </summary>
        /// <param name="chapterId">章节ID</param>
        private void UpdatePreloadTasksForChapter(int chapterId)
        {
            // 清空任务列表
            _preloadTasks.Clear();

            // 获取章节数据
            var tables = _worldContext.Tables;
            if (tables == null || tables.TbChapter == null)
            {
                Logger.LogWarning("无法获取章节数据，使用默认预加载任务");
                return;
            }

            var chapter = tables.TbChapter.Get(chapterId);
            if (chapter == null)
            {
                Logger.LogWarning($"未找到章节ID {chapterId}，使用默认预加载任务");
                return;
            }
            // 添加角色预加载任务
            _preloadTasks.Add(PreloadDefaultCharacterModels);

            // 添加怪物预加载任务（预加载所有任务的怪物）
            var allMonsterIds = new HashSet<int>();
            if (chapter.WaveGroups != null)
            {
                foreach (var waveId in chapter.WaveGroups)
                {
                    var wave = tables.TbChapterWaveGroup.Get(waveId);
                    if (wave != null && wave.WaveIds != null)
                    {
                        foreach (var missionId in wave.WaveIds)
                        {
                            var missionData = tables.TbChapterMission.Get(missionId);
                            if (missionData != null && missionData.MonsterId != null)
                            {
                                foreach (var monsterId in missionData.MonsterId)
                                {
                                    allMonsterIds.Add(monsterId);
                                }
                            }
                        }
                    }
                }
            }

            // 为每个怪物ID添加预加载任务
            foreach (var monsterId in allMonsterIds)
            {
                int id = monsterId; // 避免闭包问题
                _preloadTasks.Add(() => PreloadEntityModel(id));
            }

            Logger.Log($"已为章节 {chapter.Id} 添加 {allMonsterIds.Count} 个怪物预加载任务");

            // 添加UI预加载任务
            _preloadTasks.Add(PreloadDefaultUIElements);
            //
            // // 添加音频预加载任务
            // _preloadTasks.Add(PreloadDefaultAudioAssets);

            // 添加特效预加载任务
            _preloadTasks.Add(PreloadDefaultEffectAssets);

            // 添加环境预加载任务
            _preloadTasks.Add(PreloadDefaultEnvironmentAssets);

            Logger.Log($"已为章节 {chapterId} 更新预加载任务，总计 {_preloadTasks.Count} 个任务");
        }

        private async Task PreloadDefaultUIElements()
        {
            await Task.Delay(0); // 模拟加载时间
        }

        /// <summary>
        /// 开始预加载
        /// </summary>
        public async void StartPreloadAsync()
        {
            if (_isPreloading)
            {
                Logger.LogWarning("战斗资源预加载已在进行中");
                return;
            }

            _isPreloading = true;
            _progress = 0f;

            try
            {
                Logger.Log("开始预加载战斗资源");

                float taskWeight = 1f / _preloadTasks.Count;

                // 并行执行所有预加载任务
                var taskResults = new Task[_preloadTasks.Count];
                for (int i = 0; i < _preloadTasks.Count; i++)
                {
                    int taskIndex = i;
                    taskResults[i] = ExecuteTaskWithProgressTracking(_preloadTasks[taskIndex], taskIndex, taskWeight);
                }

                // 等待所有任务完成
                await Task.WhenAll(taskResults);

                _progress = 1f;
                OnProgressChanged?.Invoke(_progress);

                Logger.Log("战斗资源预加载完成");
                OnPreloadCompleted?.Invoke();
            }
            catch (Exception ex)
            {
                Logger.LogError($"战斗资源预加载失败: {ex.Message}");
            }
            finally
            {
                _isPreloading = false;
            }
        }

        /// <summary>
        /// 执行任务并跟踪进度
        /// </summary>
        private async Task ExecuteTaskWithProgressTracking(Func<Task> task, int taskIndex, float taskWeight)
        {
            try
            {
                await task();
            }
            catch (Exception ex)
            {
                Logger.LogError($"预加载任务 {taskIndex} 失败: {ex.Message}");
            }

            // 更新进度
            _progress = Math.Min(1f, _progress + taskWeight);
            OnProgressChanged?.Invoke(_progress);
        }
        /// <summary>
        /// 预加载默认角色模型
        /// </summary>
        private async Task PreloadDefaultCharacterModels()
        {
            Logger.Log("预加载默认角色模型");
            await Task.Delay(400); // 模拟加载时间
            Logger.Log("默认角色模型预加载完成");
        }

        /// <summary>
        /// 预加载默认怪物模型
        /// </summary>
        private async Task PreloadDefaultMonsterModels()
        {
            Logger.Log("预加载默认怪物模型");
            await Task.Delay(500); // 模拟加载时间
            Logger.Log("默认怪物模型预加载完成");
        }

        /// <summary>
        /// 预加载指定怪物模型
        /// </summary>
        private async Task PreloadEntityModel(int entityId)
        {
            Charactor charactor = _worldContext.Tables.TbCharactor.Get(entityId);
            if (charactor == null)
            {
                return;
            }
            Resource resourceInfo = _worldContext.Tables.TbResource.Get(charactor.ResourceID);
            await AssetsPoolManager.Instance.PrepareAsset<GameObject>(resourceInfo.Path);
        }


        /// <summary>
        /// 预加载默认音频资源
        /// </summary>
        private async Task PreloadDefaultAudioAssets()
        {
            Logger.Log("预加载默认音频资源");
            await Task.Delay(200); // 模拟加载时间
            Logger.Log("默认音频资源预加载完成");
        }

        /// <summary>
        /// 预加载默认特效资源
        /// </summary>
        private async Task PreloadDefaultEffectAssets()
        {
            Logger.Log("预加载默认特效资源");
            await Task.Delay(300); // 模拟加载时间
            Logger.Log("默认特效资源预加载完成");
        }

        /// <summary>
        /// 预加载默认环境资源
        /// </summary>
        private async Task PreloadDefaultEnvironmentAssets()
        {
            Logger.Log("预加载默认环境资源");
            await Task.Delay(250); // 模拟加载时间
            Logger.Log("默认环境资源预加载完成");
        }

        /// <summary>
        /// 取消预加载
        /// </summary>
        public void CancelPreload()
        {
            if (_isPreloading)
            {
                Logger.Log("取消战斗资源预加载");
                _isPreloading = false;
                // 这里可以添加取消逻辑
            }
        }

        /// <summary>
        /// 重置预加载状态
        /// </summary>
        public void Reset()
        {
            _isPreloading = false;
            _progress = 0f;
        }
    }
}
