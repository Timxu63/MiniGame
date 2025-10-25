using System;
using UnityEngine;

namespace Framework
{
    public class RuntimeManager : MonoBehaviour
    {
        public BaseRunTimeModel m_model;
        public Action m_onFinished = null;
        
        /// <summary>
        /// 加载
        /// </summary>
        public void Load()
        {
            m_model = new Runtime_DoHotFix();
            m_model.m_onFinished = m_onFinished;
            m_model.Load();
        }
    }
}