using Framework.EventSystem;
using Framework.State;

namespace HotFix
{
    public class GameState : State
    {
        public override int GetName()
        {
            return (int) StateName.GameState;
        }

        public override void OnEnter()
        {
            
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            
        }

        public override void OnShutDown()
        {
            
        }

        public override void OnAppFocus(bool hasFocus)
        {
            
        }

        public override void OnAppPause(bool pauseStatus)
        {
            
        }

        public override void OnExit()
        {
            
        }

        public override void RegisterEvents(EventSystemManager manager)
        {
        }

        public override void UnRegisterEvents(EventSystemManager manager)
        {
        }
    }
}