using cfg;
using HotFixBattle;

namespace HotFix
{
    public interface IChapterHandler
    {
        public void Initialize(Chapter config, BattleWorldContext worldContext);
        public Chapter_WaveGroup GetNextWaveGroup();
        public bool HasMoreWaveGroups();
        public void OnWaveGroupStart(Chapter_WaveGroup waveGroup);
        public void OnWaveGroupEnd(Chapter_WaveGroup waveGroup);
    }
}