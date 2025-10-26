using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITableManager
    {
        void InitialiseLocalModels(Action callBack);
        void DeInitialiseLocalModels();
        public T GetTable<T>() where T : class;
    }
}