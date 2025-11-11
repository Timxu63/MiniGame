using UnityEngine;

namespace HotFixBattle
{
    /// <summary>
    /// MapManager 可视化组件，用于在场景视图中绘制格子和方向
    /// </summary>
    public class MapManagerVisualizer : MonoBehaviour
    {
        [Header("可视化设置")]
        [SerializeField] private bool _drawGrid = true;
        [SerializeField] private bool _drawFlowField = true;
        [SerializeField] private bool _drawObstacles = true;
        [SerializeField] private float _arrowLength = 0.4f;
        [SerializeField] private Color _gridColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
        [SerializeField] private Color _flowFieldColor = Color.yellow;
        [SerializeField] private Color _obstacleColor = Color.red;

        private MapManager _mapManager;

        private void Awake()
        {
            _mapManager = MapManager.Instance;
        }

        private void OnDrawGizmos()
        {
            if (_mapManager == null || !_mapManager.IsInitialized)
                return;

            DrawGrid();
            DrawFlowField();
            DrawObstacles();
        }

        /// <summary>
        /// 绘制网格
        /// </summary>
        private void DrawGrid()
        {
            if (!_drawGrid) return;

            Gizmos.color = _gridColor;

            // 绘制水平线
            for (int y = 0; y <= _mapManager.Height; y++)
            {
                Vector3 start = new Vector3(0, 0, y * _mapManager.CellSize);
                Vector3 end = new Vector3(_mapManager.Width * _mapManager.CellSize, 0, y * _mapManager.CellSize);
                Gizmos.DrawLine(start, end);
            }

            // 绘制垂直线
            for (int x = 0; x <= _mapManager.Width; x++)
            {
                Vector3 start = new Vector3(x * _mapManager.CellSize, 0, 0);
                Vector3 end = new Vector3(x * _mapManager.CellSize, 0, _mapManager.Height * _mapManager.CellSize);
                Gizmos.DrawLine(start, end);
            }
        }

        /// <summary>
        /// 绘制流场方向
        /// </summary>
        private void DrawFlowField()
        {
            if (!_drawFlowField) return;

            Gizmos.color = _flowFieldColor;

            for (int x = 0; x < _mapManager.Width; x++)
            {
                for (int y = 0; y < _mapManager.Height; y++)
                {
                    var cell = _mapManager.GetCell(x, y);
                    if (cell == null || !cell.IsWalkable || cell.Direction == Vector2.zero)
                        continue;

                    Vector3 worldPos = _mapManager.GridToWorld(x, y);
                    Vector3 direction = new Vector3(cell.Direction.x, 0, cell.Direction.y).normalized;

                    // 绘制箭头
                    Gizmos.DrawRay(worldPos, direction * _arrowLength);

                    // 绘制箭头头部
                    Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 150, 0) * Vector3.forward;
                    Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -150, 0) * Vector3.forward;
                    Gizmos.DrawRay(worldPos + direction * _arrowLength, right * _arrowLength * 0.25f);
                    Gizmos.DrawRay(worldPos + direction * _arrowLength, left * _arrowLength * 0.25f);
                }
            }
        }

        /// <summary>
        /// 绘制障碍物
        /// </summary>
        private void DrawObstacles()
        {
            if (!_drawObstacles) return;

            for (int x = 0; x < _mapManager.Width; x++)
            {
                for (int y = 0; y < _mapManager.Height; y++)
                {
                    var cell = _mapManager.GetCell(x, y);
                    if (cell == null || cell.IsWalkable)
                        continue;

                    Vector3 worldPos = _mapManager.GridToWorld(x, y);
                    Gizmos.color = cell.DebugColor == Color.black ? _obstacleColor : cell.DebugColor;
                    Gizmos.DrawCube(worldPos, new Vector3(_mapManager.CellSize * 0.9f, 0.1f, _mapManager.CellSize * 0.9f));
                }
            }
        }
    }
}
