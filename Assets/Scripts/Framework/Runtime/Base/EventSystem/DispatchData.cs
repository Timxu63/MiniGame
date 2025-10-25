using System;

namespace Framework.EventSystem
{
    public struct DispatchData
    {
        public int EventType;
        public BaseEventArgs EventArgs;
    }

    public class DispatchDataList
    {
        private const int DefaultSize = 4;
        public DispatchData[] Data = new DispatchData[DefaultSize];
        private int _count;
        private int _size = DefaultSize;
        public int Count => _count;
        public void AddData(int type, BaseEventArgs eventArgs)
        {
            if (_count >= _size)
            {
                _size <<= 1;
                var newData = new DispatchData[_size];
                Array.Copy(newData, Data, _count);
                Data = newData;
            }
            ref var curData = ref Data[_count];
            curData.EventType = type;
            curData.EventArgs = eventArgs;
            _count++;
        }
        
        public void Clear()
        {
            _count = 0;
        }
    }
}
