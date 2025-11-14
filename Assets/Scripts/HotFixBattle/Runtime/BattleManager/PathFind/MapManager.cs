using System;
using System.Collections.Generic;
using FlowFieldSystem;
using UnityEngine;
using Framework;
using Vector2Int = UnityEngine.Vector2Int;

namespace HotFixBattle
{
    /// <summary>
    /// 地图管理器，负责管理游戏地图、寻路和流场计算
    /// </summary>
    public class MapManager : Singleton<MapManager>
    {
        #region 私有字段

        // 地图网格数据
        private CellData[,] _grid;

        // 地图尺寸
        private int _width;
        private int _height;

        // 格子大小（世界单位）
        private float _cellSize = 1.0f;

        // 流场数据
        private FlowFieldData _flowFieldData;

        // 地图边界
        private Bounds _mapBounds;

        // 是否已初始化
        private bool _isInitialized = false;

        // 默认地图大小
        private const int DEFAULT_MAP_WIDTH = 200;
        private const int DEFAULT_MAP_HEIGHT = 200;

        private BattleWorldContext m_battleWorldContext;
        #endregion

        #region 公共属性

        /// <summary>
        /// 地图宽度（格子数）
        /// </summary>
        public int Width => _width;

        /// <summary>
        /// 地图高度（格子数）
        /// </summary>
        public int Height => _height;

        /// <summary>
        /// 格子大小（世界单位）
        /// </summary>
        public float CellSize => _cellSize;

        /// <summary>
        /// 地图边界
        /// </summary>
        public Bounds MapBounds => _mapBounds;

        /// <summary>
        /// 是否已初始化
        /// </summary>
        public bool IsInitialized => _isInitialized;

        #endregion

        #region 公共方法

        /// <summary>
        /// 初始化地图管理器
        /// </summary>
        public void Initialize(BattleWorldContext worldContext)
        {
            m_battleWorldContext = worldContext;
            Initialize(DEFAULT_MAP_WIDTH, DEFAULT_MAP_HEIGHT, _cellSize);
        }

        /// <summary>
        /// 初始化地图管理器（指定尺寸）
        /// </summary>
        /// <param name="width">地图宽度（格子数）</param>
        /// <param name="height">地图高度（格子数）</param>
        /// <param name="cellSize">格子大小（世界单位）</param>
        public void Initialize(int width, int height, float cellSize = 1.0f)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("[MapManager] 地图管理器已经初始化，先清理再重新初始化");
                Clear();
            }

            _width = width;
            _height = height;
            _cellSize = cellSize;

            // 初始化网格数据
            _grid = new CellData[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _grid[x, y] = new CellData
                    {
                        X = x,
                        Y = y,
                        CellType = eCellType.Normal
                    };
                }
            }

            // 初始化流场数据
            _flowFieldData = new FlowFieldData(width, height, cellSize);

            // 设置地图边界
            Vector3 center = new Vector3(width * cellSize * 0.5f, 0, height * cellSize * 0.5f);
            Vector3 size = new Vector3(width * cellSize, 1, height * cellSize);
            _mapBounds = new Bounds(center, size);

            _isInitialized = true;
            Debug.Log($"[MapManager] 地图管理器初始化完成，尺寸: {width}x{height}，格子大小: {cellSize}");
        }

        /// <summary>
        /// 清理地图管理器
        /// </summary>
        public void Clear()
        {
            if (!_isInitialized) return;

            _grid = null;
            _flowFieldData = null;
            _isInitialized = false;
            Debug.Log("[MapManager] 地图管理器已清理");
        }

        /// <summary>
        /// 获取指定位置的格子数据
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <returns>格子数据，如果坐标超出范围则返回null</returns>
        public CellData GetCell(int x, int y)
        {
            if (!IsValidCoordinate(x, y))
            {
                return null;
            }
            return _grid[x, y];
        }

        /// <summary>
        /// 获取世界坐标对应的格子数据
        /// </summary>
        /// <param name="worldPosition">世界坐标</param>
        /// <returns>格子数据，如果坐标超出范围则返回null</returns>
        public CellData GetCell(Vector3 worldPosition)
        {
            Vector2Int gridPos = WorldToGrid(worldPosition);
            return GetCell(gridPos.x, gridPos.y);
        }

        /// <summary>
        /// 设置格子的类型
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="cellType">格子类型</param>
        /// <returns>是否设置成功</returns>
        public bool SetCellType(int x, int y, eCellType cellType)
        {
            if (!IsValidCoordinate(x, y))
            {
                return false;
            }

            _grid[x, y].SetCellType(cellType);
            UpdateFlowFieldObstacle(x, y);
            return true;
        }

        /// <summary>
        /// 设置世界坐标对应格子的类型
        /// </summary>
        /// <param name="worldPosition">世界坐标</param>
        /// <param name="cellType">格子类型</param>
        /// <returns>是否设置成功</returns>
        public bool SetCellType(Vector3 worldPosition, eCellType cellType)
        {
            Vector2Int gridPos = WorldToGrid(worldPosition);
            return SetCellType(gridPos.x, gridPos.y, cellType);
        }

        /// <summary>
        /// 检查世界坐标是否在地图范围内
        /// </summary>
        /// <param name="worldPosition">世界坐标</param>
        /// <returns>是否在范围内</returns>
        public bool IsInBounds(Vector3 worldPosition)
        {
            return _mapBounds.Contains(worldPosition);
        }

        /// <summary>
        /// 检查格子坐标是否有效
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <returns>是否有效</returns>
        public bool IsValidCoordinate(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height;
        }

        /// <summary>
        /// 检查指定世界坐标是否可通行
        /// </summary>
        /// <param name="worldPosition">世界坐标</param>
        /// <returns>是否可通行</returns>
        public bool IsWalkable(Vector3 worldPosition)
        {
            var cell = GetCell(worldPosition);
            return cell != null && cell.IsWalkable;
        }

        /// <summary>
        /// 世界坐标转格子坐标
        /// </summary>
        /// <param name="worldPosition">世界坐标</param>
        /// <returns>格子坐标</returns>
        public Vector2Int WorldToGrid(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt(worldPosition.x / _cellSize);
            int y = Mathf.FloorToInt(worldPosition.z / _cellSize);
            return new Vector2Int(x, y);
        }

        /// <summary>
        /// 格子坐标转世界坐标
        /// </summary>
        /// <param name="gridX">格子X坐标</param>
        /// <param name="gridY">格子Y坐标</param>
        /// <returns>世界坐标（格子中心）</returns>
        public Vector3 GridToWorld(int gridX, int gridY)
        {
            return new Vector3(gridX * _cellSize + _cellSize * 0.5f, 0, gridY * _cellSize + _cellSize * 0.5f);
        }

        /// <summary>
        /// 格子坐标转世界坐标
        /// </summary>
        /// <param name="gridPos">格子坐标</param>
        /// <returns>世界坐标（格子中心）</returns>
        public Vector3 GridToWorld(Vector2Int gridPos)
        {
            return GridToWorld(gridPos.x, gridPos.y);
        }

        /// <summary>
        /// 生成流场
        /// </summary>
        /// <param name="targets">目标位置和半径列表</param>
        /// <param name="unitRadius">单位半径</param>
        public void GenerateFlowField(List<(Vector3 position, float radius)> targets, float unitRadius = 0.5f)
        {
            if (!_isInitialized || targets == null || targets.Count == 0)
            {
                return;
            }

            // 将世界坐标转换为格子坐标
            var gridTargets = new (Vector2Int pos, float radius)[targets.Count];
            for (int i = 0; i < targets.Count; i++)
            {
                Vector2Int gridPos = WorldToGrid(targets[i].position);
                gridTargets[i] = (gridPos, targets[i].radius / _cellSize);
            }

            // 膨胀障碍物
            FlowFieldCalculator.InflateObstacles(_flowFieldData, unitRadius);

            // 生成流场
            FlowFieldCalculator.GenerateFlowField(_flowFieldData, gridTargets);

            // 更新格子数据中的流场信息
            UpdateGridFlowFieldData();
        }

        /// <summary>
        /// 获取指定位置的流场方向
        /// </summary>
        /// <param name="worldPosition">世界坐标</param>
        /// <returns>流场方向，如果不可通行或无流场数据则返回Vector2.zero</returns>
        public Vector2 GetFlowDirection(Vector3 worldPosition)
        {
            Vector2Int gridPos = WorldToGrid(worldPosition);
            if (!IsValidCoordinate(gridPos.x, gridPos.y))
            {
                return Vector2.zero;
            }

            Vector2Int dir = _flowFieldData.directionMap[gridPos.x, gridPos.y];
            return new Vector2(dir.x, dir.y).normalized;
        }

        /// <summary>
        /// 获取指定位置到目标的距离
        /// </summary>
        /// <param name="worldPosition">世界坐标</param>
        /// <returns>距离，如果不可通行或无流场数据则返回-1</returns>
        public int GetDistanceToTarget(Vector3 worldPosition)
        {
            Vector2Int gridPos = WorldToGrid(worldPosition);
            if (!IsValidCoordinate(gridPos.x, gridPos.y))
            {
                return -1;
            }

            return _flowFieldData.distanceMap[gridPos.x, gridPos.y];
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 更新单个格子的障碍物信息
        /// </summary>
        /// <param name="x">格子X坐标</param>
        /// <param name="y">格子Y坐标</param>
        private void UpdateFlowFieldObstacle(int x, int y)
        {
            if (IsValidCoordinate(x, y))
            {
                _flowFieldData.obstacleMap[x, y] = !_grid[x, y].IsWalkable;
            }
        }

        /// <summary>
        /// 更新格子数据中的流场信息
        /// </summary>
        private void UpdateGridFlowFieldData()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    var cell = _grid[x, y];
                    Vector2Int dir = _flowFieldData.directionMap[x, y];
                    cell.Direction = new Vector2(dir.x, dir.y).normalized;
                    cell.Distance = _flowFieldData.distanceMap[x, y];
                    cell.IsVisited = cell.Distance >= 0;
                    cell.IsBlocked = _flowFieldData.inflatedObstacleMap[x, y];
                }
            }
        }

        #endregion
    }
}