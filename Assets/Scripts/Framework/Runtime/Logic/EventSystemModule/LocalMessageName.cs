namespace Framework.Logic.Modules
{
    /// <summary>
    /// 消息统计
    /// </summary>
    public enum LocalMessageName
    {
        None = 0,

        /// <summary>
        /// 多语言  -EventArgLanguageType
        /// </summary>
        CC_REFRESH_LANGUAGE = 1,

        /// <summary>
        /// 多语言 切换完成
        /// </summary>
        CC_REFRESH_LANGUAGE_FINISH = 2,

        /// <summary>
        /// Tip界面添加子集 -EventArgTipViewModuleAddNode
        /// </summary>
        CC_TipViewModule_AddTextTipNode =3,

        /// <summary>
        ///     打开页面
        /// </summary>
        CC_ViewOpen =4,

        /// <summary>
        ///     关闭页面
        /// </summary>
        CC_ViewClose =5,
        
        /// <summary>
        /// 进入游戏主页预加载进度
        /// </summary>
        CC_PreloadProgress =6,
        
        /// <summary>
        /// 进入游戏主页预加载进度跑满
        /// </summary>
        CC_PreloadUIProgressComplete =7
    }
}