using Framework.EventSystem;

namespace HotFixBattle
{
    public class BattleWaveChangeEventArgs : BaseEventArgs
    {
        public int WaveId { get; set; }

        public override void Clear()
        {
            
        }
    }
}