using cfg;
using Framework.EventSystem;
using Framework.Runtime;
using Framework.ViewModule;
using HotFix.Runtime.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix
{
    public class UIMainViewModule : BaseViewModule
    {
        [SerializeField] private Button btn_JoinBattle;
        public override void RegisterEvents(EventSystemManager manager)
        {
            
        }

        public override void UnRegisterEvents(EventSystemManager manager)
        {
            
        }

        public override void OnCreate(object data)
        {
            btn_JoinBattle.onClick.AddListener(OnClickJoinBattle);
        }

        private void OnClickJoinBattle()
        {
            Runtime.Logic.Game.JoinBattle(1, null);
        }

        public override void OnDelete()
        {
            
        }

        public override void OnOpen(object data)
        {
            Reward item =GameTableProxy.Tables.TbReward.Get(1002);
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