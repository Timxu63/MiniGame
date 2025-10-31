using Framework.EventSystem;
using Framework.ViewModule;
using HotFixBattle;
using TMPro;
using UnityEngine;

namespace HotFix
{
    public class UIBattleViewModule : BaseViewModule
    {
        [SerializeField] private TextMeshProUGUI text_Wave;
        public override void RegisterEvents(EventSystemManager manager)
        {
            manager.RegisterEvent((int)LocalMessageName.CC_BattleWaveChange, BattleWaveChange);
        }

        public override void UnRegisterEvents(EventSystemManager manager)
        {
            manager.UnRegisterEvent((int)LocalMessageName.CC_BattleWaveChange, BattleWaveChange);
        }
        private void BattleWaveChange(int type, BaseEventArgs eventargs)
        {
            BattleWaveChangeEventArgs args = eventargs as BattleWaveChangeEventArgs;
            text_Wave.text = args.WaveId.ToString();
        }
        public override void OnCreate(object data)
        {
            
        }

        public override void OnDelete()
        {
            
        }

        public override void OnOpen(object data)
        {
            
        }

        public override void OnClose()
        {
            
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            
        }
    }
}