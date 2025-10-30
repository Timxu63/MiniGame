
using System.Collections.Generic;
using cfg;
using HotFix;

namespace HotFixBattle
{
    /// <summary>
    /// 章节处理器工厂
    /// </summary>
    public static class ChapterHandlerFactory
    {
        private static Dictionary<eChapterType, System.Func<IChapterHandler>> _handlerCreators = 
            new Dictionary<eChapterType, System.Func<IChapterHandler>>();

        /// <summary>
        /// 注册章节处理器创建函数
        /// </summary>
        /// <param name="chapterType">章节类型</param>
        /// <param name="creator">创建函数</param>
        public static void RegisterHandler(eChapterType chapterType, System.Func<IChapterHandler> creator)
        {
            if (!_handlerCreators.ContainsKey(chapterType))
            {
                _handlerCreators.Add(chapterType, creator);
            }
        }

        /// <summary>
        /// 创建章节处理器
        /// </summary>
        /// <param name="chapterType">章节类型</param>
        /// <returns>章节处理器实例</returns>
        public static IChapterHandler CreateHandler(eChapterType chapterType)
        {
            if (_handlerCreators.TryGetValue(chapterType, out var creator))
            {
                return creator();
            }

            // 默认返回普通章节处理器
            return new NormalChapterHandler();
        }

        /// <summary>
        /// 初始化默认处理器
        /// </summary>
        public static void InitializeDefaultHandlers()
        {
            // 注册普通章节处理器
            RegisterHandler(eChapterType.Normal, () => new NormalChapterHandler());
        }
    }
}
