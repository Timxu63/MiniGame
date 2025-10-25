using UnityEngine;

namespace Framework.Runtime
{
    public class GameLauncher : MonoBehaviour
    {
        [SerializeField] private GameApp m_gameApp = null;
        void Awake()
        {
            //屏幕永不休眠
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            DontDestroyOnLoad(gameObject);
                    
            m_gameApp.OnStarUp();
        }

        private void Update()
        {
            m_gameApp.OnUpdate();
        }

        private void LateUpdate()
        {
            m_gameApp.OnLateUpdate(); 
        }

        private void FixedUpdate()
        {
            m_gameApp.OnFixedUpdate();
        }

        private void OnApplicationQuit()
        {
            m_gameApp.OnAppShutdown();
            m_gameApp = null;
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            m_gameApp.OnAppFocus(hasFocus);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            m_gameApp.OnAppPause(pauseStatus);
        }


        [ContextMenu("OnShutdown")]
        public void OnShutdown()
        {
            m_gameApp.OnAppShutdown();
        }

        [ContextMenu("OnRestart")]
        public void OnRestart()
        {
            m_gameApp.OnRestart();
        }
    }
}