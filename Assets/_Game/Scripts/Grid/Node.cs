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
        public SpriteRenderer bgRenderer; // Đổi màu nền khi chọn
        #endregion
        
        #region Methods
        public void SetData(int dx, int dy, int did)
        {
            this.x = dx;
            this.y = dy;
            this.id = did;
            this.isMatched = false;

            if (textMesh != null)
            {
                textMesh.text = id.ToString();
            }

            gameObject.name = $"Node_{x}_{y}";
        }

        private void OnMouseDown()
        {
            if (isMatched) return;
            
            GridManager.Instance.OnNodeClicked(this);
        }

        public void SetSelected(bool isSelected)
        {
            if (bgRenderer != null)
            {
                bgRenderer.color = isSelected ? Color.green : Color.red;
            }
        }
        #endregion
    }
}
