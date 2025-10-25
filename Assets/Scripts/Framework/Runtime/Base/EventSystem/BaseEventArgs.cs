using System;
using System.Collections.Generic;

namespace Framework.EventSystem
{
    /// <summary>
    /// 事件的基类  
    /// </summary>
    public abstract class BaseEventArgs
    {
        public abstract void Clear();

        private EventPool _pool;
        public void SetPool(EventPool pool)
        {
            _pool = pool;
        }
        public void Release()
        {
            if (_pool == null)
                return;
            Clear();
            _pool.Push(this);
        }
    }


    public abstract class EventPool
    {
        public abstract void Push(BaseEventArgs t);
        
        public abstract void Clear();
    }


    public class EventPoolHelper
    {
        private class Pool<T> : EventPool where T : BaseEventArgs, new()
        {
            private Stack<BaseEventArgs> _cacheList;
#if DEBUG
            private HashSet<BaseEventArgs> _checkSet;
            private int _totalCount;
#endif
            public void Init(int initCount)
            {
                _cacheList ??= new Stack<BaseEventArgs>(initCount);
#if DEBUG
                _checkSet ??= new HashSet<BaseEventArgs>();
                _totalCount = 0;
#endif
            }

            public T Get()
            {
                var count = _cacheList.Count;
                T item;
                if (count > 0)
                {
                    item = _cacheList.Pop() as T;
#if DEBUG
                    _checkSet.Remove(item);
#endif
                }
                else
                {
                    item = new T();
                    item.SetPool(this);
#if DEBUG
                    _totalCount++;
#endif
                }

                return item;
            }

            public override void Push(BaseEventArgs t)
            {
                _cacheList.Push(t);
#if DEBUG
                if (!_checkSet.Add(t))
                {
                    Logger.LogError($"Pool Type {typeof(T)} object {t} 重复入池！");
                }
#endif
            }

            public override void Clear()
            {
                Check();
                _cacheList.Clear();
#if DEBUG
                _checkSet.Clear();
#endif
            }

            public void Check()
            {
#if DEBUG
                if (_cacheList.Count != _totalCount)
                {
                    Logger.LogError($"对象池{typeof(T)}有没回收的对象，数量{_totalCount - _cacheList.Count}");
                }
#endif
            }
        }


        private readonly Dictionary<Type, EventPool> _pools = new();

        public T Get<T>() where T : BaseEventArgs, new()
        {
            var pool = GetPool<T>(typeof(T));
            return pool.Get();
        }

        public void Push<T>(T t) where T : BaseEventArgs, new()
        {
            var pool = GetPool<T>(typeof(T));
            pool.Push(t);
        }

        public void Clear<T>() where T : BaseEventArgs, new()
        {
            var pool = GetPool<T>(typeof(T));
            pool.Clear();
        }

        public void ClearAllPool()
        {
            foreach (var pool in _pools.Values)
            {
                pool.Clear();
            }

            _pools.Clear();
        }

        private Pool<T> GetPool<T>(Type tType) where T : BaseEventArgs, new()
        {
            if (_pools.TryGetValue(tType, out var pool))
            {
                return pool as Pool<T>;
            }

            var newPool = new Pool<T>();
            newPool.Init(PoolItemMaxCount);
            _pools[tType] = newPool;
            return newPool;
        }

        private const int PoolItemMaxCount = 4; //默认4的池大小，每次扩张一倍。
    }
}