namespace HotFixBattle
{
    /// <summary>
    /// 格子类型枚举
    /// </summary>
    public enum eCellType
    {
        Normal = 0,     // 普通地形，可正常通行
        Obstacle = 1,   // 障碍物，不可通行
        Water = 2,      // 水域，通行成本高
        Mud = 3,        // 泥地，通行成本较高
        Road = 4,       // 道路，通行成本低
        Mountain = 5,   // 山地，不可通行
        Forest = 6,     // 森林，通行成本中等
        Bridge = 7      // 桥梁，可通行
    }
}