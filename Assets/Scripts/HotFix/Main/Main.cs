using cfg;
using Framework.Runtime;
using UnityEngine;

namespace HotFix
{
    public partial class Main
    {
        private State m_state = new State();
        private View m_view = new View();
        private DataModule m_dataModule = new DataModule();
        
        public void OnStarUp()
        {
            m_state.Register(GameApp.State);
            m_view.Register(GameApp.View);
            m_dataModule.Register(GameApp.DataModule);
            RegisterTable();
        }
        
        private void OnLoadFinished()
        {
            #if UNITY_EDITOR
            if (GameApp.IsEditorScene)
            {
                GameApp.State.ActiveState((int)StateName.GameState);
                return;
            }
            #endif
            GameApp.State.ActiveState((int)StateName.PreloadState);
        }
        /// <summary>
        /// 加载表格数据
        /// </summary>
        private void RegisterTable()
        {
            Logger.Log("Main.RegisterTable 加载表格数据");
            GameTableProxy modelProxy = new GameTableProxy();
            GameApp.Table.SetITableManager(modelProxy);
            modelProxy.InitialiseLocalModels(OnLoadFinished);
        }
        public void OnShutDown()
        {
            AssetsPoolManager.Instance.ForceReleaseAll();
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