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
                    "Assets/_Resources/Prefab/UI/Main/UIMain.prefab", true));
                views.RegisterViewModule(new ViewModuleData((int)ViewName.UILoading, null,
                    "Assets/_Resources/Prefab/UI/Loading/UILoading.prefab", true));
                views.RegisterViewModule(new ViewModuleData((int)ViewName.UIBattle, null,
                    "Assets/_Resources/Prefab/UI/Battle/UIBattle.prefab", true));
            }
        }
    }
}