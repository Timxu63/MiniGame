using System;
using System.Threading.Tasks;
using Framework.ViewModule;
using UnityEngine;

namespace HotFix
{
    public static class ViewManagerExpand
    {
        #region View
        /// <summary>
        /// 获得显示层
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="manager"></param>
        /// <param name="name"></param> 
        /// <returns></returns>
        public static T GetViewModule<T>(this ViewModuleManager manager, ViewName name) where T : BaseViewModule
        {
            return manager.GetViewModule<T>((int)name);
        }

        /// <summary>
        /// 打开界面层
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="name">界面ID</param>
        /// <param name="data">界面数据</param>
        /// <param name="layer">界面层级</param>
        /// <param name="loadedCallBack">加载回调</param>
        /// <param name="openedCallBack">打开回调</param>
        public static async void OpenView(this ViewModuleManager manager, ViewName name,
            object data = null,
            UILayers layer = UILayers.First,
            Action<GameObject> loadedCallBack = null,
            Action<GameObject> openedCallBack = null)
        {
            Logger.Log($"<color=red>[ViewModule]</color>OpenView( {name} )");
            await manager.OpenView((int)name, data, layer, loadedCallBack, openedCallBack);
        }

        /// <summary>
        /// 打开界面层,并返回Task
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="name">界面ID</param>
        /// <param name="data">界面数据</param>
        /// <param name="layer">界面层级</param>
        /// <param name="loadedCallBack">加载回调</param>
        /// <param name="openedCallBack">打开回调</param>
        public static async Task OpenViewTask(this ViewModuleManager manager, ViewName name,
            object data = null,
            UILayers layer = UILayers.First,
            Action<GameObject> loadedCallBack = null,
            Action<GameObject> openedCallBack = null)
        {
            Logger.Log($"<color=red>[ViewModule]</color>OpenViewTask( { name} )");
            await manager.OpenView((int)name, data, layer, loadedCallBack, openedCallBack);
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="name">界面ID</param>
        public static void CloseView(this ViewModuleManager manager, ViewName name)
        {
            Logger.Log($"<color=red>[ViewModule]</color>CloseView( {name} )");
            manager.CloseView((int)name);
        }

        public static void CloseViewIfOpenedOrLoading(this ViewModuleManager manager, ViewName name)
        {
            if (manager.IsOpenedOrLoading(name))
            {
                manager.CloseView(name);
            }
        }

        /// <summary>
        /// 是否界面是打开状态
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="name">界面ID</param>
        /// <returns></returns>
        public static bool IsOpened(this ViewModuleManager manager, ViewName name)
        {
            return manager.IsOpened((int)name);
        }

        /// <summary>
        /// 界面是否是加载状态
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="name">界面ID</param>
        /// <returns></returns>
        public static bool IsLoading(this ViewModuleManager manager, ViewName name)
        {
            return manager.IsLoading((int)name);
        }

        /// <summary>
        /// 界面是否是加载状态或者打开状态
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="name">界面ID</param>
        /// <returns></returns>
        public static bool IsOpenedOrLoading(this ViewModuleManager manager, ViewName name)
        {
            return manager.IsOpenedOrLoading((int)name);
        }
        #endregion
    }
}