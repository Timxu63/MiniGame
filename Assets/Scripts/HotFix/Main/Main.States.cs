using Framework;
using Framework.State;

namespace HotFix
{
    public partial class Main
    {
        private class State
        {
            /// <summary>
            /// 注册所有状态
            /// </summary>
            public void Register(StateManager states)
            {
                Logger.Log("Main.RegisterAllStates");
                states.RegisterState(new PreloadState());
                states.RegisterState(new LoginState());
                states.RegisterState(new MainState());
                states.RegisterState(new GameState());
            }
        }
    }
}
