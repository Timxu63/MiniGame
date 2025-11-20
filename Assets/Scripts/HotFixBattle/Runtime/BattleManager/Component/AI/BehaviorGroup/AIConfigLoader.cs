using cfg;
using Game.Logic.BattleModule.Entity;

namespace HotFixBattle.AI
{
    /// <summary>
    /// AI配置加载器，用于从Charactor表中加载AI配置
    /// </summary>
    public static partial class AIConfigLoader
    {
        // 此类已拆分为三个部分文件：
        // - AIConfigLoader.States.cs：状态工厂相关
        // - AIConfigLoader.Behaviors.cs：行为工厂相关
        // - AIConfigLoader.Rules.cs：决策规则工厂相关
    }
}