using System;
using Framework.EventSystem;

namespace Framework.ViewModule
{
    public class ViewEventArgs : BaseEventArgs
    {
        public int ViewName;

        public ViewEventArgs SetData(int viewName)
        {
            ViewName = viewName;
            return this;
        }

        public override void Clear()
        {
            ViewName = 0;
        }
    }
}