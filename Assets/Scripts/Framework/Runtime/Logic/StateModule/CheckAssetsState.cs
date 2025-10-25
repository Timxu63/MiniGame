using Framework.EventSystem;
using Framework.Logic.Modules;
using Framework.Runtime;

namespace Framework
{
    public class CheckAssetsState : State.State
    {
        public override int GetName()
        {
            return (int) StateName.CheckAssetsState;
        }

        public override void OnEnter()
        {
            GameApp.RunTime.Load();
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