//-----------------------------------------------------------------
//
//              Maggic @  2023-09-19 14:51 
//
//----------------------------------------------------------------

using System.Threading.Tasks;

namespace Framework.ViewModule
{
    /// <summary>
    /// 显示层加载器 基类
    /// </summary>
    public abstract class BaseViewModuleLoader
    {
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract Task OnLoad(object data);

        /// <summary>
        /// 卸载
        /// </summary>
        public abstract void OnUnLoad();
    }
}