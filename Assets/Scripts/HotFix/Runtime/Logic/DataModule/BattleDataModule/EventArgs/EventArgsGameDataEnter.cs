using cfg;
using Framework.EventSystem;

namespace HotFix
{
    public class EventArgsGameDataEnter : BaseEventArgs
    {
        /// <summary>
        /// 当前战斗模式
        /// </summary>
        public eChapterType GameModel { get; set; }
        /// <summary>
        /// 模式数据
        /// </summary>
        public ModeData ModeData { get; set; }
        public override void Clear()
        {
            
        }
    }
}