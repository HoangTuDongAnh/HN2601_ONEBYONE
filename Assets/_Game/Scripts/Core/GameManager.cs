using System.Collections.Generic;
using UnityEngine;
using _Game.Scripts.Grid;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Systems;
using _Game.Scripts.UI; // Thêm namespace Systems

namespace _Game.Scripts.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Data")]
        public ThemeData currentTheme;
        public List<LevelData> levels;

        [Header("Game Settings")] 
        public int initialShuffleCount = 3;
        public int bonusShuffleOnWin = 1;

        [Header("Game State")]
        public bool isPlaying = false;
        public int currentLevelIndex = 0;
        public float timeRemaining;
        public int totalScore;
        public int currentShuffleCount;
        public int pairsRemaining;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            StartNewGame();
        }

        public void StartNewGame()
        {
            totalScore = 0;
            currentLevelIndex = 0;
            currentShuffleCount = initialShuffleCount;
            
            UIManager.Instance.UpdateScoreUI(totalScore);
            UIManager.Instance.UpdateShuffleUI(currentShuffleCount);
            
            StartLevel(currentLevelIndex);
        }

        private void Update()
        {
            if (isPlaying)
            {
                timeRemaining -= Time.deltaTime;

                if (timeRemaining <= 0)
                {
                    GameOver();
                }
            }
        }

        public void StartLevel(int index)
        {
            if (index >= levels.Count)
            {
                Debug.Log("CHÚC MỪNG! BẠN ĐÃ PHÁ ĐẢO TOÀN BỘ GAME!");
                return;
            }

            currentLevelIndex = index;
            LevelData data = levels[index];

            // Reset thông số màn chơi
            timeRemaining = data.timeLimit;
            pairsRemaining = (data.rows * data.columns) / 2;
            isPlaying = true;

            GridManager.Instance.InitializeLevel(data, currentTheme);
            UIManager.Instance.UpdateLevelUI(currentLevelIndex);
            
            Debug.Log($"Bắt đầu Level {index + 1}. Mục tiêu: {pairsRemaining} cặp.");
        }

        public void AddScore(int amount)
        {
            totalScore += amount;
            UIManager.Instance.UpdateScoreUI(totalScore);
        }
        
        public bool TryConsumeAutoShuffle()
        {
            if (currentShuffleCount > 0)
            {
                currentShuffleCount--;
                UIManager.Instance.UpdateShuffleUI(currentShuffleCount);
                Debug.Log($"Đã dùng 1 lượt Shuffle cứu thua. Còn lại: {currentShuffleCount}");
                return true;
            }
            else
            {
                Debug.Log("Hết lượt Shuffle cứu thua -> Game Over!");
                GameOver();
                return false;
            }
        }
        
        public void OnPairMatched(Vector3 matchPosition)
        {
            // 1. Gọi Combo
            ComboManager.Instance.AddCombo(matchPosition);

            // 2. Trừ số cặp còn lại
            pairsRemaining--;

            // 3. Kiểm tra thắng
            if (pairsRemaining <= 0)
            {
                LevelComplete();
            }
        }

        private void LevelComplete()
        {
            isPlaying = false;
            
            currentShuffleCount += bonusShuffleOnWin;
            UIManager.Instance.UpdateShuffleUI(currentShuffleCount);
            Debug.Log($"Qua màn! Thưởng {bonusShuffleOnWin} lượt Shuffle.");

            Invoke(nameof(NextLevel), 2.0f);
        }

        private void GameOver()
        {
            isPlaying = false;
            timeRemaining = 0;
            Debug.Log("GAME OVER! Hết giờ.");
            // Hiện bảng thua cuộc, hỏi chơi lại
        }

        public void NextLevel()
        {
            StartLevel(currentLevelIndex + 1);
        }
    }
}