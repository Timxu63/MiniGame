using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotFix
{
    /// <summary>
    /// 状态机名称统计
    /// </summary>
    public enum StateName
    {
        /// <summary>
        /// 检查资源
        /// </summary>
        CheckAssetsState = 1,

        /// <summary>
        /// 登录状态
        /// </summary>
        LoginState = 101,

        /// <summary>
        /// 首次进入游戏
        /// </summary>
        FirstEnterWorldState,

        /// <summary>
        /// 主页状态
        /// </summary>
        MainState,

        /// <summary>
        /// 游戏状态
        /// </summary>
        GameState,
        
        /// <summary>
        /// 预加载进入主页资源状态
        /// </summary>
        PreloadState
    }
}