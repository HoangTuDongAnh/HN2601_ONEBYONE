using System.Collections.Generic;
using UnityEngine;
using _Game.Scripts.Grid;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Systems; // Thêm namespace Systems

namespace _Game.Scripts.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Data")]
        public ThemeData currentTheme;
        public List<LevelData> levels;

        [Header("Game State")]
        public bool isPlaying = false;
        public int currentLevelIndex = 0;
        public float timeRemaining;
        public int totalScore;
        public int pairsRemaining;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            StartLevel(currentLevelIndex);
        }

        private void Update()
        {
            if (isPlaying)
            {
                // Trừ thời gian
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
            
            // Gọi GridManager tạo map
            GridManager.Instance.InitializeLevel(data, currentTheme);
            
            Debug.Log($"Bắt đầu Level {index + 1}. Mục tiêu: {pairsRemaining} cặp.");
        }

        public void AddScore(int amount)
        {
            totalScore += amount;
            // (Sau này sẽ gọi UI update điểm ở đây)
        }

        // Hàm được gọi từ GridManager khi ăn thành công 1 cặp
        public void OnPairMatched()
        {
            // 1. Gọi Combo
            ComboManager.Instance.AddCombo();

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
            Debug.Log("VICTORY! Đang chuyển màn...");
            
            // Tạm thời chuyển màn luôn sau 2 giây (sau này sẽ hiện bảng Victory UI)
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