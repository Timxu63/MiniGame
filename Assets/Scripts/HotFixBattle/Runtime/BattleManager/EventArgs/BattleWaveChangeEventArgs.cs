using Framework.EventSystem;

namespace HotFixBattle
{
    public class BattleWaveChangeEventArgs : BaseEventArgs
    {
        public int MissionId { get; set; }

        public override void Clear()
        {
            
        }
    }
}