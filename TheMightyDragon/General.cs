using System;
namespace Desktop
{
    public static class General
    {
        public enum eMoveType
        {
            ToColission = 0,
            ToTile = 1
        }
        /// <summary>
        /// Map tiles legend
        /// </summary>
        public enum Legend
        {
            Crater = 2,  // where the Dragon lives
            Path = 1,
            LeftMark = -1,  // put -1 in the left position of Player moving path
            RightMark = -2, // put -2 in the left position of Player moving path
            Mountain = 4,
            Player = 5,
            Dragon = 6

        }
        public enum eDirection
        {
            None = 0,
            Idle = 1,
            Left = 2,
            Down = 4,
            Right = 8,
            Up = 16,
            Attack = 32
        }
        public enum ePlayerType
        {
            Player = 0,
            Dragon = 1
        }
        public enum eGroundType
        {
            Safe = 0,
            Danger = 1
        }
        public static int GameSpeed = 2;
        public static int TileSize = 32;

        public const int TilesHorizontaly = 25;
        public const int TilesVertically = 15;
    }
}
