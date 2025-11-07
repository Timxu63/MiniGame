using Framework.EventSystem;
using UnityEngine;

namespace HotFixBattle
{
    public class PlayerSkillArgs : BaseEventArgs
    {
        public int skillId;
        public Vector2 direction;
        public bool haveDir;
        public override void Clear()
        {
            
        }
    }
}