//-----------------------------------------------------------------
//
//              Maggic @  2023-09-19 14:49 
//
//----------------------------------------------------------------

using UnityEngine;

namespace Framework.ViewModule
{
    public enum DestoryType
    {
        Dont,
        Immediate,
        Auto,
    }

    /// <summary>
    /// 界面数据结构
    /// </summary>
    public class ViewModuleData
    {
        public int m_id;

        public string m_assetPath;
        public GameObject m_gameObject;

        public BaseViewModuleLoader m_loader;

        public GameObject m_prefab;
        public BaseViewModule m_viewModule;

        public bool m_isCanDestory = false;
        public ViewState m_viewState;

        public bool SetPadding { get; private set; }

        /// <summary>
        /// 不需要加载的构造函数
        /// </summary>
        /// <param name="viewID"></param>
        /// <param name="viewType"></param>
        /// <param name="gameObject"></param>
        /// <param name="isCanDestory"></param>
        /// <param name="setPadding"></param>
        public ViewModuleData(int viewID, GameObject gameObject = null, bool isCanDestory = true, bool setPadding = true)
        {
            m_id = viewID;
            m_gameObject = gameObject;
            m_isCanDestory = isCanDestory;
            SetPadding = setPadding;
        }

        /// <summary>
        /// 需要加载的
        /// </summary>
        /// <param name="viewID"></param>
        /// <param name="viewType"></param>
        /// <param name="loader"></param>
        /// <param name="assetPath"></param>
        /// <param name="isCanDestory"></param>
        /// <param name="setPadding"></param>
        public ViewModuleData(int viewID, BaseViewModuleLoader loader, string assetPath, bool isCanDestory = true, bool setPadding=true)
        {
            m_id = viewID;
            m_loader = loader;
            m_assetPath = assetPath;
            m_isCanDestory = isCanDestory;
            SetPadding = setPadding;
        }
    }
}