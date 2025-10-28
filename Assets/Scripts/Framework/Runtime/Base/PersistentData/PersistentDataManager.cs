using System.Collections.Generic;
using UnityEngine;

namespace Framework.PersistentData
{
    public class PersistentDataManager : MonoBehaviour
    {
        private const float LocalSaveInterval = 0.1f;//本地存档间隔时间 
        
        private readonly Dictionary<string, PersistentDataBase> _localSaveDic = new Dictionary<string, PersistentDataBase>();

        private float _tickTime;

        public void OnInit()
        {
            GameDbMgr.Instance.Init();
            _tickTime = 0f;
        }

        private ulong _userId;
        public void SetUserId(ulong userId)
        {
            _userId = userId;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_userId == 0)
                return;
            _tickTime += deltaTime;
            if (_tickTime > LocalSaveInterval)
            {
                _tickTime = 0f;
                foreach (var kv in _localSaveDic)
                {
                    var data = kv.Value;
                    if (data.IsDirty)
                    {
                        data.SaveData();
                    }
                }
            }
        }

        private void AddData(string dataName, PersistentDataBase data)
        {
            if (data == null)
            {
                Logger.LogError($"{GetType()}.RegisterData data is null! id:{dataName}");
                return;
            }

            if (!_localSaveDic.TryAdd(dataName, data))
            {
                Logger.LogError($"{GetType()}.RegisterData id is already exist! id:{dataName}");
            }
        }

        public T GetData<T>(string dataName) where T : PersistentDataBase
        {
            if (_localSaveDic.TryGetValue(dataName, out var data))
            {
                return data as T;
            }
            return default;
        }

        /// <summary>
        ///  切换账号
        /// </summary>
        public void Reset()
        {
            GameDbMgr.Instance.DropAllCollection();
            OnDeInit();
        }

        public void OnDeInit()
        {
            _localSaveDic.Clear();
        }

        public void SaveAll()
        {
            foreach (var kv in _localSaveDic)
            {
                if (kv.Value.IsDirty)
                {
                    kv.Value.SaveData();    
                }
            }
        }
        
        /// <summary>
        ///     当需要本地存档时使用这个方法注册
        /// </summary>
        public T RegisterLocalData<T>(string collectionName) where T : PersistentDataBase, new()
        {
            if (_userId == 0)
            {
                Logger.LogError($"{typeof(T)}. 注册时机不对，未设置UserId");
                return null;
            }
            
            var dbInstance = GameDbMgr.Instance;
            T data;
            if (dbInstance.ExistData(collectionName, _userId))
            {
                data = dbInstance.GetData<T>(collectionName,_userId);
                data.OnInit();
            }
            else
            {
                data = new T();
                data.OnCreate();
                data.Dirty();
            }
            data.UserId = _userId;
            
            AddData(collectionName, data);
            return data;
        }

        /// <summary>
        /// 清空本地数据引用的时候 移除注册
        /// </summary>
        /// <param name="collectionName"></param>
        public void RemoveLocalData(string collectionName)
        {
            if (_localSaveDic.ContainsKey(collectionName))
            {
                _localSaveDic.Remove(collectionName);
            }
        }
    }
}