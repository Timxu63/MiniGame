using UltraLiteDB;

namespace Framework.PersistentData
{
    public abstract class PersistentDataBase
    {
        
        public bool IsDirty { private set; get; }
       
        public abstract string CollectionName { get; }

        public ulong UserId;
        // 首次创建调用
        public abstract void OnCreate();

        // 读取bytes后调用,首次创建时不调用,也可都调用,需要改动再说
        public abstract void OnInit();
        
        public void Dirty()
        {
            IsDirty = true;
        }

        public void SaveData()
        {
            IsDirty = false;
            // GameDbMgr.Instance.SaveData(GetType(), this, CollectionName,UserId);
        }

    }
}