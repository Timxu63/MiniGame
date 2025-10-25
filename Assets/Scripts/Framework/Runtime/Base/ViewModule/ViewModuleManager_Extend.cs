//-----------------------------------------------------------------
//
//              Maggic @  2023-09-19 15:10 
//
//----------------------------------------------------------------

using System;

namespace Framework.ViewModule
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class ViewModuleManager // 遮罩层
    {
        public struct MaskDisposer : IDisposable
        {
            private ViewModuleManager m_viewModuleManager;
            public MaskDisposer(ViewModuleManager viewModuleManager)
            {
                m_viewModuleManager = viewModuleManager;
            }
            public void PopMask()
            {
                m_viewModuleManager.PopMask();
            }
            public void Dispose()
            {
                PopMask();
            }
        }
        public MaskDisposer PushMask()
        {
            SetMask(true);
            return new MaskDisposer(this);
        }
        public void PopMask()
        {
            SetMask(false);
        }
        private void SetMask(bool active)
        {
            m_maskCount += active == true ? 1 : -1;
            if (m_maskUI != null)
            {
                m_maskUI.SetActive(m_maskCount > 0);
            }
        }

        // #region 网络通信遮罩页
        //
        // private int m_netLoadingCount = 0;
        // /// <summary>
        // /// 展示网络加载遮罩页
        // /// </summary>
        // /// <param name="value"></param>
        // public void ShowNetLoading(bool value)
        // {
        //     m_netLoadingCount += value ? 1 : -1;
        //     if (value && m_netLoadingCount == 1)
        //     {
        //         if (GameApp.View.IsOpenedOrLoading(ViewName.NetLoadingViewModule) == false)
        //         {
        //             _ = GameApp.View.OpenView(ViewName.NetLoadingViewModule, null, UILayers.Third, null, null);
        //         }
        //     }
        //     else if (!value && m_netLoadingCount == 0)
        //     {
        //         if (GameApp.View.IsOpened(ViewName.NetLoadingViewModule) == true) GameApp.View.CloseView(ViewName.NetLoadingViewModule);
        //     }
        // }
        //
        // #endregion
    }
}