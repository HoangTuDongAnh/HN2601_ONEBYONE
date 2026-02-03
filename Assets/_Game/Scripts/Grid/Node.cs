using System.Collections;
using TMPro;
using UnityEngine;

namespace _Game.Scripts.Grid
{
    public class Node : MonoBehaviour
    {
        #region Variables
        [Header("Data")] 
        public int id;
        public int x, y;
        public bool isMatched = false;
        
        [Header("Visual")]
        public TextMeshPro textMesh; // Tạm thay thế Image/Sprite
        public SpriteRenderer iconRenderer;
        public SpriteRenderer bgRenderer;
        #endregion
        
        #region Methods
        public void SetData(int dx, int dy, int did, Sprite sprite)
        {
            this.x = dx;
            this.y = dy;
            this.id = did;
            this.isMatched = false;

            // 1. Xử lý hiển thị Text (giữ lại để debug nếu chưa có ảnh)
            if (textMesh != null)
            {
                textMesh.text = id.ToString();
                textMesh.gameObject.SetActive(sprite == null); // Có ảnh thì ẩn số đi
            }

            // 2. Xử lý hiển thị Ảnh
            if (iconRenderer != null)
            {
                if (sprite != null)
                {
                    iconRenderer.sprite = sprite;
                    iconRenderer.gameObject.SetActive(true);
                }
                else
                {
                    // Nếu chưa gán ảnh trong Theme thì ẩn đi
                    iconRenderer.gameObject.SetActive(false);
                }
            }

            gameObject.name = $"Node_{x}_{y}";
            
            SetSelected(false);
        }

        private void OnMouseDown()
        {
            if (isMatched) return;
            
            if (GridManager.Instance != null)
            {
                GridManager.Instance.OnNodeClicked(this);
            }
        }

        public void SetSelected(bool isSelected)
        {
            if (bgRenderer != null)
            {
                bgRenderer.color = isSelected ? Color.green : Color.white;
            }
        }
        
        public void UpdateCoordinate(int newX, int newY)
        {
            this.x = newX;
            this.y = newY;
            gameObject.name = $"Node_{x}_{y}"; // Đổi tên để dễ debug
        }
        
        public void MoveToPosition(Vector3 targetPos)
        {
            StopAllCoroutines(); // Dừng các chuyển động cũ nếu có
            StartCoroutine(MoveRoutine(targetPos));
        }
        
        private IEnumerator MoveRoutine(Vector3 targetPos)
        {
            float duration = 0.3f; 
            float elapsed = 0f;
            Vector3 startPos = transform.position;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.position = Vector3.Lerp(startPos, targetPos, t * t); 
                yield return null;
            }

            transform.position = targetPos;
        }
        #endregion
    }
}
