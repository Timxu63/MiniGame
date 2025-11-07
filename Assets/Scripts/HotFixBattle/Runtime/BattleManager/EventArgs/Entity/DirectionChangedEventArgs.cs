using Framework.EventSystem;
using UnityEngine;

namespace HotFix
{
    public class DirectionChangedEventArgs : BaseEventArgs
    {
        public Vector2 Direction { get; set; }
        public override void Clear()
        {
            
        }
    }
}