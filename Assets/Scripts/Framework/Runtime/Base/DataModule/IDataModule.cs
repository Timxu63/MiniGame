using Framework.EventSystem;

namespace Framework.DataModule
{
    public interface IDataModule
    {
        /// <summary>
        /// 获得名称
        /// </summary>
        /// <returns></returns>
        int GetName();

        void RegisterEvents(EventSystemManager manager);

        void UnRegisterEvents(EventSystemManager manager);

        void Clear();
    }
}
