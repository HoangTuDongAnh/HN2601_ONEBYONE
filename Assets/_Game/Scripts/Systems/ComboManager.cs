using UnityEngine;
using _Game.Scripts.Core;
using _Game.Scripts.UI;
using _Game.Scripts.Utils;

namespace _Game.Scripts.Systems
{
    public class ComboManager : MonoBehaviour
    {
        public static ComboManager Instance;

        [Header("Score Settings")]
        public int baseScore = 10;        
        public float multiplierStep = 0.5f; 

        [Header("Time Settings (Difficulty)")]
        public float maxComboTime = 5.0f;      
        public float minComboTime = 1.5f;      
        public float timeDecayPerCombo = 0.3f;  

        [Header("Debug Info")]
        public int currentCombo = 0;
        public float currentTimer = 0f;
        public float currentLimit = 0f;      
        public float currentScoreMultiplier = 1f;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            // Đếm ngược
            if (currentTimer > 0)
            {
                currentTimer -= Time.deltaTime;
                
                // (Gợi ý: Tại đây bạn có thể cập nhật FillAmount của UI Slider để làm thanh năng lượng tụt dần)
            }
            else
            {
                // Hết giờ -> Reset
                if (currentCombo > 0)
                {
                    ResetCombo();
                }
            }
        }

        public void AddCombo(Vector3 matchPos)
        {
            // 1. Tăng biến đếm Combo
            currentCombo++;

            // 2. TÍNH TOÁN THỜI GIAN GIỚI HẠN (Time Limit)
            // Vẫn giữ cơ chế càng combo cao thời gian càng ngắn
            // Lưu ý: Combo 1 vẫn bị trừ thời gian một chút để tạo áp lực ngay từ đầu, 
            // hoặc bạn có thể dùng (currentCombo - 1) nếu muốn cặp đầu tiên thư thả hơn.
            currentLimit = Mathf.Max(minComboTime, maxComboTime - (currentCombo * timeDecayPerCombo));
    
            // Reset đồng hồ
            currentTimer = currentLimit;

            // 3. TÍNH TOÁN HỆ SỐ ĐIỂM (SỬA ĐỔI THEO YÊU CẦU CỦA BẠN)
            if (currentCombo <= 1)
            {
                // Lần ăn đầu tiên: Hệ số vẫn là 1 (Chưa nhân)
                currentScoreMultiplier = 1f; 
            }
            else
            {
                // Từ lần ăn thứ 2 trở đi mới bắt đầu tính cộng dồn
                // Công thức: 1 + ((Combo hiện tại - 1) * Bước nhảy)
                // Ví dụ Step = 0.5:
                // - Combo 2: 1 + (1 * 0.5) = x1.5
                // - Combo 3: 1 + (2 * 0.5) = x2.0
                currentScoreMultiplier = 1f + ((currentCombo - 1) * multiplierStep);
            }
            
            // 4. HIỆU ỨNG
            UIManager.Instance.SpawnComboUI(currentCombo, currentScoreMultiplier);
            
            // 5. TÍNH ĐIỂM
            int pointsToAdd = Mathf.RoundToInt(baseScore * currentScoreMultiplier);
    
            // Cộng vào GameManager
            GameManager.Instance.AddScore(pointsToAdd);

            Debug.Log($"Combo: {currentCombo} | Time: {currentLimit:F1}s | Mult: x{currentScoreMultiplier:F1} | Score: +{pointsToAdd}");
        }

        private void ResetCombo()
        {
            currentCombo = 0;
            currentScoreMultiplier = 1;
            Debug.Log("Combo Reset!");
        }
    }
}