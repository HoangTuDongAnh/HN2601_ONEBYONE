using UnityEngine;
using _Game.Scripts.Core; 

namespace _Game.Scripts.Systems
{
    public class ComboManager : MonoBehaviour
    {
        public static ComboManager Instance;

        [Header("Settings")]
        public float comboTimeout = 4.0f;
        public int baseScore = 100;      

        [Header("Debug Info")]
        public int currentCombo = 0;
        public float currentTimer = 0f;
        public int currentScoreMultiplier = 1;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            // Đếm ngược thời gian Combo
            if (currentTimer > 0)
            {
                currentTimer -= Time.deltaTime;
                
                // (Sau này bạn sẽ cập nhật thanh Slider UI ở đây)
            }
            else
            {
                // Hết giờ -> Reset Combo
                if (currentCombo > 0)
                {
                    ResetCombo();
                }
            }
        }

        public void AddCombo()
        {
            // 1. Tăng Combo
            currentCombo++;
            
            // 2. Reset đồng hồ đếm ngược
            currentTimer = comboTimeout;

            // 3. Tính hệ số nhân (Ví dụ: Combo 1->x1, Combo 5->x2, Combo 10->x3...)
            // Công thức đơn giản: 1 + (Combo / 5)
            currentScoreMultiplier = 1 + (currentCombo / 5);

            // 4. Tính điểm
            int pointsToAdd = baseScore * currentScoreMultiplier;
            
            // 5. Cộng điểm vào GameManager
            GameManager.Instance.AddScore(pointsToAdd);

            Debug.Log($"Combo: {currentCombo} (x{currentScoreMultiplier}) - Score: +{pointsToAdd}");
        }

        private void ResetCombo()
        {
            currentCombo = 0;
            currentScoreMultiplier = 1;
            Debug.Log("Combo Reset!");
        }
    }
}