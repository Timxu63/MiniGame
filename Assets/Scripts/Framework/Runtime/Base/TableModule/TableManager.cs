using System;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class TableManager : MonoBehaviour
    {
        private ITableManager m_manager;

        /// <summary>
        /// 设置表格管理器
        /// </summary>
        /// <param name="manager"></param>
        public void SetITableManager(ITableManager manager)
        {
            m_manager = manager;
        }
        
        public T GetTable<T>() where T : class
        {
            return m_manager.GetTable<T>();
        }
       
        /// <summary>
        /// 获得表格管理器
        /// </summary>
        /// <returns></returns>
        public ITableManager GetITableManager()
        {
            return m_manager;
        }
    }
}