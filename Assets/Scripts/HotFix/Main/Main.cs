using Framework.Runtime;
using UnityEngine;

namespace HotFix
{
    public partial class Main
    {
        private State m_state = new State();
        private View m_view = new View();
        
        public void OnStarUp()
        {
            m_state.Register(GameApp.State);
            m_view.Register(GameApp.View);
            Debug.LogError("---------");
            OnLoadFinished();
        }
        
        private void OnLoadFinished()
        {
            GameApp.State.ActiveState((int)StateName.PreloadState);
        }
        public void OnShutDown()
        {
            
        }
        public void OnApplicationFocus()
        {
            
        }public void OnApplicationPause()
        {
            
        }public void OnApplicationQuit()
        {
            
        }public void GetLanguageInfoByID()
        {
            
        }
    }
}