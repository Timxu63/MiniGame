using System.Collections.Generic;
using System.Linq;
using Framework.EventSystem;
using UnityEngine;

namespace Framework.DataModule
{
    public class DataModuleManager : MonoBehaviour
    {
        private Dictionary<int, IDataModule> m_dataModules = new Dictionary<int, IDataModule>();
        [SerializeField] private EventSystemManager m_eventSystemManager = null;

        public void Clear()
        {
            foreach (var dataModule in m_dataModules.Values)
            {
                dataModule.Clear();
            }
        }
        
        /// <summary>
        /// 注册数据模块
        /// </summary>
        /// <param name="dataModule"></param>
        public void RegisterDataModule(IDataModule dataModule)
        {
            if (dataModule != null)
            {
                dataModule.RegisterEvents(m_eventSystemManager);
                m_dataModules[dataModule.GetName()] = dataModule;
            }
        }

        /// <summary>
        /// 取消注册数据模块
        /// </summary>
        /// <param name="dataModule"></param>
        public void UnRegisterDataModule(IDataModule dataModule)
        {
            if (dataModule != null)
            {
                dataModule.UnRegisterEvents(m_eventSystemManager);
                m_dataModules.Remove(dataModule.GetName());
            }
        }

        /// <summary>
        /// 卸载所有的数据模块
        /// </summary>
        /// <param name="ignoreIDs">忽略的id</param>
        public void UnRegisterAllDataModule(params int[] ignoreIDs)
        {
            List<IDataModule> unRegisters = new List<IDataModule>();
            foreach (var item in m_dataModules)
            {
                if (item.Value == null) continue;
                if (ignoreIDs != null && ignoreIDs.Contains(item.Key)) continue;
                unRegisters.Add(item.Value);
            }

            for (int i = 0; i < unRegisters.Count; i++)
            {
                var data = unRegisters[i];
                if (data == null) continue;
                UnRegisterDataModule(data);
            }
        }

        /// <summary>
        /// 通过id获得数据模块
        /// </summary>
        /// <param name="dataName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetDataModule<T>(int dataName) where T : IDataModule
        {
            if (m_dataModules.TryGetValue(dataName, out var module))
            {
                return (T)module;
            }

            return  default;
        }
    }
}