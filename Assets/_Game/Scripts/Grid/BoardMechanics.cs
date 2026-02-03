using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Utils;
using UnityEngine;

namespace _Game.Scripts.Grid
{
    public class BoardMechanics : MonoBehaviour
    {
        private GridManager _manager;

        public void Initialize(GridManager manager)
        {
            _manager = manager;
        }

        // --- 1. XỬ LÝ TRỌNG LỰC (GRAVITY) ---
        public void ApplyGravity(Node[,] grid, int cols, int rows, GravityDirection direction, float cellSize)
        {
            if (direction == GravityDirection.None) return;

            // Dồn Dọc (Lên/Xuống)
            if (direction == GravityDirection.Down || direction == GravityDirection.Up)
            {
                for (int x = 0; x < cols; x++)
                {
                    List<Node> activeNodes = new List<Node>();
                    for (int y = 0; y < rows; y++)
                    {
                        if (grid[x, y] != null && !grid[x, y].isMatched)
                            activeNodes.Add(grid[x, y]);
                        grid[x, y] = null;
                    }

                    for (int i = 0; i < activeNodes.Count; i++)
                    {
                        int newY = (direction == GravityDirection.Down) ? i : (rows - 1) - i;
                        MoveNodeTo(activeNodes[i], x, newY, grid);
                    }
                }
            }
            // Dồn Ngang (Trái/Phải)
            else if (direction == GravityDirection.Left || direction == GravityDirection.Right)
            {
                for (int y = 0; y < rows; y++)
                {
                    List<Node> activeNodes = new List<Node>();
                    for (int x = 0; x < cols; x++)
                    {
                        if (grid[x, y] != null && !grid[x, y].isMatched)
                            activeNodes.Add(grid[x, y]);
                        grid[x, y] = null;
                    }

                    for (int i = 0; i < activeNodes.Count; i++)
                    {
                        int newX = (direction == GravityDirection.Left) ? i : (cols - 1) - i;
                        MoveNodeTo(activeNodes[i], newX, y, grid);
                    }
                }
            }
        }

        private void MoveNodeTo(Node node, int x, int y, Node[,] grid)
        {
            grid[x, y] = node;
            node.UpdateCoordinate(x, y);
            node.MoveToPosition(_manager.GetWorldPosition(x, y)); // Gọi ngược lại hàm lấy vị trí
        }

        // --- 2. KIỂM TRA DEADLOCK & SHUFFLE ---
        
        // Coroutine này sẽ được gọi sau khi ăn xong
        public IEnumerator CheckDeadlockAndShuffleRoutine(Node[,] grid, int cols, int rows)
        {
            // Nếu vẫn còn nước đi -> Kết thúc
            if (HasAvailableMoves(grid, cols, rows)) yield break;

            // Nếu hết nước -> Shuffle liên tục đến khi có nước thì thôi
            int maxRetries = 10;
            do
            {
                ShuffleBoard(grid, cols, rows);
                yield return new WaitForSeconds(0.5f); // Đợi visual bay về chỗ cũ
                maxRetries--;
            } 
            while (!HasAvailableMoves(grid, cols, rows) && maxRetries > 0);
        }

        private bool HasAvailableMoves(Node[,] grid, int cols, int rows)
        {
            // Gom nhóm Node theo ID để check nhanh
            Dictionary<int, List<Node>> groups = new Dictionary<int, List<Node>>();
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    Node node = grid[x, y];
                    if (node != null && !node.isMatched)
                    {
                        if (!groups.ContainsKey(node.id)) groups[node.id] = new List<Node>();
                        groups[node.id].Add(node);
                    }
                }
            }

            foreach (var list in groups.Values)
            {
                if (list.Count < 2) continue;
                for (int i = 0; i < list.Count; i++)
                {
                    for (int j = i + 1; j < list.Count; j++)
                    {
                        // Dùng Pathfinding Static để check
                        if (Pathfinding.FindPath(list[i], list[j], grid, cols, rows) != null) return true;
                    }
                }
            }
            return false;
        }

        private void ShuffleBoard(Node[,] grid, int cols, int rows)
        {
            Debug.Log("Deadlock! Shuffling...");
            List<Node> activeNodes = new List<Node>();
            List<Vector2Int> positions = new List<Vector2Int>();

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    if (grid[x, y] != null && !grid[x, y].isMatched)
                    {
                        activeNodes.Add(grid[x, y]);
                        positions.Add(new Vector2Int(x, y));
                    }
                }
            }

            // Shuffle List
            for (int i = 0; i < activeNodes.Count; i++)
            {
                Node temp = activeNodes[i];
                int r = Random.Range(i, activeNodes.Count);
                activeNodes[i] = activeNodes[r];
                activeNodes[r] = temp;
            }

            // Gán lại
            for (int i = 0; i < activeNodes.Count; i++)
            {
                MoveNodeTo(activeNodes[i], positions[i].x, positions[i].y, grid);
            }
        }
    }
}