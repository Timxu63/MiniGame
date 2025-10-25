//-----------------------------------------------------------------
//
//              Maggic @  2023-03-21 16:22 
//
//----------------------------------------------------------------

using System;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseRunTimeModel
    {
        public Action m_onFinished = null;
        protected bool m_isLoadingFinished = false;

        public abstract void Load();

        public abstract void OnFixedUpdate();

        public abstract void OnStarUp();

        public abstract void OnShutDown();

        public abstract void OnApplicationFocus(bool hasFocus);

        public abstract void OnApplicationPause(bool pauseStatus);

        public abstract void OnApplicationQuit();
    }
}