using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.Grid
{
    public class GridManager : MonoBehaviour
    {
        #region Variables
        public static GridManager Instance;
        
        [Header("Settings")] 
        public int rows = 9;
        public int cols = 16;
        public Node nodePrefab;
        public float cellSize = 1.1f;

        [Header("Debug")] 
        public Node[,] grid;
        
        private Node _selectedNode;
        #endregion
        
        #region Unity Callbacks
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            GenerateGrid();
        }
        #endregion
        
        #region Public Methods
        private void GenerateGrid()
        {
            // 1. Setup data
            List<int> valueList = new List<int>();
            int totalCells = cols * rows;
            int pairsCount = 4;
            int uniqueSprites = totalCells / pairsCount;

            for (int i = 1; i <= uniqueSprites; i++)
            {
                for (int j = 0; j < pairsCount; j++)
                {
                    valueList.Add(i);
                }
            }
            
            // 2. Shuffle list
            Shuffle(valueList);

            // 3. Visual
            grid = new Node[cols, rows];

            Vector3 startPos = new Vector3(
                -(cols * cellSize) / 2.0f + cellSize / 2,
                -(rows * cellSize) / 2.0f + cellSize / 2,
                0
            );

            int index = 0;

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    Node newNode = Instantiate(nodePrefab, transform);

                    Vector3 pos = new Vector3(x * cellSize, y * cellSize, 0) + startPos;
                    newNode.transform.position = pos;

                    int value = valueList[index];
                    newNode.SetData(x, y, value);
                    
                    grid[x, y] = newNode;
                
                    index++;
                }
            }
        }
        
        public void OnNodeClicked(Node clickedNode)
        {
            // Trường hợp 1: Chưa chọn ô nào cả
            if (_selectedNode == null)
            {
                _selectedNode = clickedNode;
                _selectedNode.SetSelected(true); 
                Debug.Log($"Chọn ô đầu tiên: {clickedNode.x}, {clickedNode.y}");
                return;
            }

            // Trường hợp 2: Click lại vào chính ô đang chọn -> Bỏ chọn
            if (_selectedNode == clickedNode)
            {
                _selectedNode.SetSelected(false);
                _selectedNode = null;
                Debug.Log("Bỏ chọn.");
                return;
            }

            // Trường hợp 3: Chọn ô thứ 2 -> Kiểm tra Logic
            // A. Kiểm tra ID có giống nhau không?
            if (_selectedNode.id != clickedNode.id)
            {
                Debug.Log("Sai hình! Chọn lại.");
                _selectedNode.SetSelected(false); 
                _selectedNode = clickedNode;  
                _selectedNode.SetSelected(true);
                return;
            }

            // B. Kiểm tra đường đi (BFS)
            Debug.Log("Hình giống nhau! Đang tìm đường...");
            List<Vector2Int> path = FindPath(_selectedNode, clickedNode);

            if (path != null && path.Count > 0)
            {
                Debug.Log("MATCH THÀNH CÔNG!");
                
                // Xử lý ăn điểm
                _selectedNode.isMatched = true;
                clickedNode.isMatched = true;
                
                // Ẩn 2 ô đi (hoặc Destroy tùy bạn, tạm thời tắt Text)
                _selectedNode.gameObject.SetActive(false); 
                clickedNode.gameObject.SetActive(false);

                // Reset
                _selectedNode = null;
            }
            else
            {
                Debug.Log("Không tìm thấy đường đi (Bị chặn hoặc quá xa)!");
                _selectedNode.SetSelected(false);
                _selectedNode = clickedNode; 
                _selectedNode.SetSelected(true);
            }
        }
        #endregion

        #region Private Methods
        // Fisher-Yates Algorithm
        private void Shuffle(List<int> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int temp = list[i];
                int randomIndex = UnityEngine.Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }
        
        // BFS Algorithm to find path
        private List<Vector2Int> FindPath(Node start, Node end)
        {
            // Queue lưu trạng thái duyệt: {x, y, hướng đi, số lần rẽ, danh sách đường đi}
            Queue<BfsNode> queue = new Queue<BfsNode>();
            
            // HashSet để tránh duyệt lại 1 ô với cùng trạng thái (x,y,dir) -> Tối ưu
            HashSet<string> visited = new HashSet<string>();

            // Khởi tạo các hướng đi (Lên, Xuống, Trái, Phải)
            // -1: Chưa có hướng
            queue.Enqueue(new BfsNode(start.x, start.y, -1, 0, new List<Vector2Int> { new Vector2Int(start.x, start.y) }));

            int[] dx = { 0, 0, -1, 1 }; // Lên, Xuống, Trái, Phải
            int[] dy = { 1, -1, 0, 0 };

            while (queue.Count > 0)
            {
                BfsNode current = queue.Dequeue();

                // Nếu đã đến đích
                if (current.x == end.x && current.y == end.y)
                {
                    return current.path;
                }

                // Duyệt 4 hướng
                for (int i = 0; i < 4; i++)
                {
                    int nx = current.x + dx[i];
                    int ny = current.y + dy[i];

                    // 1. Kiểm tra giới hạn biên mở rộng (Cho phép đi ra ngoài 1 ô: từ -1 đến cols)
                    // Phạm vi hợp lệ: [-1] đến [cols] và [-1] đến [rows]
                    if (nx < -1 || nx > cols || ny < -1 || ny > rows) continue;

                    // 2. Kiểm tra vật cản
                    // TRƯỜNG HỢP A: Nếu tọa độ nằm BÊN TRONG lưới thật
                    if (nx >= 0 && nx < cols && ny >= 0 && ny < rows)
                    {
                        Node targetNode = grid[nx, ny];
                        // Nếu ô đó chưa bị ăn (vẫn còn hình) VÀ không phải là ô đích -> Bị chặn
                        if (!targetNode.isMatched && targetNode != end) continue;
                    }

                    // 3. Tính số lần rẽ (Turns)
                    int newTurns = current.turns;
                    if (current.direction != -1 && current.direction != i)
                    {
                        newTurns++;
                    }

                    // 4. Kiểm tra giới hạn rẽ (Max 2 lần)
                    if (newTurns > 2) continue;

                    // 5. Kiểm tra đã duyệt chưa (để tối ưu)
                    string key = $"{nx},{ny},{i},{newTurns}"; 
                    if (visited.Contains(key)) continue;

                    // Thêm vào hàng đợi
                    visited.Add(key);
                    
                    // Tạo list đường đi mới
                    List<Vector2Int> newPath = new List<Vector2Int>(current.path);
                    newPath.Add(new Vector2Int(nx, ny));

                    queue.Enqueue(new BfsNode(nx, ny, i, newTurns, newPath));
                }
            }

            return null; 
        }
        #endregion
    }
    
    [System.Serializable] // Có thể thêm cái này nếu muốn hiện trong Inspector để debug (tùy chọn)
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
}