using System.Collections.Generic;
using UnityEngine;
using _Game.Scripts.Grid;
using _Game.Scripts.ScriptableObjects;

namespace _Game.Scripts.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Data")]
        public ThemeData currentTheme;       // Kéo file Theme vào đây
        public List<LevelData> levels;       // Kéo các file Level 1, 2, 3... vào đây

        [Header("State")]
        public int currentLevelIndex = 0;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            StartLevel(currentLevelIndex);
        }

        public void StartLevel(int index)
        {
            if (index >= levels.Count)
            {
                Debug.Log("Đã hết màn chơi! You Win!");
                return;
            }

            currentLevelIndex = index;
            LevelData data = levels[index];

            Debug.Log($"Đang tải Level {data.levelIndex} - Gravity: {data.gravityDirection}");

            GridManager.Instance.InitializeLevel(data, currentTheme);
        }

        public void NextLevel()
        {
            StartLevel(currentLevelIndex + 1);
        }
    }
}