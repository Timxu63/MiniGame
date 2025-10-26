using cfg;
using Framework.EventSystem;
using Framework.Runtime;
using Framework.ViewModule;
using UnityEngine;

namespace HotFix
{
    public class UIMainViewModule : BaseViewModule
    {
        public override void RegisterEvents(EventSystemManager manager)
        {
            
        }

        public override void UnRegisterEvents(EventSystemManager manager)
        {
            
        }

        public override void OnCreate(object data)
        {
        }

        public override void OnDelete()
        {
            
        }

        public override void OnOpen(object data)
        {
            
            var item = GameApp.Table.GetTableData<TbReward, Reward>(1002);
            Debug.LogError(item.Name);
        }

        public override void OnClose()
        {
            
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            
        }
    }
}