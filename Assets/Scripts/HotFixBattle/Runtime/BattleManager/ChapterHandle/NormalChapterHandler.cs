using cfg;
using HotFixBattle;

namespace HotFix
{
    public class NormalChapterHandler : BaseChapterHandler
    {
        public override void Initialize(Chapter config, BattleWorldContext worldContext)
        {
            base.Initialize(config, worldContext);
            // 可以在这里添加普通章节特有的初始化逻辑
        }

        public override void OnWaveGroupStart(Chapter_WaveGroup waveGroup)
        {
            base.OnWaveGroupStart(waveGroup);
            // 可以在这里添加波次开始时的特定逻辑
            // 例如：显示波次提示UI
        }

        public override void OnWaveGroupEnd(Chapter_WaveGroup waveGroup)
        {
            base.OnWaveGroupEnd(waveGroup);
            // 可以在这里添加波次结束时的特定逻辑
            // 例如：显示波次完成奖励
        }
    }
}
