using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Grid
{
    public static class Pathfinding
    {
        // Class dữ liệu phụ trợ cho BFS
        public class BfsNode
        {
            public int x, y;
            public int direction;
            public int turns;
            public List<Vector2Int> path;

            public BfsNode(int x, int y, int dir, int turns, List<Vector2Int> path)
            {
                this.x = x;
                this.y = y;
                this.direction = dir;
                this.turns = turns;
                this.path = path;
            }
        }

        // Hàm FindPath dạng Static - Nhận dữ liệu Grid vào để xử lý
        public static List<Vector2Int> FindPath(Node start, Node end, Node[,] grid, int cols, int rows)
        {
            Queue<BfsNode> queue = new Queue<BfsNode>();
            HashSet<string> visited = new HashSet<string>();

            // -1: Chưa có hướng
            queue.Enqueue(new BfsNode(start.x, start.y, -1, 0, new List<Vector2Int> { new Vector2Int(start.x, start.y) }));

            int[] dx = { 0, 0, -1, 1 }; // Lên, Xuống, Trái, Phải
            int[] dy = { 1, -1, 0, 0 };

            while (queue.Count > 0)
            {
                BfsNode current = queue.Dequeue();

                if (current.x == end.x && current.y == end.y) return current.path;

                for (int i = 0; i < 4; i++)
                {
                    int nx = current.x + dx[i];
                    int ny = current.y + dy[i];

                    // 1. Kiểm tra biên (Cho phép đi ra vùng đệm -1)
                    if (nx < -1 || nx > cols || ny < -1 || ny > rows) continue;

                    // 2. Kiểm tra vật cản
                    if (nx >= 0 && nx < cols && ny >= 0 && ny < rows)
                    {
                        Node targetNode = grid[nx, ny];
                        // Logic sửa lỗi NullReference: Nếu node tồn tại (không null) thì mới check isMatched
                        if (targetNode != null)
                        {
                            if (!targetNode.isMatched && targetNode != end) continue;
                        }
                    }

                    // 3. Tính số lần rẽ
                    int newTurns = current.turns;
                    if (current.direction != -1 && current.direction != i) newTurns++;

                    if (newTurns > 2) continue;

                    // 4. Kiểm tra visited
                    string key = $"{nx},{ny},{i},{newTurns}";
                    if (visited.Contains(key)) continue;

                    visited.Add(key);
                    List<Vector2Int> newPath = new List<Vector2Int>(current.path) { new Vector2Int(nx, ny) };
                    queue.Enqueue(new BfsNode(nx, ny, i, newTurns, newPath));
                }
            }

            return null;
        }
    }
}