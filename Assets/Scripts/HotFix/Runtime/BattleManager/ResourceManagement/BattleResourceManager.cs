using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Framework.Runtime;
using Framework.EventSystem;
using cfg;
using Framework;
using HotFix;

namespace HotFixBattle
{
    /// <summary>
    /// 战斗资源管理器
    /// 负责管理战斗中的所有资源，包括特效、实体等
    /// 所有资源加载都通过此管理器进行
    /// </summary>
    public class BattleResourceManager : Singleton<BattleResourceManager>
    {
        // 资源缓存
        private readonly Dictionary<string, GameObject> _gameObjectCache = new Dictionary<string, GameObject>();
        private readonly Dictionary<string, Material> _materialCache = new Dictionary<string, Material>();
        private readonly Dictionary<string, Texture> _textureCache = new Dictionary<string, Texture>();
        private readonly Dictionary<string, AudioClip> _audioClipCache = new Dictionary<string, AudioClip>();
        private readonly Dictionary<string, AnimationClip> _animationClipCache = new Dictionary<string, AnimationClip>();

        // 资源引用计数
        private readonly Dictionary<string, int> _resourceReferenceCount = new Dictionary<string, int>();

        // 预加载状态
        private bool _isPreloading = false;
        private float _preloadProgress = 0f;

        // 预加载进度事件
        public event Action<float> OnPreloadProgressChanged;
        public event Action OnPreloadCompleted;

        /// <summary>
        /// 初始化资源管理器
        /// </summary>
        public void Initialize()
        {
            Logger.Log("初始化战斗资源管理器");
        }

        /// <summary>
        /// 清理资源管理器
        /// </summary>
        public void Cleanup()
        {
            Logger.Log("清理战斗资源管理器");
            // 清理所有缓存
            ClearAllCaches();
        }

        /// <summary>
        /// 预加载完成事件处理
        /// </summary>
        public void OnPreloadCompletedHandler()
        {
            Logger.Log("开始缓存预加载的资源");

            // 从预加载器获取资源并缓存
            CachePreloadedResources();
        }

        /// <summary>
        /// 缓存预加载的资源
        /// </summary>
        private async void CachePreloadedResources()
        {
            try
            {
                // 获取当前章节的所有资源
                var tables = GameTableProxy.Tables;
                if (tables?.TbResource == null)
                {
                    Logger.LogWarning("无法获取资源配置表");
                    return;
                }

                // 获取当前战斗数据
                var battleDataModule = GameApp.DataModule.GetDataModule<BattleDataModule>((int)DataName.BattleDataModule);
                if (battleDataModule?.m_openBattleData?.ModeData == null)
                {
                    Logger.LogWarning("无法获取战斗数据");
                    return;
                }

                int chapterId = battleDataModule.m_openBattleData.ModeData.ChapterId;
                var chapter = tables.TbChapter.Get(chapterId);
                if (chapter == null)
                {
                    Logger.LogWarning($"未找到章节 {chapterId}");
                    return;
                }

                // 收集需要预加载的资源路径
                var resourcePaths = new HashSet<string>();

                // 添加场景资源
                AddSceneResources(resourcePaths);

                // 添加角色资源
                AddCharacterResources(resourcePaths);

                // 添加怪物资源（所有任务的怪物）
                AddMonsterResources(chapter, resourcePaths);

                // 添加UI资源
                AddUIResources(resourcePaths);

                // 添加音频资源
                AddAudioResources(resourcePaths);

                // 添加特效资源
                AddEffectResources(resourcePaths);

                // 添加环境资源
                AddEnvironmentResources(resourcePaths);

                // 预加载所有资源
                await PreloadResources(resourcePaths);

                Logger.Log($"完成缓存预加载资源，共 {resourcePaths.Count} 个资源");
            }
            catch (Exception ex)
            {
                Logger.LogError($"缓存预加载资源失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 添加场景资源
        /// </summary>
        private void AddSceneResources(HashSet<string> resourcePaths)
        {
            
        }

        /// <summary>
        /// 添加角色资源
        /// </summary>
        private void AddCharacterResources(HashSet<string> resourcePaths)
        {
            // 添加玩家角色资源
            var playerConfig = GameTableProxy.Tables.TbCharactor.Get(1); // 默认玩家ID为1
            if (playerConfig != null)
            {
                var resourceConfig = GameTableProxy.Tables.TbResource.Get(playerConfig.ResourceID);
                if (resourceConfig != null)
                {
                    resourcePaths.Add(resourceConfig.Path);
                }
            }
        }

        /// <summary>
        /// 添加怪物资源
        /// </summary>
        private void AddMonsterResources(Chapter chapter, HashSet<string> resourcePaths)
        {
            if (chapter == null) return;

            // 收集所有怪物ID
            var monsterIds = new HashSet<int>();
            var waveGroups = chapter.WaveGroups;
            for (int i = 0; i < waveGroups.Length; i++)
            {
                int waveId = waveGroups[i];
                
            }
            foreach (var waveGroupId in chapter.WaveGroups)
            {
                var chapterWave = GameTableProxy.Tables.TbChapterWaveGroup.Get(waveGroupId);
                if (chapterWave != null && chapterWave.WaveIds != null)
                {
                    var missionIds = chapterWave.WaveIds;
                    for (int i = 0; i < missionIds.Length; i++)
                    {
                        int missionId = missionIds[i];
                        var mission = GameTableProxy.Tables.TbChapterMission.Get(missionId);
                        if (mission != null && mission.MonsterId != null)
                        {
                            foreach (var monsterId in mission.MonsterId)
                            {
                                monsterIds.Add(monsterId);
                            }
                        }
                    }
                }
            }

            // 添加所有怪物资源
            foreach (var monsterId in monsterIds)
            {
                var monsterConfig = GameTableProxy.Tables.TbCharactor.Get(monsterId);
                if (monsterConfig != null)
                {
                    var resourceConfig = GameTableProxy.Tables.TbResource.Get(monsterConfig.ResourceID);
                    if (resourceConfig != null)
                    {
                        resourcePaths.Add(resourceConfig.Path);
                    }
                }
            }
        }

        /// <summary>
        /// 添加UI资源
        /// </summary>
        private void AddUIResources(HashSet<string> resourcePaths)
        {
            // 添加战斗UI资源
            
        }

        /// <summary>
        /// 添加音频资源
        /// </summary>
        private void AddAudioResources(HashSet<string> resourcePaths)
        {
            
        }

        /// <summary>
        /// 添加特效资源
        /// </summary>
        private void AddEffectResources(HashSet<string> resourcePaths)
        {
           
        }

        /// <summary>
        /// 添加环境资源
        /// </summary>
        private void AddEnvironmentResources(HashSet<string> resourcePaths)
        {
            // 添加环境资源
            
        }

        /// <summary>
        /// 预加载资源
        /// </summary>
        private async UniTask PreloadResources(HashSet<string> resourcePaths)
        {
            _isPreloading = true;
            _preloadProgress = 0f;

            int totalResources = resourcePaths.Count;
            int loadedResources = 0;

            var tasks = new List<UniTask>();

            foreach (var path in resourcePaths)
            {
                tasks.Add(LoadAndCacheResource(path));
            }

            // 等待所有资源加载完成
            await UniTask.WhenAll(tasks);

            _isPreloading = false;
            _preloadProgress = 1f;
            OnPreloadProgressChanged?.Invoke(_preloadProgress);
            OnPreloadCompleted?.Invoke();
        }

        /// <summary>
        /// 加载并缓存资源
        /// </summary>
        private async UniTask LoadAndCacheResource(string path)
        {
            try
            {
                // 检查是否已经缓存
                if (IsResourceCached(path))
                {
                    IncrementReferenceCount(path);
                    return;
                }

                // 根据文件扩展名确定资源类型
                var extension = System.IO.Path.GetExtension(path).ToLower();

                switch (extension)
                {
                    case ".prefab":
                        await LoadAndCacheGameObject(path);
                        break;
                    case ".mat":
                        await LoadAndCacheMaterial(path);
                        break;
                    case ".png":
                    case ".jpg":
                        await LoadAndCacheTexture(path);
                        break;
                    case ".wav":
                    case ".mp3":
                        await LoadAndCacheAudioClip(path);
                        break;
                    case ".anim":
                        await LoadAndCacheAnimationClip(path);
                        break;
                    default:
                        // 尝试作为GameObject加载
                        await LoadAndCacheGameObject(path);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"加载资源失败: {path}, 错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 加载并缓存GameObject
        /// </summary>
        private async UniTask LoadAndCacheGameObject(string path)
        {
            var loadOperation = Addressables.LoadAssetAsync<GameObject>(path);
            await loadOperation.Task;

            if (loadOperation.Status == AsyncOperationStatus.Succeeded)
            {
                _gameObjectCache[path] = loadOperation.Result;
                _resourceReferenceCount[path] = 1;
            }
        }

        /// <summary>
        /// 加载并缓存Material
        /// </summary>
        private async UniTask LoadAndCacheMaterial(string path)
        {
            var loadOperation = Addressables.LoadAssetAsync<Material>(path);
            await loadOperation.Task;

            if (loadOperation.Status == AsyncOperationStatus.Succeeded)
            {
                _materialCache[path] = loadOperation.Result;
                _resourceReferenceCount[path] = 1;
            }
        }

        /// <summary>
        /// 加载并缓存Texture
        /// </summary>
        private async UniTask LoadAndCacheTexture(string path)
        {
            var loadOperation = Addressables.LoadAssetAsync<Texture>(path);
            await loadOperation.Task;

            if (loadOperation.Status == AsyncOperationStatus.Succeeded)
            {
                _textureCache[path] = loadOperation.Result;
                _resourceReferenceCount[path] = 1;
            }
        }

        /// <summary>
        /// 加载并缓存AudioClip
        /// </summary>
        private async UniTask LoadAndCacheAudioClip(string path)
        {
            var loadOperation = Addressables.LoadAssetAsync<AudioClip>(path);
            await loadOperation.Task;

            if (loadOperation.Status == AsyncOperationStatus.Succeeded)
            {
                _audioClipCache[path] = loadOperation.Result;
                _resourceReferenceCount[path] = 1;
            }
        }

        /// <summary>
        /// 加载并缓存AnimationClip
        /// </summary>
        private async UniTask LoadAndCacheAnimationClip(string path)
        {
            var loadOperation = Addressables.LoadAssetAsync<AnimationClip>(path);
            await loadOperation.Task;

            if (loadOperation.Status == AsyncOperationStatus.Succeeded)
            {
                _animationClipCache[path] = loadOperation.Result;
                _resourceReferenceCount[path] = 1;
            }
        }

        /// <summary>
        /// 检查资源是否已缓存
        /// </summary>
        private bool IsResourceCached(string path)
        {
            return _gameObjectCache.ContainsKey(path) ||
                   _materialCache.ContainsKey(path) ||
                   _textureCache.ContainsKey(path) ||
                   _audioClipCache.ContainsKey(path) ||
                   _animationClipCache.ContainsKey(path);
        }

        /// <summary>
        /// 增加资源引用计数
        /// </summary>
        private void IncrementReferenceCount(string path)
        {
            if (_resourceReferenceCount.ContainsKey(path))
            {
                _resourceReferenceCount[path]++;
            }
            else
            {
                _resourceReferenceCount[path] = 1;
            }
        }

        /// <summary>
        /// 减少资源引用计数
        /// </summary>
        private void DecrementReferenceCount(string path)
        {
            if (_resourceReferenceCount.ContainsKey(path))
            {
                _resourceReferenceCount[path]--;

                // 如果引用计数为0，释放资源  战斗中不释放
                // if (_resourceReferenceCount[path] <= 0)
                // {
                //     ReleaseResource(path);
                // }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        private void ReleaseResource(string path)
        {
            // 从相应的缓存中移除并释放资源
            if (_gameObjectCache.ContainsKey(path))
            {
                Addressables.Release(_gameObjectCache[path]);
                _gameObjectCache.Remove(path);
            }
            else if (_materialCache.ContainsKey(path))
            {
                Addressables.Release(_materialCache[path]);
                _materialCache.Remove(path);
            }
            else if (_textureCache.ContainsKey(path))
            {
                Addressables.Release(_textureCache[path]);
                _textureCache.Remove(path);
            }
            else if (_audioClipCache.ContainsKey(path))
            {
                Addressables.Release(_audioClipCache[path]);
                _audioClipCache.Remove(path);
            }
            else if (_animationClipCache.ContainsKey(path))
            {
                Addressables.Release(_animationClipCache[path]);
                _animationClipCache.Remove(path);
            }

            // 移除引用计数
            _resourceReferenceCount.Remove(path);

            Logger.Log($"已释放资源: {path}");
        }

        /// <summary>
        /// 清理所有缓存
        /// </summary>
        private void ClearAllCaches()
        {
            // 释放所有GameObject
            foreach (var kvp in _gameObjectCache)
            {
                Addressables.Release(kvp.Value);
            }
            _gameObjectCache.Clear();

            // 释放所有Material
            foreach (var kvp in _materialCache)
            {
                Addressables.Release(kvp.Value);
            }
            _materialCache.Clear();

            // 释放所有Texture
            foreach (var kvp in _textureCache)
            {
                Addressables.Release(kvp.Value);
            }
            _textureCache.Clear();

            // 释放所有AudioClip
            foreach (var kvp in _audioClipCache)
            {
                Addressables.Release(kvp.Value);
            }
            _audioClipCache.Clear();

            // 释放所有AnimationClip
            foreach (var kvp in _animationClipCache)
            {
                Addressables.Release(kvp.Value);
            }
            _animationClipCache.Clear();

            // 清理引用计数
            _resourceReferenceCount.Clear();

            Logger.Log("已清理所有资源缓存");
        }

        #region 公共API

        /// <summary>
        /// 获取GameObject
        /// </summary>
        public GameObject GetGameObject(string path)
        {
            if (_gameObjectCache.TryGetValue(path, out var gameObject))
            {
                IncrementReferenceCount(path);
                return gameObject;
            }

            // 如果缓存中没有，异步加载
            LoadAndCacheResource(path).Forget();
            return null;
        }

        /// <summary>
        /// 获取Material
        /// </summary>
        public Material GetMaterial(string path)
        {
            if (_materialCache.TryGetValue(path, out var material))
            {
                IncrementReferenceCount(path);
                return material;
            }

            // 如果缓存中没有，异步加载
            LoadAndCacheResource(path).Forget();
            return null;
        }

        /// <summary>
        /// 获取Texture
        /// </summary>
        public Texture GetTexture(string path)
        {
            if (_textureCache.TryGetValue(path, out var texture))
            {
                IncrementReferenceCount(path);
                return texture;
            }

            // 如果缓存中没有，异步加载
            LoadAndCacheResource(path).Forget();
            return null;
        }

        /// <summary>
        /// 获取AudioClip
        /// </summary>
        public AudioClip GetAudioClip(string path)
        {
            if (_audioClipCache.TryGetValue(path, out var audioClip))
            {
                IncrementReferenceCount(path);
                return audioClip;
            }

            // 如果缓存中没有，异步加载
            LoadAndCacheResource(path).Forget();
            return null;
        }

        /// <summary>
        /// 获取AnimationClip
        /// </summary>
        public AnimationClip GetAnimationClip(string path)
        {
            if (_animationClipCache.TryGetValue(path, out var animationClip))
            {
                IncrementReferenceCount(path);
                return animationClip;
            }

            // 如果缓存中没有，异步加载
            LoadAndCacheResource(path).Forget();
            return null;
        }

        /// <summary>
        /// 释放资源引用
        /// </summary>
        public void ReleaseResourceRef(string path)
        {
            DecrementReferenceCount(path);
        }

        /// <summary>
        /// 实例化GameObject
        /// </summary>
        public GameObject InstantiateGameObject(string path, Transform parent = null)
        {
            var prefab = GetGameObject(path);
            if (prefab != null)
            {
                var instance = GameObject.Instantiate(prefab);
                if (parent != null)
                {
                    instance.transform.SetParent(parent);
                }
                return instance;
            }

            return null;
        }

        /// <summary>
        /// 异步实例化GameObject
        /// </summary>
        public async UniTask<GameObject> InstantiateGameObjectAsync(string path, Transform parent = null)
        {
            var prefab = GetGameObject(path);
            if (prefab != null)
            {
                var instance = await UniTask.RunOnThreadPool(() => GameObject.Instantiate(prefab));

                await UniTask.SwitchToMainThread();

                if (parent != null)
                {
                    instance.transform.SetParent(parent);
                }

                return instance;
            }

            // 如果缓存中没有，先加载
            await LoadAndCacheResource(path);

            // 再次尝试获取
            prefab = GetGameObject(path);
            if (prefab != null)
            {
                var instance = await UniTask.RunOnThreadPool(() => GameObject.Instantiate(prefab));

                await UniTask.SwitchToMainThread();

                if (parent != null)
                {
                    instance.transform.SetParent(parent);
                }

                return instance;
            }

            return null;
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        public void PlaySoundEffect(string path, Vector3 position)
        {
            var audioClip = GetAudioClip(path);
            if (audioClip != null)
            {
                // 在指定位置播放音效
                AudioSource.PlayClipAtPoint(audioClip, position);
            }
        }

        /// <summary>
        /// 创建特效
        /// </summary>
        public GameObject CreateEffect(string path, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var effectPrefab = GetGameObject(path);
            if (effectPrefab != null)
            {
                var effect = GameObject.Instantiate(effectPrefab, position, rotation);
                if (parent != null)
                {
                    effect.transform.SetParent(parent);
                }

                // 如果特效有自动销毁组件，不需要手动处理
                // 否则，可能需要手动管理特效生命周期

                return effect;
            }

            return null;
        }

        /// <summary>
        /// 异步创建特效
        /// </summary>
        public async UniTask<GameObject> CreateEffectAsync(string path, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var effectPrefab = GetGameObject(path);
            if (effectPrefab != null)
            {
                var effect = await UniTask.RunOnThreadPool(() => GameObject.Instantiate(effectPrefab, position, rotation));

                await UniTask.SwitchToMainThread();

                if (parent != null)
                {
                    effect.transform.SetParent(parent);
                }

                return effect;
            }

            // 如果缓存中没有，先加载
            await LoadAndCacheResource(path);

            // 再次尝试创建
            effectPrefab = GetGameObject(path);
            if (effectPrefab != null)
            {
                var effect = await UniTask.RunOnThreadPool(() => GameObject.Instantiate(effectPrefab, position, rotation));

                await UniTask.SwitchToMainThread();

                if (parent != null)
                {
                    effect.transform.SetParent(parent);
                }

                return effect;
            }

            return null;
        }

        #endregion
    }
}
