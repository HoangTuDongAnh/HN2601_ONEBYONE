using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewTheme", menuName = "Game Data/Theme Data")]
    public class ThemeData : ScriptableObject
    {
        [Header("Collection")]
        public List<Sprite> icons; 
        
        [Header("Background")]
        public Sprite backgroundSprite; 
        
        public Sprite GetSpriteById(int id)
        {
            // ID trong game logic của bạn đang là 1 -> 36
            // List index là 0 -> 35. Nên cần trừ 1
            int index = id - 1; 
            
            if (index >= 0 && index < icons.Count)
            {
                return icons[index];
            }
            return null; 
        }
    }
}