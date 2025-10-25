using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.EventSystem
{
    public delegate void HandlerEvent(int type, BaseEventArgs eventArgs = null);

    public class EventSystemManager : MonoBehaviour
    {
        private readonly Dictionary<int, List<HandlerEvent>> _handles = new Dictionary<int, List<HandlerEvent>>();
        private readonly EventPoolHelper _eventPoolHelper = new EventPoolHelper();
        private readonly DispatchDataList _dispatchDataList = new DispatchDataList();

        private void Awake()
        {
            InitBucket();
        }

        public T GetEvent<T>() where T : BaseEventArgs, new()
        {
            return _eventPoolHelper.Get<T>();
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="handle"></param>
        public void RegisterEvent(int type, HandlerEvent handle)
        {
            _handles.TryGetValue(type, out var handles);
            if (handles != null)
            {
                handles.Add(handle);
            }
            else
            {
                handles = new List<HandlerEvent> {handle};
                _handles.Add(type, handles);
            }
        }

        /// <summary>
        /// 取消事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="handle"></param>
        public void UnRegisterEvent(int type, HandlerEvent handle)
        {
            _handles.TryGetValue(type, out var handles);
            handles?.Remove(handle);
        }

        /// <summary>
        /// 卸载所有的显示模块模块
        /// </summary>
        /// <param name="ignoreIDs">忽略的ID</param>
        public void UnRegisterAllEvent()
        {
            _handles.Clear();
            _dispatchDataList.Clear();
        }

        /// <summary>
        /// 立即发送事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="type"></param>
        /// <param name="eventArgs"></param>
        public void DispatchNow(int type, BaseEventArgs eventArgs = null)
        {
            _handles.TryGetValue(type, out var handles);
            if (handles != null)
            {
                var count = handles.Count;
                if (count > 0)
                {
                    var tmpHandles = GetTmpHandleEvent(count);
                    handles.CopyTo(tmpHandles);
                    for (int i = 0; i < count; i++)
                    {
                        tmpHandles[i]?.Invoke(type, eventArgs);
                    }

                    eventArgs?.Release();
                    ReturnTmpHandles(tmpHandles);
                }
            }
        }

        // private List<HandlerEvent> _handlerEventsHelper = new List<HandlerEvent>();

        /// <summary>
        /// 当Update时发送事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="type"></param>
        /// <param name="eventArgs"></param>
        public void Dispatch(int type, BaseEventArgs eventArgs = null)
        {
            _dispatchDataList.AddData(type, eventArgs);
        }

        /// <summary>
        /// Update 运行
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="unscaledDeltaTime"></param>
        public void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            var count = _dispatchDataList.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    ref var curData = ref _dispatchDataList.Data[i];
                    DispatchNow(curData.EventType, curData.EventArgs);
                }

                _dispatchDataList.Clear();
            }
        }

        private void InitBucket()
        {
            for (int i = 0; i < _buckets.Length; i++)
            {
                var maxSize = GetMaxSizeForBucket(i);
                _buckets[i] = new Bucket(maxSize);
            }
        }


        private readonly Bucket[] _buckets = new Bucket[10]; //最大的buff 是4096,同时监听超过4096个消息的话，那就是有问题了

        private HandlerEvent[] GetTmpHandleEvent(int count)
        {
            var index = SelectBucketIndex(count);
            HandlerEvent[] buffer;
            if (index < _buckets.Length)
            {
                const int maxBucketsToTry = 2;
                int i = index;
                do
                {
                    buffer = _buckets[i].Rent();
                    if (buffer != null)
                    {
                        return buffer;
                    }
                } while (++i < _buckets.Length && i != index + maxBucketsToTry);

                buffer = new HandlerEvent[_buckets[index].BuffSize];
            }
            else
            {
                buffer = new HandlerEvent[count];
#if UNITY_EDITOR
                Logger.LogError("HandlerEvent  buffer  is too large  " + count);
#endif
            }

            return buffer;
        }


        private void ReturnTmpHandles(HandlerEvent[] array)
        {
            int bucket = SelectBucketIndex(array.Length);
            bool haveBucket = bucket < _buckets.Length;
            if (haveBucket)
            {
                Array.Clear(array, 0, array.Length);
                _buckets[bucket].Return(array);
            }
        }

        private static int SelectBucketIndex(int bufferSize)
        {
            return Log2((bufferSize - 1) | 7) - 2;
        }

        private static int GetMaxSizeForBucket(int binIndex)
        {
            int maxSize = 8 << binIndex;
            return maxSize;
        }

        private static int Log2(int n)
        {
            int log = 0;
            if (n >= 1 << 16) { n >>= 16; log += 16; }
            if (n >= 1 << 8) { n >>= 8; log += 8; }
            if (n >= 1 << 4) { n >>= 4; log += 4; }
            if (n >= 1 << 2) { n >>= 2; log += 2; }
            if (n >= 1 << 1) { log += 1; }

            return log;
        }

        private class Bucket
        {
            private const int BuffLen = 16; //同长度的Buff最多16个，应该够了，我们嵌套发送消息的情况应该不会太多
            internal readonly int BuffSize;
            private readonly HandlerEvent[][] _buffers;
            private int _index;

            internal Bucket(int buffSize)
            {
                //HLog.LogError("Bucket buffSize " + buffSize);
                BuffSize = buffSize;
                _buffers = new HandlerEvent[BuffLen][];
            }

            internal HandlerEvent[] Rent()
            {
                var buffers = _buffers;
                HandlerEvent[] buffer = null;
                bool allocateBuffer = false;

                if (_index < buffers.Length)
                {
                    buffer = buffers[_index];
                    buffers[_index++] = null;
                    allocateBuffer = buffer == null;
                }

                if (allocateBuffer)
                {
                    buffer = new HandlerEvent[BuffSize];
                }

                return buffer;
            }

            internal void Return(HandlerEvent[] array)
            {
#if UNITY_EDITOR
                if (array.Length != BuffSize)
                {
                    Logger.LogError("Return buffer size is not match " + array.Length + " " + BuffSize);
                    return;
                }
#endif
                var returned = _index != 0;
                if (returned)
                {
                    _buffers[--_index] = array;
                }
            }
        }
    }
}