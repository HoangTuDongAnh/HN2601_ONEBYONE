using UnityEngine;
using TMPro; 

namespace _Game.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [Header("References")]
        public TextMeshProUGUI textLevel;
        public TextMeshProUGUI textScore;
        public TextMeshProUGUI textShuffle;

        [Header("Effects")]
        public FloatingText uiComboTextPrefab;
        
        private void Awake()
        {
            Instance = this;
        }
        
        public void UpdateLevelUI(int levelIndex)
        {
            if (textLevel != null)
                textLevel.text = "" + (levelIndex + 1).ToString();
        }
        
        public void UpdateScoreUI(int score)
        {
            if (textScore != null)
                textScore.text = score.ToString();
        }

        public void UpdateShuffleUI(int count)
        {
            if (textShuffle != null)
                textShuffle.text = count.ToString();
        }
        
        public void SpawnComboUI(int comboCount, float multiplier)
        {
            if (textScore == null) return;

            // 1. SỬA PARENT: Làm con của textScore để không bị Vertical Layout của Panel_Info chi phối
            FloatingText floating = Instantiate(uiComboTextPrefab, textScore.transform);

            // 2. Reset vị trí về giữa textScore rồi dịch sang phải/lên chút cho đẹp
            floating.transform.localPosition = new Vector3(0f, 20f, 0f); 

            // 3. SỬA NỘI DUNG: Hiển thị cả Combo và Hệ số
            // Ví dụ: "Combo x2 x1.5"
            string content = "";
            if (multiplier > 1f)
            {
                // Combo có hệ số: "Combo x5\nx2.5"
                content = $"Combo x{comboCount}\n<size=80%>x{multiplier:F1} point</size>";
            }
            else
            {
                // Combo đầu tiên: "Combo x1"
                content = $"Combo x{comboCount}";
            }
            
            // 4. Màu sắc: Đỏ rực nếu hệ số cao
            Color color = multiplier >= 3f ? Color.red : Color.yellow;
            
            floating.Init(content, color);
        }
    }
}