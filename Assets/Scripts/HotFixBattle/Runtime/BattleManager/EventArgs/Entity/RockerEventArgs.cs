using Framework.EventSystem;
using UnityEngine;

namespace HotFixBattle
{
    public class RockerEventArgs : BaseEventArgs
    {
        public Vector2 moveDir;
        public override void Clear()
        {
            
        }
    }
}