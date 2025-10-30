using Framework.DataModule;
using Framework.EventSystem;

namespace HotFix
{
    public class BattleDataModule : IDataModule
    {
        public EventArgsGameDataEnter m_openBattleData;
        public int GetName()
        {
            return (int)DataName.BattleDataModule;
        }

        public void RegisterEvents(EventSystemManager manager)
        {
            manager.RegisterEvent((int)LocalMessageName.CC_GameData_GameEnter, UpdateCurBattleData);
        }

        public void UnRegisterEvents(EventSystemManager manager)
        {
            manager.UnRegisterEvent((int)LocalMessageName.CC_GameData_GameEnter, UpdateCurBattleData);
        }

        private void UpdateCurBattleData(int type, BaseEventArgs eventargs)
        {
            EventArgsGameDataEnter args = (EventArgsGameDataEnter)eventargs;
            m_openBattleData = args;
        }

        public void Clear()
        {
            
        }
    }
}