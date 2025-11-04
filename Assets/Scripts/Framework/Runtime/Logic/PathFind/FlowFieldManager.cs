using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class FlowFieldManager : MonoBehaviour
{
    public int width = 50;
    public int height = 50;
    public float cellSize = 1f;

    public Transform player;
    public List<Transform> monsters;
    public float unitRadius = 0.4f; // 单位（怪物/玩家）半径

    public bool[,] obstacleMap;
    private Vector2Int[,] directionMap;
    private int[,] distanceMap;

    public float refreshInterval = 0.5f; 
    private bool isGenerating = false;

    private Vector2Int[] dirs = {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1)
    };

    void Start()
    {
        obstacleMap = new bool[width, height];
        directionMap = new Vector2Int[width, height];
        distanceMap = new int[width, height];

        // 示例障碍
        for (int x = 10; x < 20; x++)
        {
            obstacleMap[x, 25] = true;
        }

        // 膨胀障碍，根据半径标记周围格子不可走
        InflateObstacles(unitRadius);

        StartCoroutine(UpdateFlowFieldLoop());
    }

    void Update()
    {
        MoveMonsters();
    }

    /// <summary>
    /// 膨胀障碍物，半径换算成格子距离
    /// </summary>
    void InflateObstacles(float radius)
    {
        int inflateCells = Mathf.CeilToInt(radius / cellSize);

        bool[,] newMap = new bool[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (obstacleMap[x, y])
                {
                    for (int dx = -inflateCells; dx <= inflateCells; dx++)
                    {
                        for (int dy = -inflateCells; dy <= inflateCells; dy++)
                        {
                            int nx = x + dx;
                            int ny = y + dy;
                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                // 可以使用圆形膨胀
                                if (Vector2Int.Distance(new Vector2Int(x, y), new Vector2Int(nx, ny)) * cellSize <= radius)
                                {
                                    newMap[nx, ny] = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        obstacleMap = newMap;
    }

    IEnumerator UpdateFlowFieldLoop()
    {
        while (true)
        {
            yield return StartCoroutine(GenerateFlowFieldCoroutine());
            yield return new WaitForSeconds(refreshInterval);
        }
    }

    IEnumerator GenerateFlowFieldCoroutine()
    {
        isGenerating = true;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                distanceMap[x, y] = -1;
                directionMap[x, y] = Vector2Int.zero;
            }
        }

        Vector2Int playerCell = WorldToCell(player.position);
        if (!InBounds(playerCell))
        {
            isGenerating = false;
            yield break;
        }

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        distanceMap[playerCell.x, playerCell.y] = 0;
        queue.Enqueue(playerCell);

        int processedCount = 0;
        int maxPerFrame = 1000;

        while (queue.Count > 0)
        {
            var cell = queue.Dequeue();

            foreach (var d in dirs)
            {
                Vector2Int next = cell + d;
                if (!InBounds(next)) continue;
                if (obstacleMap[next.x, next.y]) continue;
                if (distanceMap[next.x, next.y] != -1) continue;

                // 防止斜角穿越
                if (Mathf.Abs(d.x) == 1 && Mathf.Abs(d.y) == 1)
                {
                    if (obstacleMap[cell.x + d.x, cell.y] || obstacleMap[cell.x, cell.y + d.y])
                        continue;
                }

                distanceMap[next.x, next.y] = distanceMap[cell.x, cell.y] + 1;
                directionMap[next.x, next.y] = -d;
                queue.Enqueue(next);
            }

            processedCount++;
            if (processedCount >= maxPerFrame)
            {
                processedCount = 0;
                yield return null;
            }
        }
        isGenerating = false;
    }

    void MoveMonsters()
    {
        // if (isGenerating) return;

        foreach (var m in monsters)
        {
            Vector2Int cell = WorldToCell(m.position);
            if (!InBounds(cell)) continue;

            Vector2Int dir = directionMap[cell.x, cell.y];
            if (dir != Vector2Int.zero)
            {
                Vector3 moveDir = new Vector3(dir.x, 0, dir.y).normalized;
                m.position += moveDir * Time.deltaTime * 3f;
            }
        }
    }

    Vector2Int WorldToCell(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / cellSize);
        int y = Mathf.FloorToInt(worldPos.z / cellSize);
        return new Vector2Int(x, y);
    }

    bool InBounds(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < width && cell.y >= 0 && cell.y < height;
    }

    void OnDrawGizmos()
    {
        if (obstacleMap == null || directionMap == null) return;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 cellCenter = new Vector3(x * cellSize + cellSize * 0.5f, 0, y * cellSize + cellSize * 0.5f);
                Gizmos.color = Color.gray;
                Gizmos.DrawWireCube(cellCenter, new Vector3(cellSize, 0.01f, cellSize));

                if (obstacleMap[x, y])
                {
                    Gizmos.color = new Color(1, 0, 0, 0.5f);
                    Gizmos.DrawCube(cellCenter, new Vector3(cellSize, 0.01f, cellSize));
                }
                else
                {
                    Vector2Int dir = directionMap[x, y];
                    if (dir != Vector2Int.zero)
                    {
                        Gizmos.color = Color.green;
                        Vector3 start = cellCenter;
                        Vector3 end = cellCenter + new Vector3(dir.x, 0, dir.y).normalized * 0.4f;
                        Gizmos.DrawLine(start, end);
                        Vector3 right = Quaternion.Euler(0, 45, 0) * (end - start).normalized * 0.15f;
                        Vector3 left = Quaternion.Euler(0, -45, 0) * (end - start).normalized * 0.15f;
                        Gizmos.DrawLine(end, end - right);
                        Gizmos.DrawLine(end, end - left);
                    }
                }
            }
        }
    }
}