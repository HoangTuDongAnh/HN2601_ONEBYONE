using UnityEngine;
using TMPro; 

namespace _Game.Scripts.UI
{
    public class FloatingText : MonoBehaviour
    {
        // Dùng TMP_Text để hỗ trợ cả bản UI và bản World
        public TMP_Text textMesh; 
        public float moveSpeed = 50f; 
        public float lifeTime = 1f;

        public void Init(string text, Color color)
        {
            textMesh.text = text;
            textMesh.color = color;
            
            // Tự hủy sau lifeTime giây
            Destroy(gameObject, lifeTime);
        }

        private void Update()
        {
            // 1. Bay lên trên (Dùng transform.position vẫn đúng cho UI world position)
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;

            // 2. Mờ dần (Fade out)
            if (textMesh != null)
            {
                Color color = textMesh.color;
                // Giảm Alpha từ từ
                color.a = Mathf.Lerp(color.a, 0, Time.deltaTime / lifeTime); 
                textMesh.color = color;
            }
        }
    }
}