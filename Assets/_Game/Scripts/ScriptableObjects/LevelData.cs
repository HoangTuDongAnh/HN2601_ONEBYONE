using _Game.Scripts.Utils;
using UnityEngine;

namespace _Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Level_01", menuName = "Game Data/Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("General Info")]
        public int levelIndex;
        public float timeLimit = 300f;

        [Header("Grid Settings")]
        public int rows = 9;
        public int columns = 16;

        [Header("Difficulty")]
        public GravityDirection gravityDirection;
    }
}