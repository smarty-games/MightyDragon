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
    }
}
