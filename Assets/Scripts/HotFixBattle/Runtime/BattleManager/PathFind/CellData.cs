using UnityEngine;

namespace HotFixBattle
{
    public class CellData
    {
        public int X { get; set; } // 网格X坐标
        public int Y { get; set; } // 网格Y坐标
        public eCellType CellType { get; set; } = eCellType.Normal; // 格子类型

        /// <summary>
        /// 是否可通行（根据格子类型判断）
        /// </summary>
        public bool IsWalkable 
        { 
            get 
            {
                return CellType != eCellType.Obstacle && CellType != eCellType.Mountain;
            }
        }

        /// <summary>
        /// 通行成本（根据格子类型自动计算）
        /// </summary>
        public float Cost 
        { 
            get 
            {
                switch (CellType)
                {
                    case eCellType.Normal:
                        return 1.0f;
                    case eCellType.Obstacle:
                    case eCellType.Mountain:
                        return float.MaxValue; // 不可通行
                    case eCellType.Water:
                        return 3.0f; // 水域通行成本高
                    case eCellType.Mud:
                        return 2.0f; // 泥地通行成本较高
                    case eCellType.Road:
                        return 0.5f; // 道路通行成本低
                    case eCellType.Forest:
                        return 1.5f; // 森林通行成本中等
                    case eCellType.Bridge:
                        return 0.8f; // 桥梁通行成本较低
                    default:
                        return 1.0f;
                }
            }
        }

        // 流场寻路相关属性
        public float Distance { get; set; } = float.MaxValue; // 到目标的距离
        public Vector2 Direction { get; set; } = Vector2.zero; // 流场方向
        public bool IsVisited { get; set; } = false; // 是否已访问
        public bool IsTarget { get; set; } = false; // 是否是目标点
        public bool IsBlocked { get; set; } = false; // 是否被阻挡

        // 可视化相关
        public Color DebugColor { get; set; } = Color.white;

        /// <summary>
        /// 设置格子类型并更新相关属性
        /// </summary>
        /// <param name="cellType">格子类型</param>
        public void SetCellType(eCellType cellType)
        {
            CellType = cellType;
            // 根据格子类型设置调试颜色
            switch (cellType)
            {
                case eCellType.Normal:
                    DebugColor = Color.white;
                    break;
                case eCellType.Obstacle:
                    DebugColor = Color.black;
                    break;
                case eCellType.Water:
                    DebugColor = Color.blue;
                    break;
                case eCellType.Mud:
                    DebugColor = new Color(0.5f, 0.25f, 0.1f); // 棕色
                    break;
                case eCellType.Road:
                    DebugColor = Color.grey;
                    break;
                case eCellType.Mountain:
                    DebugColor = new Color(0.4f, 0.4f, 0.4f); // 深灰色
                    break;
                case eCellType.Forest:
                    DebugColor = Color.green;
                    break;
                case eCellType.Bridge:
                    DebugColor = new Color(0.7f, 0.5f, 0.3f); // 木色
                    break;
            }
        }
    }
}