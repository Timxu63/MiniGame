using System;
using System.Collections.Generic;
using cfg;
using Framework;
using Luban;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace HotFix
{
    public class GameTableProxy : ITableManager
    {
        private static Tables _tables;
        public static Tables Tables
        {
            get
            {
                return _tables;
            }
        }
        private Action _onFinished;
        private int count;
        public void InitialiseLocalModels(Action callBack)
        {
            // 找到所有名为 LocalModel 的 Label 的资源位置
            Addressables.LoadResourceLocationsAsync("LocalModel", typeof(TextAsset))
                .Completed += OnLocationsLoaded;
            _onFinished = callBack;
        }
        void OnLocationsLoaded(AsyncOperationHandle<IList<IResourceLocation>> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                count = handle.Result.Count;
                foreach (var location in handle.Result)
                {
                    Debug.Log($"资源名: {location.PrimaryKey}, 地址: {location.InternalId}");
                    // 如果要加载这个资源
                    Addressables.LoadAssetAsync<TextAsset>(location).Completed += async assetHandle =>
                    {
                        await AssetsPoolManager.Instance.PrepareAsset<TextAsset>(location.PrimaryKey);
                        Debug.Log($"已加载: {assetHandle.Result.name}");
                        LoadFinish();
                    };
                }
            }
            else
            {
                Debug.LogError("加载资源位置失败");
            }
            
            
        }

        private void LoadFinish()
        {
            count--;
            if (count == 0)
            {
                // 创建 Tables，并用缓存读取二进制
                _tables = new Tables(fileName =>
                {
                    string path = $"Assets/_Resources/LocalModel/{fileName}.bytes";
                    if (AssetsPoolManager.Instance.GetAsset(path, out var data))
                    {
                        return new ByteBuf(((TextAsset)data.Result).bytes);
                    }
                    throw new Exception($"[LubanConfig] 配置文件未找到: {path}");
                });
                _onFinished?.Invoke();
            }
        }
        
        public void DeInitialiseLocalModels()
        {
            //这里在AssetsPoolManager释放时全部释放
        }
        
        /// <summary>
        /// 获取指定配置表
        /// </summary>
        public T GetTable<T>() where T : class
        {
            var table = _tables.GetType().GetProperty(typeof(T).Name)?.GetValue(_tables);
            if (table == null)
                throw new Exception($"[LubanConfig] 未找到表: {typeof(T).Name}");
            return table as T;
        }
    }
}