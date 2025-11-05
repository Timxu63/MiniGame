using System;
using System.Collections.Generic;
using FlowFieldSystem;
using UnityEngine;

namespace FlowFieldSystem
{
    /// <summary>
    /// 地图和流场数据容器
    /// </summary>
    public class FlowFieldData
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public float CellSize { get; private set; }

        // 原始阻挡
        public bool[,] obstacleMap;
        // 膨胀阻挡（考虑单位半径）
        public bool[,] inflatedObstacleMap;
        // BFS结果
        public Vector2Int[,] directionMap;
        public int[,] distanceMap;

        public FlowFieldData(int width, int height, float cellSize)
        {
            Width = width;
            Height = height;
            CellSize = cellSize;

            obstacleMap = new bool[width, height];
            inflatedObstacleMap = new bool[width, height];
            directionMap = new Vector2Int[width, height];
            distanceMap = new int[width, height];
        }

        public bool InBounds(Vector2Int c)
        {
            return c.x >= 0 && c.x < Width && c.y >= 0 && c.y < Height;
        }
    }

    /// <summary>
    /// 流场寻路算法（8向 + 单位半径碰撞 + 目标大小支持）
    /// </summary>
    public static class FlowFieldCalculator
    {
        // 8方向
        private static readonly Vector2Int[] dirs = {
            new Vector2Int(0, 1),    // 上
            new Vector2Int(0, -1),   // 下
            new Vector2Int(-1, 0),   // 左
            new Vector2Int(1, 0),    // 右
            new Vector2Int(1, 1),    // 右上
            new Vector2Int(1, -1),   // 右下
            new Vector2Int(-1, 1),   // 左上
            new Vector2Int(-1, -1)   // 左下
        };

        /// <summary>
        /// 根据单位半径膨胀阻挡
        /// </summary>
        public static void InflateObstacles(FlowFieldData data, float unitRadius)
        {
            int inflateCells = (int)Math.Ceiling(unitRadius / data.CellSize);
            bool[,] newMap = new bool[data.Width, data.Height];

            for (int x = 0; x < data.Width; x++)
            {
                for (int y = 0; y < data.Height; y++)
                {
                    if (data.obstacleMap[x, y])
                    {
                        for (int dx = -inflateCells; dx <= inflateCells; dx++)
                        {
                            for (int dy = -inflateCells; dy <= inflateCells; dy++)
                            {
                                int nx = x + dx;
                                int ny = y + dy;
                                if (nx >= 0 && nx < data.Width && ny >= 0 && ny < data.Height)
                                {
                                    float dist = (float)Math.Sqrt(dx * dx + dy * dy) * data.CellSize;
                                    if (dist <= unitRadius)
                                    {
                                        newMap[nx, ny] = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            data.inflatedObstacleMap = newMap;
        }

        /// <summary>
        /// 把目标大小转换成 BFS 起点集合
        /// </summary>
        private static void AddTargetArea(List<Vector2Int> startPoints, FlowFieldData data, Vector2Int center, float targetRadius)
        {
            int radCells = (int)Math.Ceiling(targetRadius / data.CellSize);
            for (int dx = -radCells; dx <= radCells; dx++)
            {
                for (int dy = -radCells; dy <= radCells; dy++)
                {
                    int nx = center.x + dx;
                    int ny = center.y + dy;
                    if (nx >= 0 && nx < data.Width && ny >= 0 && ny < data.Height)
                    {
                        double dist = Math.Sqrt(dx * dx + dy * dy) * data.CellSize;
                        if (dist <= targetRadius)
                        {
                            startPoints.Add(new Vector2Int(nx, ny));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 生成流场（多个目标，每个有位置&大小，考虑单位半径碰撞）
        /// </summary>
        public static void GenerateFlowField(FlowFieldData data, (Vector2Int pos, float radius)[] targets)
        {
            // 初始化 distance & direction
            for (int x = 0; x < data.Width; x++)
            {
                for (int y = 0; y < data.Height; y++)
                {
                    data.distanceMap[x, y] = -1;
                    data.directionMap[x, y] = new Vector2Int(0, 0);
                }
            }

            // BFS 起点集合
            List<Vector2Int> startPoints = new List<Vector2Int>();
            foreach (var target in targets)
            {
                AddTargetArea(startPoints, data, target.pos, target.radius);
            }

            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            foreach (var sp in startPoints)
            {
                if (!data.InBounds(sp) || data.inflatedObstacleMap[sp.x, sp.y]) continue;
                data.distanceMap[sp.x, sp.y] = 0;
                queue.Enqueue(sp);
            }

            // BFS
            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();
                foreach (var d in dirs)
                {
                    var next = cell + d;
                    if (!data.InBounds(next)) continue;
                    if (data.inflatedObstacleMap[next.x, next.y]) continue;
                    if (data.distanceMap[next.x, next.y] != -1) continue;

                    // 防止斜角穿越
                    if (Math.Abs(d.x) == 1 && Math.Abs(d.y) == 1)
                    {
                        if (data.inflatedObstacleMap[cell.x + d.x, cell.y] ||
                            data.inflatedObstacleMap[cell.x, cell.y + d.y])
                            continue;
                    }

                    data.distanceMap[next.x, next.y] = data.distanceMap[cell.x, cell.y] + 1;
                    data.directionMap[next.x, next.y] = new Vector2Int(-d.x, -d.y);
                    queue.Enqueue(next);
                }
            }
        }
    }
}