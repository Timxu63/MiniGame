using Framework.DataModule;
using Framework.Runtime;

namespace HotFix
{
    public partial class Main
    {
        private class DataModule
        {
            /// <summary>
            /// 注册  
            /// </summary>
            /// <param name="manager"></param>
            /// <param name="dataModule"></param>
            private void OnRegister(DataModuleManager manager, IDataModule dataModule)
            {
                manager.RegisterDataModule(dataModule);
            }

            /// <summary>
            ///  注册所有数据层
            /// </summary>
            public void Register(DataModuleManager manager)
            {
                OnRegister(manager, new BattleDataModule());
            }
        }
    }
}