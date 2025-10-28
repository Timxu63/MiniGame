using UltraLiteDB;

namespace Framework.PersistentData
{
    public enum GameDbOp
    {
        None,
        Insert,
        Update,
        Delete,
        UpOrInsert
    }

    public struct GameDbOpCmd
    {
        public GameDbOp op;
        public string colName;
        public ulong id;
        public BsonDocument doc;
    }
}