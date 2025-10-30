using cfg;
using HotFixBattle;

namespace HotFix
{
    public static class BattleUtil
    {
        public static ModeData GetMainBattleTransferDataFromServer(int chapterId)
        {
            var modeData = new ModeDataNormal();
            modeData.ChapterId = chapterId;
            modeData.RandomSeed = (uint)0;

            return modeData;
        }
        
        /// <summary>
        /// 根据游戏模式获取对应的战斗管理器类型
        /// </summary>
        /// <param name="gameModel">游戏模式</param>
        /// <returns>战斗管理器类型</returns>
        public static System.Type GetBattleManagerType(eChapterType gameModel)
        {
            switch (gameModel)
            {
                case eChapterType.Normal:
                    return typeof(BattleManagerNormal);
                // 可以在这里添加其他游戏模式对应的战斗管理器类型
                // case eChapterType.Elite:
                //     return typeof(BattleManagerElite);
                // case eChapterType.Boss:
                //     return typeof(BattleManagerBoss);
                default:
                    return typeof(BattleManagerNormal);
            }
        }
    }
}