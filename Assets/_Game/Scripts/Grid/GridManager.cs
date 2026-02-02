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
        
        private Node selectedNode;
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
        #endregion
    }
}