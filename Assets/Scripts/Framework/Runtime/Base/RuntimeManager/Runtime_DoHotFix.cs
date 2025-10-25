
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework
{
    public class Runtime_DoHotFix : BaseRunTimeModel
    {
        private System.Reflection.Assembly m_hotAssembly;
        private object m_mainClass;
        private Type m_appType;
        private MethodInfo m_onStarUp;
        private MethodInfo m_onShutDown;

        private MethodInfo m_applicationFocus;
        private MethodInfo m_onApplicationPause;
        private MethodInfo m_onApplicationQuit;
        private MethodInfo m_getLanguageInfoByID;
        public override async void Load()
        {
            await LoadAssembly();

            OnStarUp();
        }
        
        public override void OnStarUp()
        {
            if (m_onStarUp == null) return;
            m_onStarUp.Invoke(m_mainClass, null);
        }
        private async UniTask LoadAssembly()
        {
            Logger.Log("Do LoadAssembly");
            m_hotAssembly = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "HotFix");
            
            if (m_hotAssembly == null)
            {
                Logger.LogError("[HyBridCLRManager] dll 未加载");
            }

            m_mainClass = m_hotAssembly.CreateInstance("HotFix.Main");
            m_appType = m_hotAssembly.GetType("HotFix.Main");

            m_onStarUp = m_appType.GetMethod("OnStarUp");
            m_onShutDown = m_appType.GetMethod("OnShutDown");

            m_applicationFocus = m_appType.GetMethod("OnApplicationFocus");
            m_onApplicationPause = m_appType.GetMethod("OnApplicationPause");
            m_onApplicationQuit = m_appType.GetMethod("OnApplicationQuit");
            m_getLanguageInfoByID = m_appType.GetMethod("GetLanguageInfoByID");
            await UniTask.CompletedTask;
        }
        public override void OnFixedUpdate()
        {
            
        }

        public override void OnShutDown()
        {
            
        }

        public override void OnApplicationFocus(bool hasFocus)
        {
            
        }

        public override void OnApplicationPause(bool pauseStatus)
        {
            
        }

        public override void OnApplicationQuit()
        {
            
        }
    }
}