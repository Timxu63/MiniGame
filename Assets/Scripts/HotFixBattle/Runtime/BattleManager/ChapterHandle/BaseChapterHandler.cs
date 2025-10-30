using cfg;
using HotFixBattle;

namespace HotFix
{
    // BaseChapterHandler.cs
    public abstract class BaseChapterHandler : IChapterHandler
    {
        protected Chapter _config;
        protected int _currentWaveGroupIndex = 0;
        protected BattleWorldContext _worldContext;
    
        public virtual void Initialize(Chapter config, BattleWorldContext worldContext)
        {
            _config = config;
            _worldContext = worldContext;
            _currentWaveGroupIndex = 0;
        }
    
        public virtual Chapter_WaveGroup GetNextWaveGroup()
        {
            if (_currentWaveGroupIndex < _config.WaveGroups.Length)
            {
                int waveGroupId = _config.WaveGroups[_currentWaveGroupIndex++];
                var curData = _worldContext.Tables.TbChapterWaveGroup.Get(waveGroupId);
                return curData;
            }
            return null;
        }
    
        public virtual bool HasMoreWaveGroups()
        {
            return _currentWaveGroupIndex < _config.WaveGroups.Length;
        }
    
        public virtual void OnWaveGroupStart(Chapter_WaveGroup waveGroup) { }
        public virtual void OnWaveGroupEnd(Chapter_WaveGroup waveGroup) { }
    }
}