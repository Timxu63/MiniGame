using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UltraLiteDB;
using UnityEngine;

namespace Framework.PersistentData
{
    /// <summary>
    ///  本地数据库管理
    /// </summary>
    public class GameDbMgr : Singleton<GameDbMgr>
    {
        [RuntimeInitializeOnLoadMethod]
        private static void RunOnStart()
        {
            Application.quitting += Quit;
        }

        private static void Quit()
        {
            Instance._requireQuit = true;
        }

        //加密Key
        private const string EncryptKey = "MKM0Xjw1FLVkz9A1v2Oxud2m9uK9Nux";
        private readonly object _writeLocker = new object();
        private static string _localPath = string.Empty;
        private static string _filesDir;
        private static string _cacheDir;
        private static string _externalFilesDir;
        private static string _externalCacheDir;

        private bool _init;
        private bool _requireQuit;

        private UltraLiteDatabase _database;
        private Dictionary<string, UltraLiteCollection<BsonDocument>> _cacheCollections;
        private Queue<GameDbOpCmd> _cmdQueue;
        private Thread _writeThread;

        private static string DBPath
        {
            get
            {
                if (string.IsNullOrEmpty(_localPath))
                {
                    _localPath = $"{GetDataFolder()}/DB.bytes";
                }

                return _localPath;
            }
        }

        private static string GetDataFolder()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_filesDir == null) {
            try {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                    using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
                        using (AndroidJavaObject filesDir = currentActivity.Call<AndroidJavaObject>("getFilesDir")) {
                            _filesDir = filesDir.Call<string>("getCanonicalPath");
                        }

                        using (AndroidJavaObject cacheDir = currentActivity.Call<AndroidJavaObject>("getCacheDir")) {
                            _cacheDir = cacheDir.Call<string>("getCanonicalPath");
                        }

                        using (AndroidJavaObject externalFilesDir = currentActivity.Call<AndroidJavaObject>("getExternalFilesDir", (object)null)) {
                            _externalFilesDir = externalFilesDir.Call<string>("getCanonicalPath");
                        }

                        using (AndroidJavaObject externalCacheDir = currentActivity.Call<AndroidJavaObject>("getExternalCacheDir")) {
                            _externalCacheDir = externalCacheDir.Call<string>("getCanonicalPath");
                        }
                    }
                }
            } catch (System.Exception ex) {
                _filesDir = Application.persistentDataPath;
                Debug.LogException(ex);
            }
        }
        return _filesDir;
#else
            return Application.persistentDataPath;
#endif
        }

        public void Init()
        {
            if (_init)
            {
                return;
            }

            _init = true;
            _requireQuit = false;

            var connectString = new ConnectionString
            {
                Filename = DBPath,
                Password = EncryptKey
            };
            _database = new UltraLiteDatabase(connectString);
            _cacheCollections = new Dictionary<string, UltraLiteCollection<BsonDocument>>();
            _cmdQueue = new Queue<GameDbOpCmd>();

            StartWriteThread();
        }

        private void WaitOpQueueEmpty()
        {
            while (true)
            {
                var count = 0;
                lock (_writeLocker)
                {
                    count = _cmdQueue.Count;
                }

                if (count <= 0)
                {
                    break;
                }

                Thread.Sleep(1);
            }
        }

        private UltraLiteCollection<BsonDocument> GetCollection(string colName)
        {
            if (!_cacheCollections.TryGetValue(colName, out var collection))
            {
                collection = _database.GetCollection<BsonDocument>(colName);
                _cacheCollections.Add(colName, collection);
            }

            return collection;
        }

        public T GetData<T>(string colName, ulong id)
        {
            if (!_init)
            {
                throw new Exception($"{GetType()}.GetData game db is not init now!");
            }

            // 仅处理极端边界情况,一般不会卡主
            WaitOpQueueEmpty();

            BsonDocument bsonDoc;
            lock (_writeLocker)
            {
                var collection = GetCollection(colName);
                bsonDoc = collection.FindById(id);
            }

            if (bsonDoc == null)
            {
                Logger.LogError($"{GetType()}.GetData collection do not have the data find by id={id} col={colName}");
                return default;
            }

            var data = BsonMapper.Global.ToObject<T>(bsonDoc);
            return data;
        }

        public bool ExistData(string colName, ulong id)
        {
            if (!_init)
            {
                throw new Exception($"{GetType()}.ExistData game db is not init now!");
            }

            // 仅处理极端边界情况,一般不会卡主
            WaitOpQueueEmpty();

            var query = Query.EQ("_id", id);

            lock (_writeLocker)
            {
                var collection = GetCollection(colName);
                return collection.Exists(query);
            }
        }

        public void SaveData(Type type, object data, string colName, ulong id)
        {
            if (!_init)
            {
                throw new Exception($"{GetType()}.SaveData game db is not init now!");
            }

            var doc = BsonMapper.Global.ToDocument(type, data);
            var opCmd = new GameDbOpCmd()
            {
                op = GameDbOp.UpOrInsert,
                colName = colName,
                id = id,
                doc = doc
            };
            lock (_writeLocker)
            {
                _cmdQueue.Enqueue(opCmd);
            }
        }

        public void SaveData<T>(T data, string colName, ulong id)
        {
            if (!_init)
            {
                throw new Exception($"{GetType()}.SaveData game db is not init now!");
            }

            var doc = BsonMapper.Global.ToDocument(data);
            var opCmd = new GameDbOpCmd()
            {
                op = GameDbOp.UpOrInsert,
                colName = colName,
                id = id,
                doc = doc
            };
            lock (_writeLocker)
            {
                _cmdQueue.Enqueue(opCmd);
            }
        }

        private void StartWriteThread()
        {
            _writeThread = new Thread(delegate()
            {
                while (true)
                {
                    lock (_writeLocker)
                    {
                        if (_cmdQueue.Count > 0)
                        {
                            var cmd = _cmdQueue.Dequeue();

                            Logger.Log($"FileMgr.SaveFile {cmd.colName}");

                            var collection = GetCollection(cmd.colName);
                            if (cmd.op == GameDbOp.UpOrInsert)
                            {
                                collection.Upsert(cmd.id, cmd.doc);
                            }
                            else if (cmd.op == GameDbOp.Insert)
                            {
                                collection.Insert(cmd.id, cmd.doc);
                            }
                            else if (cmd.op == GameDbOp.Delete)
                            {
                                collection.Delete(cmd.id);
                            }
                            else if (cmd.op == GameDbOp.Update)
                            {
                                collection.Update(cmd.id, cmd.doc);
                            }
                            else if (cmd.op == GameDbOp.None)
                            {
                                // wtf??
                                Debug.LogError("cmd.op = none");
                            }
                        }
                        else
                        {
                            if (_requireQuit)
                            {
                                _cacheCollections.Clear();
                                _database.Dispose();
                                break;
                            }
                        }
                    }

                    Thread.Sleep(300);
                }
            });
            _writeThread.IsBackground = true;
            _writeThread.Start();
        }

        private void ExitWriteThread()
        {
            _requireQuit = true;
            _writeThread.Join();
            if (_writeThread.ThreadState != ThreadState.Stopped)
            {
                Logger.LogError("ExitWriteThread wtf!");
            }

            _writeThread = null;
        }

        /// <summary>
        /// 清空数据库
        /// </summary>
        public void DropAllCollection()
        {
            Logger.Log("FileMgr.Reset");
            ExitWriteThread();
            var collectionNames = _database.GetCollectionNames().ToArray();
            foreach (var collectionName in collectionNames)
            {
                _database.DropCollection(collectionName);
            }

            StartWriteThread();
        }
    }
}