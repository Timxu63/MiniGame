using Framework.EventSystem;

namespace HotFix
{
    public static class EventExpand
    {
        public static void RegisterEvent(this EventSystemManager manager, LocalMessageName name, HandlerEvent handle)
        {
            manager.RegisterEvent((int)name, handle);
        }

        /// <summary>
        /// 取消注册事件
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="name">事件ID</param>
        /// <param name="handle">事件回调</param>
        public static void UnRegisterEvent(this EventSystemManager manager, LocalMessageName name, HandlerEvent handle)
        {
            manager.UnRegisterEvent((int)name, handle);
        }
    }
}