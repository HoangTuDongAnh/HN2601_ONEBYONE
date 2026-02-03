using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Core;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Utils;
using UnityEngine;

namespace _Game.Scripts.Grid
{
    [RequireComponent(typeof(BoardMechanics))] 
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance;

        [Header("References")]
        public Node nodePrefab;
        public LineRenderer linePrefab;
        public BoardMechanics mechanics; 

        [Header("Settings (Read-Only)")]
        public int rows;
        public int columns;
        public float cellSize = 1.1f;
        public GravityDirection currentGravity;

        [Header("Debug")]
        public Node[,] grid;
        private Node _selectedNode;
        private bool _isProcessing = false; 

        private void Awake()
        {
            Instance = this;
            mechanics = GetComponent<BoardMechanics>();
            mechanics.Initialize(this);
        }

        public void InitializeLevel(LevelData levelData, ThemeData themeData)
        {
            this.rows = levelData.rows;
            this.columns = levelData.columns;
            this.currentGravity = levelData.gravityDirection;

            ClearOldGrid();
            GenerateGrid(themeData);
        }

        private void GenerateGrid(ThemeData themeData)
        {
            List<int> valueList = new List<int>();
            int totalCells = columns * rows;
            int uniqueSprites = totalCells / 4;
            for (int i = 1; i <= uniqueSprites; i++)
                for (int j = 0; j < 4; j++) valueList.Add(i);

            // Shuffle đơn giản cho lúc Init
            for (int i = 0; i < valueList.Count; i++) {
                int temp = valueList[i];
                int r = Random.Range(i, valueList.Count);
                valueList[i] = valueList[r];
                valueList[r] = temp;
            }

            grid = new Node[columns, rows];
            
            Vector3 centerOffset = new Vector3(
                -(columns * cellSize) / 2.0f + cellSize / 2,
                -(rows * cellSize) / 2.0f + cellSize / 2,
                0
            );
            
            Vector3 startPos = transform.position + centerOffset;
            
            int index = 0;
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    Node newNode = Instantiate(nodePrefab, transform);
                    newNode.transform.position = startPos + new Vector3(x * cellSize, y * cellSize, 0);
                    
                    int value = valueList[index];
                    Sprite icon = (themeData != null) ? themeData.GetSpriteById(value) : null;
                    newNode.SetData(x, y, value, icon);
                    
                    grid[x, y] = newNode;
                    index++;
                }
            }
        }

        public void OnNodeClicked(Node clickedNode)
        {
            if (_isProcessing) return; // Đang ăn thì không cho click
            if (_selectedNode == null)
            {
                _selectedNode = clickedNode;
                _selectedNode.SetSelected(true);
                return;
            }
            if (_selectedNode == clickedNode)
            {
                _selectedNode.SetSelected(false);
                _selectedNode = null;
                return;
            }

            // Logic Check Match
            if (_selectedNode.id != clickedNode.id)
            {
                _selectedNode.SetSelected(false);
                _selectedNode = clickedNode;
                _selectedNode.SetSelected(true);
                return;
            }

            // Gọi Pathfinding Static
            List<Vector2Int> path = Pathfinding.FindPath(_selectedNode, clickedNode, grid, columns, rows);

            if (path != null && path.Count > 0)
            {
                StartCoroutine(HandleMatchRoutine(_selectedNode, clickedNode, path));
                _selectedNode.SetSelected(false);
            }
            else
            {
                _selectedNode.SetSelected(false);
                _selectedNode = clickedNode;
                _selectedNode.SetSelected(true);
            }
        }

        private IEnumerator HandleMatchRoutine(Node node1, Node node2, List<Vector2Int> path)
        {
            _isProcessing = true; // Khóa input

            // Vẽ đường
            LineRenderer line = Instantiate(linePrefab);
            line.positionCount = path.Count;
            for (int i = 0; i < path.Count; i++)
            {
                Vector3 pos = GetWorldPosition(path[i].x, path[i].y);
                pos.z = -1f;
                line.SetPosition(i, pos);
            }

            yield return new WaitForSeconds(0.3f);
            Destroy(line.gameObject);

            node1.isMatched = true;
            node2.isMatched = true;
            node1.gameObject.SetActive(false);
            node2.gameObject.SetActive(false);
            _selectedNode = null;
            
            Vector3 centerPos = (node1.transform.position + node2.transform.position) / 2f;
            
            GameManager.Instance.OnPairMatched(centerPos);
            
            yield return null;

            // --- GỌI MECHANICS ĐỂ XỬ LÝ DỒN & CHECK BÀI ---
            
            // 1. Dồn
            mechanics.ApplyGravity(grid, columns, rows, currentGravity, cellSize);
            if (currentGravity != GravityDirection.None) yield return new WaitForSeconds(0.4f);

            // 2. Kiểm tra Deadlock và Shuffle nếu cần
            yield return StartCoroutine(mechanics.CheckDeadlockAndShuffleRoutine(grid, columns, rows));

            _isProcessing = false; // Mở khóa input
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            Vector3 centerOffset = new Vector3(
                -(columns * cellSize) / 2.0f + cellSize / 2,
                -(rows * cellSize) / 2.0f + cellSize / 2,
                0
            );
            
            return transform.position + centerOffset + new Vector3(x * cellSize, y * cellSize, 0);
        }

        private void ClearOldGrid()
        {
            if (grid == null) return;
            foreach (var node in grid) if (node != null) Destroy(node.gameObject);
        }
    }
}