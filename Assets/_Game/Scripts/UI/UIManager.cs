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
    }
}