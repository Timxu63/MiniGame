using Framework.ViewModule;

namespace HotFix
{
    public partial class Main
    {
        private class View
        { 
            public void Register(ViewModuleManager views)
            {
                views.RegisterViewModule(new ViewModuleData((int)ViewName.UIMain, null,
                    "Assets/_Resources/Prefab/Main/UIMain.prefab", true));
                views.RegisterViewModule(new ViewModuleData((int)ViewName.UILoading, null,
                    "Assets/_Resources/Prefab/Loading/UILoading.prefab", true));
                views.RegisterViewModule(new ViewModuleData((int)ViewName.UIBattle, null,
                    "Assets/_Resources/Prefab/Battle/UIBattle.prefab", true));
            }
        }
    }
}